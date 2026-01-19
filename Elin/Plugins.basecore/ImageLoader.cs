using System.IO;
using UnityEngine;

public static class ImageLoader
{
	public static Texture2D LoadPNG(string _path, FilterMode filter = FilterMode.Point)
	{
		if (!File.Exists(_path))
		{
			return null;
		}
		byte[] array = ReadImageFile(_path);
		if (array == null || array.Length < 8)
		{
			return null;
		}
		int width;
		int height;
		if (IsPng(array))
		{
			if (!TryGetPngSize(array, out width, out height))
			{
				return null;
			}
		}
		else
		{
			if (!IsJpeg(array))
			{
				return null;
			}
			if (!TryGetJpegSize(array, out width, out height))
			{
				return null;
			}
		}
		TextureImportSetting.Data data = (TextureImportSetting.Instance ? TextureImportSetting.Instance.data : IO.importSetting);
		Texture2D texture2D = new Texture2D(width, height, data.format, data.mipmap, data.linear);
		texture2D.LoadImage(array);
		texture2D.wrapMode = data.wrapMode;
		texture2D.filterMode = filter;
		texture2D.anisoLevel = data.anisoLevel;
		texture2D.mipMapBias = data.mipmapBias;
		return texture2D;
	}

	public static byte[] ReadImageFile(string _path)
	{
		using FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		using BinaryReader binaryReader = new BinaryReader(input);
		return binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
	}

	private static bool IsPng(byte[] data)
	{
		if (data.Length >= 8 && data[0] == 137 && data[1] == 80 && data[2] == 78 && data[3] == 71 && data[4] == 13 && data[5] == 10 && data[6] == 26)
		{
			return data[7] == 10;
		}
		return false;
	}

	private static bool IsJpeg(byte[] data)
	{
		if (data.Length >= 2 && data[0] == byte.MaxValue)
		{
			return data[1] == 216;
		}
		return false;
	}

	private static bool TryGetPngSize(byte[] data, out int width, out int height)
	{
		width = (height = 0);
		if (data.Length < 24)
		{
			return false;
		}
		width = ReadIntBigEndian(data, 16);
		height = ReadIntBigEndian(data, 20);
		if (width > 0)
		{
			return height > 0;
		}
		return false;
	}

	private static bool TryGetJpegSize(byte[] data, out int width, out int height)
	{
		width = (height = 0);
		int i = 2;
		while (i + 1 < data.Length)
		{
			if (data[i] != byte.MaxValue)
			{
				i++;
				continue;
			}
			for (; i < data.Length && data[i] == byte.MaxValue; i++)
			{
			}
			if (i >= data.Length)
			{
				break;
			}
			byte b = data[i++];
			if (b == 218 || b == 217 || i + 1 >= data.Length)
			{
				break;
			}
			int num = (data[i] << 8) | data[i + 1];
			if (num < 2)
			{
				return false;
			}
			if (IsSofMarker(b))
			{
				if (i + 7 >= data.Length)
				{
					return false;
				}
				height = (data[i + 3] << 8) | data[i + 4];
				width = (data[i + 5] << 8) | data[i + 6];
				if (width > 0)
				{
					return height > 0;
				}
				return false;
			}
			i += num;
		}
		return false;
	}

	private static bool IsSofMarker(byte marker)
	{
		switch (marker)
		{
		case 192:
		case 193:
		case 194:
		case 195:
		case 197:
		case 198:
		case 199:
		case 201:
		case 202:
		case 203:
		case 205:
		case 206:
		case 207:
			return true;
		default:
			return false;
		}
	}

	private static int ReadIntBigEndian(byte[] data, int offset)
	{
		if (offset + 3 >= data.Length)
		{
			return 0;
		}
		return ((data[offset] & 0xFF) << 24) | ((data[offset + 1] & 0xFF) << 16) | ((data[offset + 2] & 0xFF) << 8) | (data[offset + 3] & 0xFF);
	}
}
