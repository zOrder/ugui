using System;

namespace MinMVC
{
	public abstract class BaseCommand : IBaseCommand
	{
		public event Action<IBaseCommand> onFinish;

		public bool isRetained { get; private set; }

		protected void Retain()
		{
			isRetained = true;
		}

		public virtual void Release()
		{
			isRetained = false;
			onFinish(this);
		}
	}

	public abstract class Command : BaseCommand, ICommand
	{
		public abstract void Execute();
	}

	public abstract class Command<T> : BaseCommand, ICommand<T>
	{
		public abstract void Execute(T param);
	}

	public abstract class Command<TParam0, TParam1> : BaseCommand, ICommand<TParam0, TParam1>
	{
		public abstract void Execute(TParam0 param0, TParam1 param1);
	}
}
