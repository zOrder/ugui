using UnityEngine;
using System;

namespace MinMVC
{
	public abstract class MediatedView : MonoBehaviour, IMediatedView
	{
		public event Action<IMediatedView> mediate;
		public event Action onRemove;

		protected virtual void Start()
		{
			Mediate(this);
		}

		public virtual void OnMediate()
		{
		}

		public virtual void Mediate(IMediatedView v)
		{
			if (mediate != null) {
				mediate(v);
			} else {
				var mediatableView = FindMediatableParent();

				if (mediatableView != null) {
					mediatableView.Mediate(v);
				}
			}
		}

		public virtual void Remove()
		{
			if (gameObject != null) {
				Destroy(gameObject);
			} else {
				OnDestroy();
			}
		}

		IMediatedView FindMediatableParent()
		{
			Type type = typeof(IMediatedView);
			Transform parent = transform.parent;
			IMediatedView mediatableView = null;

			while (parent != null) {
				mediatableView = (IMediatedView)parent.GetComponent(type);

				if (mediatableView != null) {
					break;
				}

				parent = parent.transform.parent;
			}

			return mediatableView;
		}

		protected virtual void OnDestroy()
		{
			if (onRemove != null) {
				onRemove();
			}
		}
	}
}
