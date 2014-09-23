using System;

namespace MinMVC
{
	public abstract class Mediator<T> : IMediator where T: IMediatedView
	{
		public event Action<IMediatedView> mediate;
		public event Action<IMediator> onRemove;

		protected T _view;

		public IMediatedView view {
			set {
				_view = (T)value;
			}
		}

		public virtual void OnRegister()
		{
			_view.mediate += Mediate;
			_view.onRemove += OnRemove;
		}

		public virtual void Remove()
		{
			_view.Remove();
		}

		protected virtual void OnRemove()
		{
			_view.mediate -= Mediate;
			_view.onRemove -= OnRemove;

			onRemove(this);
		}

		protected virtual void Mediate(IMediatedView view)
		{
			mediate(view);
		}
	}
}
