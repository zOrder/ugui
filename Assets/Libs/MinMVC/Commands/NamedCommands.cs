using System;
using System.Collections.Generic;

namespace MinMVC
{
	public class NamedCommands : Commands, INamedCommands
	{
		readonly IDictionary<string,HashSet<Type>> _nameCommandMap = new Dictionary<string,HashSet<Type>>();

		public void Add<T>(string name) where T: IBaseCommand, new()
		{
			Type type = typeof(T);
			Add(name, type);
		}

		public void Add(string name, Type type)
		{
			HashSet<Type> commands;

			if (!_nameCommandMap.TryGetValue(name, out commands)) {
				_nameCommandMap[name] = commands = new HashSet<Type>();
			}

			if (!commands.Add(type)) {
				throw new Exception("could not add " + type + " to " + name + " again");
			}
		}

		public void Remove<T>(string name)
		{
			Type type = typeof(T);
			Remove(name, type);
		}

		public void Remove(string name, Type type)
		{
			HashSet<Type> list;

			if (_nameCommandMap.TryGetValue(name, out list)) {
				list.Remove(type);
			}
		}

		public void Remove(string name)
		{
			_nameCommandMap.Remove(name);
		}

		public void Remove<T>()
		{
			Type type = typeof(T);
			Remove(type);
		}

		public void Remove(Type type)
		{
			foreach (HashSet<Type> commands in _nameCommandMap.Values) {
				commands.Remove(type);
			}
		}

		public void Execute(string name)
		{
			ExecuteCommands(name, Execute);
		}

		public void Execute<T>(string name, T param)
		{
			ExecuteCommands(name, type => Execute(type, param));
		}

		public void Execute<TParam0, TParam1>(string name, TParam0 param0, TParam1 param1)
		{
			ExecuteCommands(name, type => Execute(type, param0, param1));
		}

		void ExecuteCommands(string name, Action<Type> executeCommand)
		{
			HashSet<Type> types;

			if (_nameCommandMap.TryGetValue(name, out types)) {
				foreach (Type type in types) {
					executeCommand(type);
				}
			}
		}
	}
}
