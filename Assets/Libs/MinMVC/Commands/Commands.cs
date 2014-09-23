using System;
using System.Collections.Generic;

namespace MinMVC
{
	public class Commands : ICommands
	{
		[Inject]
		public IInjector injector;

		readonly IDictionary<Type,Queue<IBaseCommand>> _cache = new Dictionary<Type,Queue<IBaseCommand>>();
		readonly HashSet<IBaseCommand> _retained = new HashSet<IBaseCommand>();

		[PostInjection]
		public void Init()
		{
			injector.onCleanUp += OnCleanUp;
		}

		void OnCleanUp()
		{
			injector.onCleanUp -= OnCleanUp;

			ReleaseCommands();
		}

		void ReleaseCommands()
		{
			IBaseCommand[] clone = new IBaseCommand[_retained.Count];
			_retained.CopyTo(clone);

			for (int i = 0, cloneLength = clone.Length; i < cloneLength; i++) {
				IBaseCommand command = clone[i];
				command.Release();
				command.onFinish -= OnFinish;
			}
		}

		public void Execute<T>()
		{
			Type type = typeof(T);
			Execute(type);
		}

		public void Execute(Type type)
		{
			ExecuteCommand(type, command => ((ICommand)command).Execute());
		}

		public void Execute<T, TParam>(TParam param)
		{
			Type type = typeof(T);
			Execute(type, param);
		}

		public void Execute<TParam>(Type type, TParam param)
		{
			ExecuteCommand(type, command => ((ICommand<TParam>)command).Execute(param));
		}

		public void Execute<T, TParam0, TParam1>(TParam0 param0, TParam1 param1)
		{
			Type type = typeof(T);
			Execute(type, param0, param1);
		}

		public void Execute<TParam0, TParam1>(Type type, TParam0 param0, TParam1 param1)
		{
			ExecuteCommand(type, command => ((ICommand<TParam0, TParam1>)command).Execute(param0, param1));
		}

		void ExecuteCommand(Type type, Action<IBaseCommand> executeCommand)
		{
			IBaseCommand command = GetCommand(type);
			executeCommand(command);

			if (command.isRetained) {
				_retained.Add(command);
			} else {
				command.Release();
			}
		}

		void OnFinish(IBaseCommand command)
		{
			Type type = command.GetType();
			Queue<IBaseCommand> cached;

			if (!_cache.TryGetValue(type, out cached)) {
				_cache[type] = cached = new Queue<IBaseCommand>();
			}

			cached.Enqueue(command);
			_retained.Remove(command);
		}

		IBaseCommand GetCommand(Type type)
		{
			Queue<IBaseCommand> cachedCommands;
			bool hasCached = _cache.TryGetValue(type, out cachedCommands) && cachedCommands.Count > 0;

			IBaseCommand command;

			if (hasCached) {
				command = cachedCommands.Dequeue();
			} else {
				if (!injector.Has(type)) {
					injector.Register(type, type, true);
				}

				command = injector.Get<IBaseCommand>(type);
				command.onFinish += OnFinish;
			}

			return command;
		}
	}
}
