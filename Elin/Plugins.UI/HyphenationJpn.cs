using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HyphenationJpn : UIBehaviour
{
	public int padding;

	public bool useParentWidth = true;

	public bool usePreferredWidth = true;

	public bool evaluateLate;

	private string text;

	private bool useHypenation;

	private RectTransform _rectTransform;

	private Text _text;

	private float width;

	private static readonly string RITCH_TEXT_REPLACE = "(\\<color=.*\\>|</color>|\\<size=.n\\>|</size>|<b>|</b>|<i>|</i>)";

	private static readonly char[] HYP_FRONT = ",)]｝、。）〕〉》」』】〙〗〟’”｠»ァィゥェォッャュョヮヵヶっぁぃぅぇぉっゃゅょゎ‐゠–〜ー?!！？‼⁇⁈⁉・:;。.".ToCharArray();

	private static readonly char[] HYP_BACK = "(（[｛〔〈《「『【〘〖〝‘“｟«".ToCharArray();

	private static readonly char[] HYP_LATIN = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789<>=/().,".ToCharArray();

	private RectTransform _RectTransform
	{
		get
		{
			if (_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	public Text _Text
	{
		get
		{
			if (_text == null)
			{
				_text = GetComponent<Text>();
			}
			return _text;
		}
	}

	private void UpdateText(string str)
	{
		if (useHypenation)
		{
			_Text.text = GetFormatedText(str).IsEmpty(str);
		}
	}

	public void _SetText(string str, bool useHypenation)
	{
		text = str;
		this.useHypenation = useHypenation;
		if (!useHypenation)
		{
			_Text.horizontalOverflow = HorizontalWrapMode.Wrap;
			_Text.text = text;
			return;
		}
		_Text.text = text;
		width = GetWidth();
		if (width < 50f)
		{
			this.RebuildLayoutTo<Canvas>();
			this.RebuildLayout();
			width = GetWidth();
			if (width < 50f)
			{
				BaseCore.Instance.actionsNextFrame.Add(delegate
				{
					width = GetWidth();
					UpdateText(text);
					this.RebuildLayout();
				});
			}
			else
			{
				UpdateText(text);
				this.RebuildLayout();
			}
		}
		else
		{
			UpdateText(text);
		}
	}

	public float GetWidth()
	{
		if (usePreferredWidth)
		{
			LayoutElement component = GetComponent<LayoutElement>();
			if ((bool)component && component.enabled && component.preferredWidth != -1f)
			{
				return component.preferredWidth;
			}
		}
		if ((bool)base.transform.parent && useParentWidth)
		{
			LayoutGroup component2 = base.transform.parent.GetComponent<LayoutGroup>();
			float num = base.transform.parent.Rect().rect.width;
			if ((bool)component2)
			{
				num = num - (float)component2.padding.left - (float)component2.padding.right;
			}
			return num;
		}
		return _RectTransform.rect.width;
	}

	private float GetSpaceWidth()
	{
		float textWidth = GetTextWidth("m m");
		float textWidth2 = GetTextWidth("mm");
		return textWidth - textWidth2;
	}

	private float GetTextWidth(string message)
	{
		if (_text.supportRichText)
		{
			message = Regex.Replace(message, RITCH_TEXT_REPLACE, string.Empty);
		}
		_Text.text = message;
		return _Text.preferredWidth;
	}

	private string GetFormatedText(string msg)
	{
		if (string.IsNullOrEmpty(msg))
		{
			return string.Empty;
		}
		msg = Regex.Replace(msg, "\\n", "〒");
		float num = width;
		float spaceWidth = GetSpaceWidth();
		_Text.horizontalOverflow = HorizontalWrapMode.Overflow;
		StringBuilder stringBuilder = new StringBuilder();
		float num2 = 0f;
		foreach (string word in GetWordList(msg))
		{
			string text = word;
			num2 += GetTextWidth(text);
			if (text.StartsWith("〒"))
			{
				num2 = 0f;
				stringBuilder.AppendLine();
				text = text.Replace("〒", "");
			}
			if (text == "〒")
			{
				num2 = 0f;
				stringBuilder.AppendLine();
				continue;
			}
			if (text == " ")
			{
				num2 += spaceWidth;
			}
			if (num2 > num - (float)padding)
			{
				stringBuilder.Append(Environment.NewLine);
				num2 = GetTextWidth(text);
			}
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}

	private List<string> GetWordList(string tmpText)
	{
		List<string> list = new List<string>();
		StringBuilder stringBuilder = new StringBuilder();
		char c = '\0';
		bool flag = false;
		for (int i = 0; i < tmpText.Length; i++)
		{
			char c2 = tmpText[i];
			char c3 = ((i < tmpText.Length - 1) ? tmpText[i + 1] : c);
			char c4 = ((i <= 0) ? c : (c4 = tmpText[i - 1]));
			if (c2 == '<')
			{
				flag = true;
			}
			if (c2 == '>')
			{
				flag = false;
			}
			stringBuilder.Append(c2);
			if (!flag && ((IsLatin(c2) && IsLatin(c4) && IsLatin(c2) && !IsLatin(c4)) || (!IsLatin(c2) && CHECK_HYP_BACK(c4)) || (!IsLatin(c3) && !CHECK_HYP_FRONT(c3) && !CHECK_HYP_BACK(c2)) || i == tmpText.Length - 1))
			{
				list.Add(stringBuilder.ToString());
				stringBuilder = new StringBuilder();
			}
		}
		return list;
	}

	private static bool CHECK_HYP_FRONT(char str)
	{
		return Array.Exists(HYP_FRONT, (char item) => item == str);
	}

	private static bool CHECK_HYP_BACK(char str)
	{
		return Array.Exists(HYP_BACK, (char item) => item == str);
	}

	private static bool IsLatin(char s)
	{
		return false;
	}
}
