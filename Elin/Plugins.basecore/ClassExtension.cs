using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Pluralize.NET;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ClassExtension
{
	private static Vector3 vector3;

	public static IPluralize pluralizer = new Pluralizer();

	public static string lang(this string s)
	{
		return Lang.Get(s);
	}

	public static string langPlural(this string s, int i)
	{
		string text = Lang.Get(s);
		return Lang.Parse((i <= 1 || !Lang.setting.pluralize) ? text : pluralizer.Pluralize(text), i.ToString() ?? "");
	}

	public static string lang(this string s, string ref1, string ref2 = null, string ref3 = null, string ref4 = null, string ref5 = null)
	{
		return Lang.Parse(s, ref1, ref2, ref3, ref4, ref5);
	}

	public static string langGame(this string s)
	{
		return Lang.Game.Get(s);
	}

	public static string langGame(this string s, string ref1, string ref2 = null, string ref3 = null, string ref4 = null)
	{
		return Lang.Game.Parse(s, ref1, ref2, ref3, ref4);
	}

	public static string Parentheses(this string str)
	{
		return "(" + str + ")";
	}

	public static string Bracket(this string str, int type = 0)
	{
		return type switch
		{
			-1 => str, 
			1 => "「" + str + "」", 
			2 => "『" + str + "』", 
			3 => "《" + str + "》", 
			4 => "(" + str + ")", 
			_ => "_bracketLeft".lang() + str + "_bracketRight".lang(), 
		};
	}

	public static byte[] ToBytes(this BitArray bits)
	{
		byte[] array = new byte[(bits.Length - 1) / 8 + 1];
		bits.CopyTo(array, 0);
		return array;
	}

	public static bool GetBit(this byte pByte, int bitNo)
	{
		return (pByte & (1 << bitNo)) != 0;
	}

	public static byte SetBit(this byte pByte, int bitNo, bool value)
	{
		if (!value)
		{
			return Convert.ToByte(pByte & ~(1 << bitNo));
		}
		return Convert.ToByte(pByte | (1 << bitNo));
	}

	public static int ToInt3(this float a)
	{
		return (int)(a * 1000f);
	}

	public static float FromInt3(this int a)
	{
		return (float)a / 1000f;
	}

	public static int ToInt2(this float a)
	{
		return (int)(a * 100f);
	}

	public static float FromInt2(this int a)
	{
		return (float)a / 100f;
	}

	public static int Minimum(this int i)
	{
		if (i != 0)
		{
			if (i <= 0)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	public static bool IsNull(this object o)
	{
		return o == null;
	}

	public static bool IsOn(this int data, int digit)
	{
		BitArray32 bitArray = default(BitArray32);
		bitArray.SetInt(data);
		return bitArray[digit];
	}

	public static T ToEnum<T>(this int value)
	{
		return (T)Enum.ToObject(typeof(T), value);
	}

	public static T ToEnum<T>(this long value)
	{
		return (T)Enum.ToObject(typeof(T), value);
	}

	public static T ToEnum<T>(this string value, bool ignoreCase = true)
	{
		return (T)Enum.Parse(typeof(T), value, ignoreCase);
	}

	public static Type ToType(this string value)
	{
		return Type.GetType("Elona." + value + ", Assembly-CSharp");
	}

	public static T NextEnum<T>(this T src) where T : struct
	{
		return ((T[])Enum.GetValues(src.GetType())).NextItem(src);
	}

	public static T PrevEnum<T>(this T src) where T : struct
	{
		return ((T[])Enum.GetValues(src.GetType())).PrevItem(src);
	}

	public static bool Within(this int v, int v2, int range)
	{
		if (v - v2 >= range)
		{
			return v - v2 < -range;
		}
		return true;
	}

	public static bool Within(this byte v, byte v2, byte range)
	{
		if (v - v2 >= range)
		{
			return v - v2 < -range;
		}
		return true;
	}

	public static int Clamp(this int v, int min, int max, bool loop = false)
	{
		if (v < min)
		{
			v = ((!loop) ? min : max);
		}
		else if (v > max)
		{
			v = ((!loop) ? max : min);
		}
		return v;
	}

	public static int ClampMin(this int v, int min)
	{
		if (v >= min)
		{
			return v;
		}
		return min;
	}

	public static int ClampMax(this int v, int max)
	{
		if (v <= max)
		{
			return v;
		}
		return max;
	}

	public static T ToField<T>(this string s, object o)
	{
		return (T)o.GetType().GetField(s).GetValue(o);
	}

	public static bool HasField<T>(this object o, string name)
	{
		FieldInfo field = o.GetType().GetField(name);
		if (field != null)
		{
			return typeof(T).IsAssignableFrom(field.FieldType);
		}
		return false;
	}

	public static T GetField<T>(this object o, string name)
	{
		return (T)o.GetType().GetField(name).GetValue(o);
	}

	public static void SetField<T>(this object o, string name, T value)
	{
		o.GetType().GetField(name).SetValue(o, value);
	}

	public static T GetProperty<T>(this object o, string name)
	{
		return (T)o.GetType().GetProperty(name).GetValue(o);
	}

	public static void SetProperty<T>(this object o, string name, T value)
	{
		o.GetType().GetProperty(name).SetValue(o, value);
	}

	public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
	{
		if (list is List<T>)
		{
			((List<T>)list).Sort(comparison);
			return;
		}
		List<T> list2 = new List<T>(list);
		list2.Sort(comparison);
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = list2[i];
		}
	}

	public static TValue TryGet<TValue>(this IList<TValue> array, int index, int defIndex = -1)
	{
		if (array.Count != 0)
		{
			if (index >= array.Count)
			{
				if (defIndex != -1)
				{
					if (array.Count <= defIndex)
					{
						return array[Mathf.Max(0, array.Count - 1)];
					}
					return array[defIndex];
				}
				return array[Mathf.Max(0, array.Count - 1)];
			}
			return array[Math.Max(0, index)];
		}
		return default(TValue);
	}

	public static TValue TryGet<TValue>(this IList<TValue> array, int index, bool returnNull)
	{
		if (index >= array.Count)
		{
			return default(TValue);
		}
		return array[index];
	}

	public static bool IsEmpty(this Array array)
	{
		if (array != null)
		{
			return array.Length == 0;
		}
		return true;
	}

	public static int IndexOf(this IList<Component> list, GameObject go)
	{
		if (!go)
		{
			return -1;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gameObject == go)
			{
				return i;
			}
		}
		return -1;
	}

	public static TValue Move<TValue>(this IList<TValue> list, TValue target, int a)
	{
		int num = list.IndexOf(target);
		int num2 = num + a;
		if (num2 < 0)
		{
			num2 = list.Count - 1;
		}
		if (num2 >= list.Count)
		{
			num2 = 0;
		}
		list.Remove(target);
		list.Insert(num2, target);
		return list[num];
	}

	public static IList<TValue> Copy<TValue>(this IList<TValue> list)
	{
		return list.ToList();
	}

	public static void Each<TValue>(this IList<TValue> list, Action<TValue> action)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			action(list[num]);
		}
	}

	public static IList<TValue> Shuffle<TValue>(this IList<TValue> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			int index = Rand._random.Next(num);
			num--;
			TValue value = list[num];
			list[num] = list[index];
			list[index] = value;
		}
		return list;
	}

	public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue fallback = default(TValue))
	{
		TValue value = default(TValue);
		if (key != null && source.TryGetValue(key, out value))
		{
			return value;
		}
		return fallback;
	}

	public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TKey key_fallback)
	{
		TValue value = default(TValue);
		if (key != null && source.TryGetValue(key, out value))
		{
			return value;
		}
		return source[key_fallback];
	}

	public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> func = null)
	{
		TValue val = dict.TryGetValue(key);
		if (val == null)
		{
			val = ((func != null) ? func() : Activator.CreateInstance<TValue>());
			dict.Add(key, val);
		}
		return val;
	}

	public static TKey[] CopyKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict)
	{
		TKey[] array = new TKey[dict.Keys.Count];
		dict.Keys.CopyTo(array, 0);
		return array;
	}

	public static TValue[] CopyValues<TKey, TValue>(this IDictionary<TKey, TValue> dict)
	{
		TValue[] array = new TValue[dict.Keys.Count];
		dict.Values.CopyTo(array, 0);
		return array;
	}

	public static T RandomItem<T>(this IEnumerable<T> ie)
	{
		int num = ie.Count();
		if (num == 0)
		{
			return default(T);
		}
		return ie.ElementAt(Rand.rnd(num));
	}

	public static TValue RandomItem<TKey, TValue>(this IDictionary<TKey, TValue> source)
	{
		if (source.Count != 0)
		{
			return source.ElementAt(Rand.rnd(source.Count)).Value;
		}
		return default(TValue);
	}

	public static TValue RandomItem<TValue>(this IList<TValue> source)
	{
		if (source.Count != 0)
		{
			return source[Rand.rnd(source.Count)];
		}
		return default(TValue);
	}

	public static TValue RandomItem<TValue>(this IList<TValue> source, TValue exclude)
	{
		return source.RandomItem(source.IndexOf(exclude));
	}

	public static TValue RandomItem<TValue>(this IList<TValue> source, int exclude)
	{
		if (source.Count > 1)
		{
			int num;
			do
			{
				num = Rand.rnd(source.Count);
			}
			while (num == exclude);
			return source[num];
		}
		if (source.Count == 1)
		{
			return source[0];
		}
		return default(TValue);
	}

	public static TValue RandomItem<TValue>(this IList<TValue> source, Func<TValue, bool> funcValid, TValue defaultValue = default(TValue))
	{
		for (int i = 0; i < 100; i++)
		{
			TValue val = source[Rand.rnd(source.Count)];
			if (funcValid(val))
			{
				return val;
			}
		}
		return defaultValue;
	}

	public static TValue RandomItemWeighted<TValue>(this IList<TValue> source, Func<TValue, float> getWeight)
	{
		if (source.Count == 0)
		{
			return default(TValue);
		}
		if (source.Count == 1)
		{
			return source[0];
		}
		float num = 0f;
		foreach (TValue item in source)
		{
			num += getWeight(item);
		}
		float num2 = Rand.Range(0f, num);
		num = 0f;
		foreach (TValue item2 in source)
		{
			num += getWeight(item2);
			if (num2 < num)
			{
				return item2;
			}
		}
		return source.First();
	}

	public static TValue Remainder<TValue>(this IList<TValue> source, int divider)
	{
		if (source.Count != 0)
		{
			return source[divider % source.Count];
		}
		return default(TValue);
	}

	public static TValue FirstItem<TKey, TValue>(this IDictionary<TKey, TValue> source)
	{
		if (source == null)
		{
			return default(TValue);
		}
		return source[source.First().Key];
	}

	public static TValue LastItem<TValue>(this IList<TValue> source)
	{
		if (source.Count != 0)
		{
			return source[source.Count - 1];
		}
		return default(TValue);
	}

	public static TValue NextItem<TValue>(this IList<TValue> source, ref int index)
	{
		index++;
		if (index >= source.Count)
		{
			index = 0;
		}
		if (source.Count != 0)
		{
			return source[index];
		}
		return default(TValue);
	}

	public static int NextIndex<TValue>(this IList<TValue> source, TValue val)
	{
		if (val == null)
		{
			return 0;
		}
		int num = source.IndexOf(val) + 1;
		if (num >= source.Count)
		{
			num = 0;
		}
		return num;
	}

	public static TValue NextItem<TValue>(this IList<TValue> source, TValue val)
	{
		int num = source.IndexOf(val) + 1;
		if (num >= source.Count)
		{
			num = 0;
		}
		if (source.Count != 0)
		{
			return source[num];
		}
		return default(TValue);
	}

	public static TValue PrevItem<TValue>(this IList<TValue> source, TValue val)
	{
		int num = source.IndexOf(val) - 1;
		if (num < 0)
		{
			num = source.Count - 1;
		}
		if (source.Count != 0)
		{
			return source[num];
		}
		return default(TValue);
	}

	public static TValue Clamp<TValue>(this IList<TValue> source, int i)
	{
		if (i < 0)
		{
			i = 0;
		}
		else if (i >= source.Count)
		{
			i = source.Count - 1;
		}
		return source[i];
	}

	public static List<T> GetList<T>(this IDictionary source)
	{
		List<T> list = new List<T>();
		foreach (object value in source.Values)
		{
			if (value is T)
			{
				list.Add((T)value);
			}
		}
		return list;
	}

	public static void ToDictionary<T>(this IList<UPair<T>> list, ref Dictionary<string, T> dic)
	{
		dic = new Dictionary<string, T>();
		for (int i = 0; i < list.Count; i++)
		{
			dic.Add(list[i].name, list[i].value);
		}
	}

	public static T FindMax<T>(this List<T> list, Func<T, int> func)
	{
		int num = int.MinValue;
		T result = default(T);
		foreach (T item in list)
		{
			int num2 = func(item);
			if (num2 > num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}

	public static void Set<T1, T2>(this Dictionary<T1, T2> dic, Dictionary<T1, T2> from)
	{
		dic.Clear();
		foreach (KeyValuePair<T1, T2> item in from)
		{
			dic[item.Key] = item.Value;
		}
	}

	public static int Calc(this string str, int power = 0, int ele = 0, int p2 = 0)
	{
		return Cal.Calcuate(str.Replace("p2", p2.ToString() ?? "").Replace("p", power.ToString() ?? "").Replace("e", ele.ToString() ?? ""));
	}

	public static int ToInt<T>(this string str)
	{
		return str.ToInt(typeof(T));
	}

	public static int ToInt(this string str, Type type)
	{
		FieldInfo field = type.GetField(str, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		if (field == null)
		{
			Debug.LogError("Field is null:" + str + "/" + type);
			return -1;
		}
		return (int)field.GetValue(null);
	}

	public static int ToInt(this string str)
	{
		if (int.TryParse(str, out var result))
		{
			return result;
		}
		return 0;
	}

	public static float ToFloat(this string str)
	{
		float result = 0f;
		try
		{
			if (!float.TryParse(str, out result))
			{
				Debug.Log("exception: ToFlat1" + str);
				result = 1f;
			}
		}
		catch
		{
			Debug.Log("exception: ToFlat2" + str);
			result = 1f;
		}
		return result;
	}

	public static string StripLastPun(this string str)
	{
		if (str != null)
		{
			return str.TrimEnd(Lang.words.period);
		}
		return str;
	}

	public static string StripPun(this string str, bool stripComma, bool insertSpaceForComma = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in str)
		{
			if (c == Lang.words.comma)
			{
				if (stripComma)
				{
					if (insertSpaceForComma)
					{
						stringBuilder.Append('\u3000');
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			else if (c != Lang.words.period)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public static string Repeat(this string str, int count)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Mathf.Abs(count); i++)
		{
			stringBuilder.Append(str);
		}
		return stringBuilder.ToString();
	}

	public static string StripBrackets(this string str)
	{
		return str.Replace("\"", "").Replace("「", "").Replace("」", "")
			.Replace("“", "")
			.Replace("\"", "");
	}

	public static string TryAddExtension(this string s, string ext)
	{
		if (!s.Contains("." + ext))
		{
			return s + "." + ext;
		}
		return s;
	}

	public static bool IsEmpty(this string str)
	{
		if (str != null)
		{
			return str == "";
		}
		return true;
	}

	public static string IsEmpty(this string str, string defaultStr)
	{
		if (str != null && !(str == ""))
		{
			return str;
		}
		return defaultStr;
	}

	public static string TrimNewLines(this string text)
	{
		while (text.EndsWith(Environment.NewLine))
		{
			text = text.Substring(0, text.Length - Environment.NewLine.Length);
		}
		return text;
	}

	public static int[] SplitToInts(this string str, char separator)
	{
		string[] array = str.Split(separator);
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = int.Parse(array[i]);
		}
		return array2;
	}

	public static string[] SplitNewline(this string str)
	{
		return str.Split(Environment.NewLine.ToCharArray());
	}

	public static bool Contains(this string[] strs, string id)
	{
		if (strs == null || strs.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < strs.Length; i++)
		{
			if (strs[i] == id)
			{
				return true;
			}
		}
		return false;
	}

	public static string Evalute(this string[] array, float val)
	{
		return array[(int)Mathf.Clamp((float)(array.Length - 1) * val, 0f, array.Length - 1)];
	}

	public static string ToShortNumber(this int a)
	{
		string text;
		if (a < 1000000)
		{
			if (a >= 1000)
			{
				return a / 1000 + "K";
			}
			text = a.ToString();
			if (text == null)
			{
				return "";
			}
		}
		else
		{
			text = a / 1000000 + "M";
		}
		return text;
	}

	public static string ToFormat(this int a)
	{
		return $"{a:#,0}";
	}

	public static string ToFormat(this long a)
	{
		return $"{a:#,0}";
	}

	public static string ToText(this int a, bool skipIfZero = true)
	{
		object obj;
		if (!(a == 0 && skipIfZero))
		{
			if (a >= 0)
			{
				return "+" + a;
			}
			obj = a.ToString();
			if (obj == null)
			{
				return "";
			}
		}
		else
		{
			obj = "";
		}
		return (string)obj;
	}

	public static string TagColor(this string s, Color c, string txt)
	{
		return s + "<color=" + c.ToHex() + ">" + txt + "</color>";
	}

	public static string TagColor(this string s, Color c)
	{
		return "<color=" + c.ToHex() + ">" + s + "</color>";
	}

	public static string TagSize(this string s, string txt, int size)
	{
		return s + "<size=" + size + ">" + txt + "</size>";
	}

	public static string TagSize(this string s, int size)
	{
		return "<size=" + size + ">" + s + "</size>";
	}

	public static string GetFullFileNameWithoutExtension(this FileInfo fileInfo)
	{
		return fileInfo.Directory.FullName + "/" + Path.GetFileNameWithoutExtension(fileInfo.Name);
	}

	public static string GetFullFileNameWithoutExtension(this string path)
	{
		return new FileInfo(path).GetFullFileNameWithoutExtension();
	}

	public static string AddArticle(this string s)
	{
		if (!Lang.setting.addArticle || s.Length < 1)
		{
			return s;
		}
		char c = s[0];
		s = ((c == 'a' || c == 'i' || c == 'u' || c == 'e' || c == 'o') ? "an " : "a ") + s;
		return s;
	}

	public static string AddArticle(this string s, int num, ArticleStyle style = ArticleStyle.Default, string replace = null)
	{
		if (!Lang.setting.addArticle || s.Length < 1)
		{
			return s;
		}
		char c = s[0];
		string text = ((num >= 2) ? (num.ToFormat() + " ") : ((c == 'a' || c == 'i' || c == 'u' || c == 'e' || c == 'o') ? "an " : "a "));
		if (num >= 2 && Lang.setting.pluralize)
		{
			s = ((replace.IsEmpty() || !s.Contains(replace) || s.Contains("limestone stone")) ? pluralizer.Pluralize(s) : s.Replace(replace, pluralizer.Pluralize(replace)));
		}
		return style switch
		{
			ArticleStyle.The => "_the".lang().ToTitleCase() + " " + ((num > 1) ? (num + " ") : "") + s, 
			ArticleStyle.None => ((num > 1) ? (num.ToFormat() + " ") : "") + s.ToTitleCase(), 
			_ => text + s, 
		};
	}

	public static string ToTitleCase(this string s, bool wholeText = false)
	{
		if (!Lang.setting.capitalize)
		{
			return s;
		}
		char[] array = s.ToCharArray();
		bool flag = true;
		for (int i = 0; i < array.Length; i++)
		{
			if (flag)
			{
				array[i] = char.ToUpper(array[i]);
				flag = false;
			}
			if (!wholeText)
			{
				break;
			}
			if (array[i] == ' ')
			{
				flag = true;
			}
		}
		return new string(array);
	}

	public static void LoopTail<T>(this List<T> list, bool vertical = false) where T : Selectable
	{
		if (list.Count > 1)
		{
			Navigation navigation = list[0].navigation;
			Navigation navigation2 = list[list.Count - 1].navigation;
			if (vertical)
			{
				navigation.selectOnUp = list[list.Count - 1];
				navigation2.selectOnDown = list[0];
			}
			else
			{
				navigation.selectOnLeft = list[list.Count - 1];
				navigation2.selectOnRight = list[0];
			}
			list[0].navigation = navigation;
			list[list.Count - 1].navigation = navigation2;
		}
	}

	public static void SetNavigation(this Component a, Component b, bool vertical = false)
	{
		if (!a || !b)
		{
			return;
		}
		Selectable component = a.GetComponent<Selectable>();
		Selectable component2 = b.GetComponent<Selectable>();
		if ((bool)component && (bool)component2)
		{
			Navigation navigation = component.navigation;
			Navigation navigation2 = component2.navigation;
			if (vertical)
			{
				navigation.selectOnUp = component2;
				navigation2.selectOnDown = component;
			}
			else
			{
				navigation.selectOnLeft = component2;
				navigation2.selectOnRight = component;
			}
			component.navigation = navigation;
			component2.navigation = navigation2;
		}
	}

	public static void LoopSelectable(this List<Selectable> sels, bool vertical = true, bool horizonal = true, bool asGroup = true)
	{
		for (int i = 0; i < sels.Count; i++)
		{
			Selectable selectable = sels[i];
			Selectable selectable2 = sels[0];
			Navigation navigation = selectable.navigation;
			if (horizonal)
			{
				navigation.selectOnRight = ((i + 1 < sels.Count) ? sels[i + 1] : selectable2);
			}
			if (asGroup)
			{
				for (int j = i + 1; j < sels.Count; j++)
				{
					if (sels[j].transform.position.y < selectable.transform.position.y)
					{
						selectable2 = sels[j];
						break;
					}
				}
			}
			else
			{
				selectable2 = ((i + 1 < sels.Count) ? sels[i + 1] : selectable2);
			}
			if (vertical)
			{
				navigation.selectOnDown = selectable2;
			}
			selectable.navigation = navigation;
		}
		for (int num = sels.Count - 1; num >= 0; num--)
		{
			Selectable selectable3 = sels[num];
			Selectable selectable4 = sels[sels.Count - 1];
			Navigation navigation2 = selectable3.navigation;
			if (horizonal)
			{
				navigation2.selectOnLeft = ((num > 0) ? sels[num - 1] : selectable4);
			}
			if (asGroup)
			{
				int num2 = sels.Count - 1;
				for (int num3 = num - 1; num3 >= 0; num3--)
				{
					if (sels[num3].transform.position.y > selectable3.transform.position.y)
					{
						num2 = num3;
						break;
					}
				}
				int num4 = num2;
				while (num4 >= 0 && !(sels[num4].transform.position.y > sels[num2].transform.position.y))
				{
					selectable4 = sels[num4];
					num4--;
				}
			}
			else
			{
				selectable4 = ((num > 0) ? sels[num - 1] : selectable4);
			}
			if (vertical)
			{
				navigation2.selectOnUp = ((selectable4 == selectable3) ? sels[sels.Count - 1] : selectable4);
			}
			navigation2.mode = Navigation.Mode.Explicit;
			selectable3.navigation = navigation2;
		}
	}

	public static void LoopSelectable(this Transform l, bool vertical = true, bool horizonal = true, bool asGroup = true)
	{
		List<Selectable> list = l.GetComponentsInChildren<Selectable>().ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!list[num].interactable || list[num].navigation.mode == Navigation.Mode.None)
			{
				list.RemoveAt(num);
			}
		}
		list.LoopSelectable(vertical, horizonal, asGroup);
	}

	public static Color ToColor(this string s)
	{
		Color color = Color.white;
		ColorUtility.TryParseHtmlString("#" + s, out color);
		return color;
	}

	public static void SetAlpha(this Image c, float a)
	{
		c.color = new Color(c.color.r, c.color.g, c.color.b, a);
	}

	public static void SetAlpha(this RawImage c, float a)
	{
		c.color = new Color(c.color.r, c.color.g, c.color.b, a);
	}

	public static Color SetAlpha(this Color c, float a)
	{
		c.a = a;
		return c;
	}

	public static Color Multiply(this Color c, float mtp, float add)
	{
		return new Color(c.r * mtp + add, c.g * mtp + add, c.b * mtp + add, c.a);
	}

	public static string Tag(this Color c)
	{
		return "<color=" + $"#{(int)(c.r * 255f):X2}{(int)(c.g * 255f):X2}{(int)(c.b * 255f):X2}" + ">";
	}

	public static string ToHex(this Color c)
	{
		return $"#{(int)(c.r * 255f):X2}{(int)(c.g * 255f):X2}{(int)(c.b * 255f):X2}";
	}

	public static void SetOnClick(this Button b, Action action)
	{
		b.onClick.RemoveAllListeners();
		b.onClick.AddListener(delegate
		{
			action();
		});
	}

	public static void SetListener(this Button.ButtonClickedEvent e, Action action)
	{
		e.RemoveAllListeners();
		e.AddListener(delegate
		{
			action();
		});
	}

	public static void SetAlpha(this Text text, float aloha = 1f)
	{
		text.color = new Color(text.color.r, text.color.g, text.color.b, aloha);
	}

	public static void SetSlider(this Slider slider, float value, Func<float, string> action, bool notify)
	{
		slider.SetSlider(value, action, -1, -1, notify);
	}

	public static void SetSlider(this Slider slider, float value, Func<float, string> action, int min = -1, int max = -1, bool notify = true)
	{
		slider.onValueChanged.RemoveAllListeners();
		slider.onValueChanged.AddListener(delegate(float a)
		{
			slider.GetComponentInChildren<Text>(includeInactive: true).text = action(a);
		});
		if (min != -1)
		{
			slider.minValue = min;
			slider.maxValue = max;
		}
		if (notify)
		{
			slider.value = value;
		}
		else
		{
			slider.SetValueWithoutNotify(value);
		}
		slider.GetComponentInChildren<Text>(includeInactive: true).text = action(value);
	}

	public static T GetOrCreate<T>(this Component t) where T : Component
	{
		T val = t.gameObject.GetComponent<T>();
		if (!val)
		{
			val = t.gameObject.AddComponent<T>();
		}
		return val;
	}

	public static RectTransform Rect(this Component c)
	{
		return c.transform as RectTransform;
	}

	public static void CopyRect(this RectTransform r, RectTransform t)
	{
		r.pivot = t.pivot;
		r.sizeDelta = t.sizeDelta;
		r.anchorMin = t.anchorMin;
		r.anchorMax = t.anchorMax;
		r.anchoredPosition = t.anchoredPosition;
	}

	public static void ToggleActive(this Component c)
	{
		c.SetActive(!c.gameObject.activeSelf);
	}

	public static void SetActive(this Component c, bool enable)
	{
		if (c.gameObject.activeSelf != enable)
		{
			c.gameObject.SetActive(enable);
		}
	}

	public static void SetActive(this Component c, bool enable, Action<bool> onChangeState)
	{
		if (c.gameObject.activeSelf != enable)
		{
			c.gameObject.SetActive(enable);
			onChangeState(enable);
		}
	}

	public static Selectable GetSelectable(this GameObject go)
	{
		return go.GetComponentInChildren<Selectable>();
	}

	public static void SetLayerRecursively(this GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform item in obj.transform)
		{
			item.gameObject.SetLayerRecursively(layer);
		}
	}

	public static bool IsInteractable(this GameObject obj)
	{
		Selectable selectable = (obj ? obj.GetComponent<Selectable>() : null);
		if ((bool)selectable)
		{
			return selectable.interactable;
		}
		return false;
	}

	public static bool IsChildOf(this GameObject c, GameObject root)
	{
		if (c.transform == root.transform)
		{
			return true;
		}
		if ((bool)c.transform.parent)
		{
			return c.transform.parent.gameObject.IsChildOf(root);
		}
		return false;
	}

	public static bool IsPrefab(this Component c)
	{
		return c.gameObject.scene.name == null;
	}

	public static T CreateMold<T>(this Component c, string name = null) where T : Component
	{
		T result = null;
		for (int i = 0; i < c.transform.childCount; i++)
		{
			T component = c.transform.GetChild(i).GetComponent<T>();
			if ((bool)component && (name.IsEmpty() || name == component.name))
			{
				component.gameObject.SetActive(value: false);
				result = component;
				break;
			}
		}
		c.DestroyChildren();
		return result;
	}

	public static Transform Find(this Component c, string name = null)
	{
		return c.Find<Transform>(name);
	}

	public static T Find<T>(this Component c, string name = null, bool recursive = false) where T : Component
	{
		if (recursive)
		{
			T[] componentsInChildren = c.transform.GetComponentsInChildren<T>();
			foreach (T val in componentsInChildren)
			{
				if (name == null || name == val.name)
				{
					return val;
				}
			}
			return null;
		}
		for (int j = 0; j < c.transform.childCount; j++)
		{
			T component = c.transform.GetChild(j).GetComponent<T>();
			if ((bool)component && (name == null || name == component.name))
			{
				return component;
			}
		}
		return null;
	}

	public static GameObject FindTagInParents(this Component c, string tag, bool includeInactive = true)
	{
		Transform transform = c.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if ((includeInactive || child.gameObject.activeSelf) && child.tag == tag)
			{
				return child.gameObject;
			}
		}
		if ((bool)transform.parent)
		{
			return transform.parent.FindTagInParents(tag, includeInactive);
		}
		return null;
	}

	public static T GetComponentInChildrenExcludSelf<T>(this Transform c) where T : Component
	{
		for (int i = 0; i < c.childCount; i++)
		{
			T component = c.GetChild(i).GetComponent<T>();
			if ((bool)component)
			{
				return component;
			}
		}
		return null;
	}

	public static List<T> GetComponentsInDirectChildren<T>(this Transform comp, bool includeInactive = true) where T : Component
	{
		List<T> list = new List<T>();
		Transform transform = comp.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			T component = transform.GetChild(i).GetComponent<T>();
			if ((bool)component && (includeInactive || component.gameObject.activeInHierarchy))
			{
				list.Add(component);
			}
		}
		return list;
	}

	public static List<T> GetComponentsInDirectChildren<T>(this Component comp, bool includeInactive = true) where T : Component
	{
		List<T> list = new List<T>();
		Transform transform = comp.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			T component = transform.GetChild(i).GetComponent<T>();
			if ((bool)component && (includeInactive || component.gameObject.activeInHierarchy))
			{
				list.Add(component);
			}
		}
		return list;
	}

	public static T GetComponentInDirectChildren<T>(this Component comp) where T : Component
	{
		Transform transform = comp.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			T component = transform.GetChild(i).GetComponent<T>();
			if ((bool)component)
			{
				return component;
			}
		}
		return null;
	}

	public static void DestroyChildren(this Component component, bool destroyInactive = false, bool ignoreDestroy = true)
	{
		if (!component || !component.transform)
		{
			Debug.LogWarning("DestroyChlidren:" + component);
			return;
		}
		for (int num = component.transform.childCount - 1; num >= 0; num--)
		{
			GameObject gameObject = component.transform.GetChild(num).gameObject;
			if ((!ignoreDestroy || !(gameObject.tag == "IgnoreDestroy")) && (destroyInactive || gameObject.activeSelf))
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}
	}

	public static bool IsMouseOver(this RectTransform r, Camera cam)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(r, Input.mousePosition, cam);
	}

	public static void RebuildLayout(this Component c, bool recursive = false)
	{
		if (recursive)
		{
			foreach (Transform item in c.transform)
			{
				if (item is RectTransform)
				{
					item.RebuildLayout(recursive: true);
				}
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(c.transform as RectTransform);
	}

	public static void RebuildLayoutTo<T>(this Component c) where T : Component
	{
		c.RebuildLayout();
		if ((bool)c.transform.parent && !c.transform.parent.GetComponent<T>())
		{
			c.transform.parent.RebuildLayoutTo<T>();
		}
	}

	public static void RebuildLayoutTo(this Component c, Component target)
	{
		c.RebuildLayout();
		if (!(c == target) && (bool)c.transform.parent)
		{
			c.transform.parent.RebuildLayoutTo(target);
		}
	}

	public static void SetRect(this RectTransform r, float x, float y, float w, float h, float pivotX, float pivotY, float minX, float minY, float maxX, float maxY)
	{
		r.SetPivot(pivotX, pivotY);
		r.SetAnchor(minX, minY, maxX, maxY);
		if (w != -1f)
		{
			r.sizeDelta = new Vector2(w, h);
		}
		r.anchoredPosition = new Vector2((x == -1f) ? r.anchoredPosition.x : x, (y == -1f) ? r.anchoredPosition.y : y);
	}

	public static void SetPivot(this RectTransform r, float x, float y)
	{
		r.pivot = new Vector2(x, y);
	}

	public static void SetAnchor(this RectTransform r, float minX, float minY, float maxX, float maxY)
	{
		r.anchorMin = new Vector2(minX, minY);
		r.anchorMax = new Vector2(maxX, maxY);
	}

	public static void _SetAnchor(this RectTransform _rect, RectPosition anchor)
	{
		switch (anchor)
		{
		case RectPosition.TopCenter:
			_rect.SetAnchor(0.5f, 1f, 0.5f, 1f);
			break;
		case RectPosition.BottomCenter:
			_rect.SetAnchor(0.5f, 0f, 0.5f, 0f);
			break;
		case RectPosition.TopLEFT:
			_rect.SetAnchor(0f, 1f, 0f, 1f);
			break;
		case RectPosition.TopRIGHT:
			_rect.SetAnchor(1f, 1f, 1f, 1f);
			break;
		case RectPosition.BottomLEFT:
			_rect.SetAnchor(0f, 0f, 0f, 0f);
			break;
		case RectPosition.BottomRIGHT:
			_rect.SetAnchor(1f, 0f, 1f, 0f);
			break;
		case RectPosition.Left:
			_rect.SetAnchor(0f, 0.5f, 0f, 0.5f);
			break;
		case RectPosition.Right:
			_rect.SetAnchor(1f, 0.5f, 1f, 0.5f);
			break;
		default:
			_rect.SetAnchor(0.5f, 0.5f, 0.5f, 0.5f);
			break;
		}
	}

	public static void SetAnchor(this RectTransform _rect, RectPosition anchor = RectPosition.Auto)
	{
		Vector3 position = _rect.position;
		_rect._SetAnchor((anchor == RectPosition.Auto) ? _rect.GetAnchor() : anchor);
		_rect.position = position;
	}

	public static RectPosition GetAnchor(this RectTransform _rect)
	{
		Vector3 position = _rect.position;
		int width = Screen.width;
		int height = Screen.height;
		bool flag = position.y > (float)height * 0.5f;
		if (position.x > (float)width * 0.3f && position.x < (float)width * 0.7f)
		{
			if (position.y > (float)height * 0.3f && position.y < (float)height * 0.7f)
			{
				return RectPosition.Center;
			}
			if (!flag)
			{
				return RectPosition.BottomCenter;
			}
			return RectPosition.TopCenter;
		}
		if (position.x < (float)width * 0.5f)
		{
			if (!flag)
			{
				return RectPosition.BottomLEFT;
			}
			return RectPosition.TopLEFT;
		}
		if (!flag)
		{
			return RectPosition.BottomRIGHT;
		}
		return RectPosition.TopRIGHT;
	}

	public static Transform GetLastChild(this Transform trans)
	{
		return trans.GetChild(trans.childCount - 1);
	}

	public static Vector2 ToVector2(this Vector3 self)
	{
		return new Vector2(self.x, self.z);
	}

	public static void SetScale(this SpriteRenderer renderer, float size)
	{
		float x = renderer.bounds.size.x;
		float y = renderer.bounds.size.y;
		float x2 = size / x;
		float y2 = size / y;
		renderer.transform.localScale = new Vector3(x2, y2, 1f);
	}

	private static float GetForwardDiffPoint(Vector2 forward)
	{
		if (object.Equals(forward, Vector2.up))
		{
			return 90f;
		}
		object.Equals(forward, Vector2.right);
		return 0f;
	}

	public static void LookAt2D(this Transform self, Transform target, Vector2 forward)
	{
		self.LookAt2D(target.position, forward);
	}

	public static void LookAt2D(this Transform self, Vector3 target, Vector2 forward)
	{
		float forwardDiffPoint = GetForwardDiffPoint(forward);
		Vector3 vector = target - self.position;
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		self.rotation = Quaternion.AngleAxis(num - forwardDiffPoint, Vector3.forward);
	}

	public static float Distance(this Transform t, Vector2 pos)
	{
		return Vector3.Distance(t.position, pos);
	}

	public static Vector3 Plus(this Vector3 v, Vector2 v2)
	{
		v.x += v2.x;
		v.y += v2.y;
		return v;
	}

	public static Vector3 SetZ(this Vector3 v, float a)
	{
		v.z = a;
		return v;
	}

	public static Vector3 SetY(this Vector3 v, float a)
	{
		v.y = a;
		return v;
	}

	public static Vector3 PlusX(this Vector3 v, float a)
	{
		v.x += a;
		return v;
	}

	public static Vector3 PlusY(this Vector3 v, float a)
	{
		v.y += a;
		return v;
	}

	public static Vector3 PlusZ(this Vector3 v, float a)
	{
		v.z += a;
		return v;
	}

	public static void SetPosition(this Transform transform, float x, float y, float z)
	{
		vector3.Set(x, y, z);
		transform.position = vector3;
	}

	public static void SetPositionX(this Transform transform, float x)
	{
		vector3.Set(x, transform.position.y, transform.position.z);
		transform.position = vector3;
	}

	public static void SetPositionY(this Transform transform, float y)
	{
		vector3.Set(transform.position.x, y, transform.position.z);
		transform.position = vector3;
	}

	public static void SetPositionZ(this Transform transform, float z)
	{
		vector3.Set(transform.position.x, transform.position.y, z);
		transform.position = vector3;
	}

	public static void AddPosition(this Transform transform, float x, float y, float z)
	{
		vector3.Set(transform.position.x + x, transform.position.y + y, transform.position.z + z);
		transform.position = vector3;
	}

	public static void AddPositionX(this Transform transform, float x)
	{
		vector3.Set(transform.position.x + x, transform.position.y, transform.position.z);
		transform.position = vector3;
	}

	public static void AddPositionY(this Transform transform, float y)
	{
		vector3.Set(transform.position.x, transform.position.y + y, transform.position.z);
		transform.position = vector3;
	}

	public static void AddPositionZ(this Transform transform, float z)
	{
		vector3.Set(transform.position.x, transform.position.y, transform.position.z + z);
		transform.position = vector3;
	}

	public static void SetLocalPosition(this Transform transform, float x, float y, float z)
	{
		vector3.Set(x, y, z);
		transform.localPosition = vector3;
	}

	public static void SetLocalPositionX(this Transform transform, float x)
	{
		vector3.Set(x, transform.localPosition.y, transform.localPosition.z);
		transform.localPosition = vector3;
	}

	public static void SetLocalPositionY(this Transform transform, float y)
	{
		vector3.Set(transform.localPosition.x, y, transform.localPosition.z);
		transform.localPosition = vector3;
	}

	public static void SetLocalPositionZ(this Transform transform, float z)
	{
		vector3.Set(transform.localPosition.x, transform.localPosition.y, z);
		transform.localPosition = vector3;
	}

	public static void AddLocalPosition(this Transform transform, float x, float y, float z)
	{
		vector3.Set(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z + z);
		transform.localPosition = vector3;
	}

	public static void AddLocalPositionX(this Transform transform, float x)
	{
		vector3.Set(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z);
		transform.localPosition = vector3;
	}

	public static void AddLocalPositionY(this Transform transform, float y)
	{
		vector3.Set(transform.localPosition.x, transform.localPosition.y + y, transform.localPosition.z);
		transform.localPosition = vector3;
	}

	public static void AddLocalPositionZ(this Transform transform, float z)
	{
		vector3.Set(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + z);
		transform.localPosition = vector3;
	}

	public static void SetLocalScale(this Transform transform, float x, float y, float z)
	{
		vector3.Set(x, y, z);
		transform.localScale = vector3;
	}

	public static void SetLocalScaleX(this Transform transform, float x)
	{
		vector3.Set(x, transform.localScale.y, transform.localScale.z);
		transform.localScale = vector3;
	}

	public static void SetLocalScaleY(this Transform transform, float y)
	{
		vector3.Set(transform.localScale.x, y, transform.localScale.z);
		transform.localScale = vector3;
	}

	public static void SetLocalScaleZ(this Transform transform, float z)
	{
		vector3.Set(transform.localScale.x, transform.localScale.y, z);
		transform.localScale = vector3;
	}

	public static void AddLocalScale(this Transform transform, float x, float y, float z)
	{
		vector3.Set(transform.localScale.x + x, transform.localScale.y + y, transform.localScale.z + z);
		transform.localScale = vector3;
	}

	public static void AddLocalScaleX(this Transform transform, float x)
	{
		vector3.Set(transform.localScale.x + x, transform.localScale.y, transform.localScale.z);
		transform.localScale = vector3;
	}

	public static void AddLocalScaleY(this Transform transform, float y)
	{
		vector3.Set(transform.localScale.x, transform.localScale.y + y, transform.localScale.z);
		transform.localScale = vector3;
	}

	public static void AddLocalScaleZ(this Transform transform, float z)
	{
		vector3.Set(transform.localScale.x, transform.localScale.y, transform.localScale.z + z);
		transform.localScale = vector3;
	}

	public static void SetEulerAngles(this Transform transform, float x, float y, float z)
	{
		vector3.Set(x, y, z);
		transform.eulerAngles = vector3;
	}

	public static void SetEulerAnglesX(this Transform transform, float x)
	{
		vector3.Set(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
		transform.eulerAngles = vector3;
	}

	public static void SetEulerAnglesY(this Transform transform, float y)
	{
		vector3.Set(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
		transform.eulerAngles = vector3;
	}

	public static void SetEulerAnglesZ(this Transform transform, float z)
	{
		vector3.Set(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
		transform.eulerAngles = vector3;
	}

	public static void AddEulerAngles(this Transform transform, float x, float y, float z)
	{
		vector3.Set(transform.eulerAngles.x + x, transform.eulerAngles.y + y, transform.eulerAngles.z + z);
		transform.eulerAngles = vector3;
	}

	public static void AddEulerAnglesX(this Transform transform, float x)
	{
		vector3.Set(transform.eulerAngles.x + x, transform.eulerAngles.y, transform.eulerAngles.z);
		transform.eulerAngles = vector3;
	}

	public static void AddEulerAnglesY(this Transform transform, float y)
	{
		vector3.Set(transform.eulerAngles.x, transform.eulerAngles.y + y, transform.eulerAngles.z);
		transform.eulerAngles = vector3;
	}

	public static void AddEulerAnglesZ(this Transform transform, float z)
	{
		vector3.Set(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + z);
		transform.eulerAngles = vector3;
	}

	public static void SetLocalEulerAngles(this Transform transform, float x, float y, float z)
	{
		vector3.Set(x, y, z);
		transform.localEulerAngles = vector3;
	}

	public static void SetLocalEulerAnglesX(this Transform transform, float x)
	{
		vector3.Set(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
		transform.localEulerAngles = vector3;
	}

	public static void SetLocalEulerAnglesY(this Transform transform, float y)
	{
		vector3.Set(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
		transform.localEulerAngles = vector3;
	}

	public static void SetLocalEulerAnglesZ(this Transform transform, float z)
	{
		vector3.Set(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalEulerAngles(this Transform transform, float x, float y, float z)
	{
		vector3.Set(transform.localEulerAngles.x + x, transform.localEulerAngles.y + y, transform.localEulerAngles.z + z);
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalEulerAnglesX(this Transform transform, float x)
	{
		vector3.Set(transform.localEulerAngles.x + x, transform.localEulerAngles.y, transform.localEulerAngles.z);
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalEulerAnglesY(this Transform transform, float y)
	{
		vector3.Set(transform.localEulerAngles.x, transform.localEulerAngles.y + y, transform.localEulerAngles.z);
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalEulerAnglesZ(this Transform transform, float z)
	{
		vector3.Set(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + z);
		transform.localEulerAngles = vector3;
	}

	public static void ToggleKeyword(this Material m, string id, bool enable)
	{
		if (enable)
		{
			m.EnableKeyword(id);
		}
		else
		{
			m.DisableKeyword(id);
		}
	}

	public static void ForeachReverse<T>(this IList<T> items, Action<T> action)
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			action(items[num]);
		}
	}

	public static void ForeachReverse<T>(this IList<T> items, Func<T, bool> action)
	{
		int num = items.Count - 1;
		while (num >= 0 && !action(items[num]))
		{
			num--;
		}
	}

	public static float ClampAngle(this float angle)
	{
		if (angle < 0f)
		{
			return 360f - (0f - angle) % 360f;
		}
		return angle % 360f;
	}

	public static Vector3 Random(this Vector3 v)
	{
		return new Vector3(Rand.Range(0f - v.x, v.x), Rand.Range(0f - v.y, v.y), Rand.Range(0f - v.z, v.z));
	}

	public static T Instantiate<T>(this T s) where T : ScriptableObject
	{
		string name = s.name;
		T val = UnityEngine.Object.Instantiate(s);
		val.name = name;
		return val;
	}

	public static int GetRuntimeEventCount(this UnityEventBase unityEvent)
	{
		Type typeFromHandle = typeof(UnityEventBase);
		Assembly assembly = Assembly.GetAssembly(typeFromHandle);
		Type type = assembly.GetType("UnityEngine.Events.InvokableCallList");
		Type type2 = assembly.GetType("UnityEngine.Events.BaseInvokableCall");
		Type type3 = typeof(List<>).MakeGenericType(type2);
		FieldInfo field = typeFromHandle.GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic);
		FieldInfo field2 = type.GetField("m_RuntimeCalls", BindingFlags.Instance | BindingFlags.NonPublic);
		object value = field.GetValue(unityEvent);
		object value2 = field2.GetValue(value);
		return (int)type3.GetProperty("Count").GetValue(value2, null);
	}
}
