using System;
using UnityEngine;

public class SkinColorProfileEx : ScriptableObject
{
	[Serializable]
	public class GroupColors
	{
		public Color colorDefault;

		public Color colorSelected;

		public Color colorNoSelection;

		public bool spriteSwap;
	}

	public GroupColors general;

	public GroupColors tab;

	public Color deco1;
}
