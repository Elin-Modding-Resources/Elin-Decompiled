using System.IO;
using System.Linq;
using UnityEngine;

public static class GalleryFlattenAndRename
{
	public static string root = CorePath.CorePackage.ETC + "/Gallery";

	public static UD_Int_String dict;

	public const int ThumbMaxW = 360;

	public const int ThumbMaxH = 240;

	public static UD_Int_String Run()
	{
		dict = new UD_Int_String();
		int num = MoveAllFilesUnderSubfoldersToRoot(root);
		string[] array = Directory.GetDirectories(root, "*", SearchOption.AllDirectories).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			Directory.Delete(array[i], recursive: true);
		}
		array = Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly);
		foreach (string text in array)
		{
			if (text.EndsWith("meta"))
			{
				File.Delete(text);
			}
		}
		RenameAllFilesToLeadingNumber(root);
		array = Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly);
		foreach (string text2 in array)
		{
			if (text2.EndsWith("meta"))
			{
				File.Delete(text2);
			}
		}
		CreateThumbnailsSameFolder(root, 360, 240);
		Debug.Log("[Gallery] Done. Total:" + num + "/" + dict.Count);
		return dict;
	}

	private static int MoveAllFilesUnderSubfoldersToRoot(string root)
	{
		string[] directories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
		int num = 0;
		string[] array = directories;
		for (int i = 0; i < array.Length; i++)
		{
			string[] files = Directory.GetFiles(array[i], "*", SearchOption.TopDirectoryOnly);
			foreach (string text in files)
			{
				string text2 = Path.Combine(root, Path.GetFileName(text));
				File.Move(text, text2);
				if (!text2.EndsWith("meta"))
				{
					num++;
				}
			}
		}
		return num;
	}

	private static void RenameAllFilesToLeadingNumber(string root)
	{
		string[] files = Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly);
		foreach (string obj in files)
		{
			string extension = Path.GetExtension(obj);
			string text = Path.GetFileNameWithoutExtension(obj).Split('_')[0];
			string text2 = Path.Combine(root, text + extension);
			File.Move(obj, text2);
			dict[text.ToInt()] = text2;
		}
	}

	private static void CreateThumbnailsSameFolder(string root, int maxW, int maxH)
	{
		string[] files = Directory.GetFiles(root, "*", SearchOption.TopDirectoryOnly);
		foreach (string text in files)
		{
			if (text.EndsWith(".meta"))
			{
				continue;
			}
			string text2 = Path.GetExtension(text).ToLowerInvariant();
			if (!IsImageExt(text2) || Path.GetFileName(text).Contains(text2 + "_t"))
			{
				continue;
			}
			byte[] data = File.ReadAllBytes(text);
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false, linear: false);
			if (!texture2D.LoadImage(data))
			{
				Object.DestroyImmediate(texture2D);
				continue;
			}
			int width = texture2D.width;
			int height = texture2D.height;
			float num = Mathf.Min((float)maxW / (float)width, (float)maxH / (float)height);
			if (num > 1f)
			{
				num = 1f;
			}
			int w = Mathf.Max(1, Mathf.RoundToInt((float)width * num));
			int h = Mathf.Max(1, Mathf.RoundToInt((float)height * num));
			Texture2D texture2D2 = ResizeTo(texture2D, w, h);
			string path = text + "_t";
			byte[] bytes = ((!(text2 == ".png")) ? texture2D2.EncodeToJPG(90) : texture2D2.EncodeToPNG());
			File.WriteAllBytes(path, bytes);
			Object.DestroyImmediate(texture2D);
			Object.DestroyImmediate(texture2D2);
		}
	}

	private static bool IsImageExt(string ext)
	{
		if (!(ext == ".png") && !(ext == ".jpg"))
		{
			return ext == ".jpeg";
		}
		return true;
	}

	private static Texture2D ResizeTo(Texture2D src, int w, int h)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
		RenderTexture active = RenderTexture.active;
		Graphics.Blit(src, temporary);
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(w, h, TextureFormat.RGBA32, mipChain: false, linear: false);
		texture2D.ReadPixels(new Rect(0f, 0f, w, h), 0, 0);
		texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		return texture2D;
	}
}
