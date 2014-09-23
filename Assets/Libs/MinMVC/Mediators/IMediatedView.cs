using System;

namespace MinMVC
{
	public interface IMediatedView
	{
		event Action<IMediatedView> mediate;
		event Action onRemove;

		void OnMediate();

		void Mediate(IMediatedView view);

		void Remove();
	}
}
