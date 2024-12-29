using System;
using UnityEngine;

public class UIListEx<T> : UIList where T : Component
{
	public class RefObjectEx : RefObject
	{
		public Action onClick;

		public Action<T> onInstantiate;

		public string text;
	}

	public T mold;

	public override void Refresh(bool highlightLast = false)
	{
		if (callbacks == null)
		{
			callbacks = new Callback<RefObjectEx, T>
			{
				onClick = delegate(RefObjectEx a, T b)
				{
					if (a.onClick != null)
					{
						a.onClick();
					}
				},
				onInstantiate = delegate(RefObjectEx a, T b)
				{
					OnInstantiate(a, b);
					if (a.onInstantiate != null)
					{
						a.onInstantiate(b);
					}
				},
				mold = mold
			};
		}
		base.Refresh();
	}

	public virtual void OnInstantiate(RefObjectEx o, T t)
	{
	}
}
