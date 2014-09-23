using System;

namespace MinMVC
{
	public interface IMediator
	{
		event Action<IMediatedView> mediate;
		event Action<IMediator> onRemove;

		IMediatedView view { set; }

		void OnRegister();

		void Remove();
	}
}
