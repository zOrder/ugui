using System;
using System.Collections.Generic;

namespace MinMVC
{
	public class Mediators : IMediators
	{
		[Inject]
		public IInjector injector;

		readonly HashSet<IMediator> _mediators = new HashSet<IMediator>();
		readonly IDictionary<Type, HashSet<Type>> _viewMediatorsMap = new Dictionary<Type, HashSet<Type>>();

		[PostInjection]
		public void Init()
		{
			injector.onCleanUp += OnCleanUp;
		}

		void OnCleanUp()
		{
			injector.onCleanUp -= OnCleanUp;

			RemoveAll();
		}

		public void Map<TViewInterface, TMediator>() where TViewInterface : IMediatedView where TMediator : IMediator
		{
			Type viewType = typeof(TViewInterface);
			HashSet<Type> mediatorTypes;

			if (!_viewMediatorsMap.TryGetValue(viewType, out mediatorTypes)) {
				_viewMediatorsMap[viewType] = mediatorTypes = new HashSet<Type>();
			}

			injector.Register<TMediator>(true);
			Type mediatorType = typeof(TMediator);
			mediatorTypes.Add(mediatorType);
		}

		public void Mediate<T>(T view) where T : IMediatedView
		{
			Type viewType = view.GetType();
			Mediate(view, viewType);

			Type[] interfaces = viewType.GetInterfaces();

			for (int i = 0, length = interfaces.Length; i < length; i++) {
				Type viewInterface = interfaces[i];
				Mediate(view, viewInterface);
			}
		}

		void Mediate<T>(T view, Type viewType) where T : IMediatedView
		{
			HashSet<Type> mediatorTypes;

			if (_viewMediatorsMap.TryGetValue(viewType, out mediatorTypes)) {
				foreach (Type mediatorType in mediatorTypes) {
					InitMediator(mediatorType, view);
				}
			}
		}

		void InitMediator(Type mediatorType, IMediatedView view)
		{
			IMediator mediator = injector.Get<IMediator>(mediatorType);
			_mediators.Add(mediator);
			mediator.mediate += Mediate;
			mediator.onRemove += OnRemove;
			mediator.view = view;
			mediator.OnRegister();
			view.OnMediate();
		}

		void OnRemove(IMediator mediator)
		{
			mediator.mediate -= Mediate;
			mediator.onRemove -= OnRemove;
			mediator.view = null;
			_mediators.Remove(mediator);
		}

		public void RemoveAll()
		{
			IMediator[] clone = new IMediator[_mediators.Count];
			_mediators.CopyTo(clone);

			for (int i = 0, cloneLength = clone.Length; i < cloneLength; i++) {
				IMediator mediator = clone[i];
				mediator.Remove();
			}
		}
	}
}
