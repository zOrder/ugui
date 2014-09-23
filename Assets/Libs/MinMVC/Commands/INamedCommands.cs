using System;

namespace MinMVC
{
	public interface INamedCommands : ICommands
	{
		void Add<T>(string name) where T: IBaseCommand, new();

		void Add(string name, Type type);

		void Execute(string name);

		void Execute<T>(string name, T param);

		void Execute<TParam0, TParam1>(string name, TParam0 param0, TParam1 param1);

		void Remove<T>(string name);

		void Remove(string name, Type type);

		void Remove(string name);

		void Remove<T>();

		void Remove(Type type);
	}
}
