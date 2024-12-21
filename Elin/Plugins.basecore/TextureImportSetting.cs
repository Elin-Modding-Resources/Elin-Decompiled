using System;
using UnityEngine;

public class TextureImportSetting : MonoBehaviour
{
	[Serializable]
	public class Data
	{
		public TextureFormat format = TextureFormat.ARGB32;

		public TextureWrapMode wrapMode;

		public FilterMode filterMode;

		public bool linear;

		public bool mipmap;

		public bool alphaIsTransparency = true;

		public bool fixTranparency;

		public int anisoLevel;

		public int mipmapBias;
	}

	public static TextureImportSetting Instance;

	public Data data;

	private void Awake()
	{
		Instance = this;
	}
}
