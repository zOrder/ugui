using System;

namespace MinMVC
{
	public interface IInjector
	{
		event Action onCleanUp;

		IInjector parent { set; }

		void Register<TInterface, TClass>(bool preventCaching = false);

		void Register(Type key, Type value, bool preventCaching = false);

		void Register<T>(bool preventCaching = false);

		void Register<T>(T instance);

		T Get<T>() where T: class;

		T Get<T>(Type type) where T: class;

		object Get(Type type);

		bool Has<T>();

		bool Has(Type type);

		void CleanUp();
	}
}
