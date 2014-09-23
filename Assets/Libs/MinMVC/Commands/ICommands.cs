using System;

namespace MinMVC
{
	public interface ICommands
	{
		void Execute<T>();

		void Execute(Type type);

		void Execute<T, TParam>(TParam param);

		void Execute<TParam>(Type type, TParam param);

		void Execute<T, TParam0, TParam1>(TParam0 param0, TParam1 param1);

		void Execute<TParam0, TParam1>(Type type, TParam0 param0, TParam1 param1);
	}
}
