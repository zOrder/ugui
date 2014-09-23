using System;
using System.Collections.Generic;
using System.Reflection;

namespace MinMVC
{
	public class Injector : IInjector
	{
		public event Action onCleanUp;

		readonly IDictionary<Type,InjectionInfo> _infoMap = new Dictionary<Type,InjectionInfo>();
		readonly IDictionary<Type, Type> _typeMap = new Dictionary<Type, Type>();
		readonly IDictionary<Type, object> _instanceCache = new Dictionary<Type, object>();

		IInjector _parent;

		public IInjector parent
		{
			set
			{
				if (_parent != null)
				{
					_parent.onCleanUp -= CleanUp;
				}

				_parent = value;

				if (_parent != null)
				{
					_parent.onCleanUp += CleanUp;
				}
			}
		}

		public Injector()
		{
			Register<IInjector>(this);
		}

		public void CleanUp()
		{
			parent = null;
			onCleanUp();
		}

		public void Register<T>(bool preventCaching = false)
		{
			Register<T, T>(preventCaching);
		}

		public void Register<TInterface, TClass>(bool preventCaching = false)
		{
			Type key = typeof(TInterface);

			if (!_typeMap.ContainsKey(key))
			{
				_typeMap[key] = typeof(TClass);

				if (!preventCaching)
				{
					Register(key, default(TClass));
				}
			}
			else
			{
				throw new Exception("already registered " + key);
			}
		}

		public void Register(Type type, Type value, bool preventCaching = false)
		{
			if (!_typeMap.ContainsKey(type))
			{
				_typeMap[type] = value;

				if (!preventCaching)
				{
					Register(type, default(object));
				}
			}
			else
			{
				throw new Exception("already registered " + type);
			}
		}

		public void Register<T>(T instance)
		{
			Type key = typeof(T);
			Register(key, instance);
		}

		void Register<T>(Type type, T instance)
		{
			object cachedInstance;
			_instanceCache.TryGetValue(type, out cachedInstance);

			if (cachedInstance == null)
			{
				_instanceCache[type] = instance;
			}
			else
			{
				throw new Exception("already cached " + type);
			}
		}

		public T Get<T>() where T: class
		{
			Type type = typeof(T);

			return Get<T>(type);
		}

		public T Get<T>(Type type) where T: class
		{
			return (T)Get(type);
		}

		public object Get(Type type)
		{
			object instance;
			_instanceCache.TryGetValue(type, out instance);

			if (instance == null)
			{
				Type value;

				if (_typeMap.TryGetValue(type, out value))
				{
					instance = Activator.CreateInstance(value);
					Inject(instance);

					if (_instanceCache.ContainsKey(type))
					{
						_instanceCache[type] = instance;
					}
				}
				else if (_parent != null)
				{
					instance = _parent.Get(type); 
				}
			}

			if (instance == null)
			{
				throw new Exception("no instance for " + type);
			}

			return instance;
		}

		public bool Has<T>()
		{
			Type type = typeof(T);

			return Has(type);
		}

		public bool Has(Type type)
		{
			bool hasType = _instanceCache.ContainsKey(type) || _typeMap.ContainsKey(type);

			if (!hasType && _parent != null)
			{
				hasType = _parent.Has(type);
			}

			return hasType;
		}

		void Inject<T>(T instance)
		{
			Type type = instance.GetType();
			InjectionInfo info;

			if (!_infoMap.TryGetValue(type, out info))
			{
				_infoMap[type] = info = ParseInfo(type);
			}

			if (info != null)
			{
				InjectProperties(instance, type, info.properties, BindingFlags.SetProperty);
				InjectProperties(instance, type, info.fields, BindingFlags.SetField);
				InvokePostInjections(instance, info.postInjections);
			}
		}

		InjectionInfo ParseInfo(Type type)
		{
			IDictionary<string, Type> properties = GetPropertyInjections(type.GetProperties());
			IDictionary<string, Type> fields = GetFieldInjections(type.GetFields());
			IList<MethodInfo> posts = GetPostInjections(type.GetMethods());

			InjectionInfo info = null;

			if (properties != null || fields != null || posts != null)
			{
				info = new InjectionInfo
				{
					properties = properties,
					fields = fields,
					postInjections = posts
				};
			}

			return info;
		}

		void InjectProperties<T>(T instance, Type type, IDictionary<string, Type> properties, BindingFlags flags)
		{
			if (properties != null)
			{
				foreach (KeyValuePair<string, Type> pair in properties)
				{
					object injection = Get(pair.Value);
					object[] param = { injection };

					type.InvokeMember(pair.Key, flags, null, instance, param);
				}
			}
		}

		void InvokePostInjections(object instance, IList<MethodInfo> postInjections)
		{
			if (postInjections != null)
			{
				object[] param = new object[0];

				for (int i = 0, postInjectionsCount = postInjections.Count; i < postInjectionsCount; i++)
				{
					postInjections[i].Invoke(instance, param);
				}
			}
		}

		IList<MethodInfo> GetPostInjections(MethodInfo[] methods)
		{
			IList<MethodInfo> postInjections = null;

			for (int i = 0, methodsLength = methods.Length; i < methodsLength; i++)
			{
				MethodInfo method = methods[i];
				object[] attributes = method.GetCustomAttributes(true);

				for (int j = 0, attributesLength = attributes.Length; j < attributesLength; j++)
				{
					if (attributes[j] is PostInjectionAttribute)
					{
						postInjections = postInjections ?? new List<MethodInfo>();
						postInjections.Add(method);
					}
				}
			}

			return postInjections;
		}

		IDictionary<string, Type> GetFieldInjections(FieldInfo[] fields)
		{
			IDictionary<string, Type> injections = null;

			for (int i = 0, length = fields.Length; i < length; i++)
			{
				FieldInfo field = fields[i];
				injections = ParseAttributes(field, field.FieldType, injections);
			}

			return injections;
		}

		IDictionary<string, Type> GetPropertyInjections(PropertyInfo[] properties)
		{
			IDictionary<string, Type> injections = null;

			for (int i = 0, length = properties.Length; i < length; i++)
			{
				PropertyInfo property = properties[i];
				injections = ParseAttributes(property, property.PropertyType, injections);
			}

			return injections;
		}

		IDictionary<string, Type> ParseAttributes(MemberInfo info, Type type, IDictionary<string, Type> propInjections)
		{
			object[] attributes = info.GetCustomAttributes(true);
		
			for (int i = 0, attributesLength = attributes.Length; i < attributesLength; i++)
			{
				if (attributes[i] is InjectAttribute)
				{
					propInjections = propInjections ?? new Dictionary<string, Type>();
					propInjections[info.Name] = type;
				}
			}

			return propInjections;
		}
	}

	public class InjectionInfo
	{
		public IList<MethodInfo> postInjections;
		public IDictionary<string, Type> fields;
		public IDictionary<string, Type> properties;
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class InjectAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class PostInjectionAttribute : Attribute
	{
	}
}
