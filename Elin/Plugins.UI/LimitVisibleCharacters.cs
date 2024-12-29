using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Limit Visible Characters", 8)]
[RequireComponent(typeof(Text))]
public class LimitVisibleCharacters : BaseMeshEffect
{
	private const string REGEX_TAGS = "<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";

	[SerializeField]
	private int m_VisibleCharacterCount;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public int visibleCharacterCount
	{
		get
		{
			return m_VisibleCharacterCount;
		}
		set
		{
			if (m_VisibleCharacterCount != value)
			{
				m_VisibleCharacterCount = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	protected LimitVisibleCharacters()
	{
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}
		vh.GetUIVertexStream(m_Verts);
		Text component = GetComponent<Text>();
		List<UIVertex> list = new List<UIVertex>();
		IEnumerator enumerator = null;
		Match match = null;
		string text = component.text.Substring(0, component.cachedTextGenerator.characterCountVisible);
		int lengthWithoutTags = text.Length;
		if (component.supportRichText)
		{
			enumerator = GetRegexMatchedTags(text, out lengthWithoutTags).GetEnumerator();
			match = null;
			if (enumerator.MoveNext())
			{
				match = (Match)enumerator.Current;
			}
		}
		if (visibleCharacterCount >= lengthWithoutTags)
		{
			return;
		}
		int num = 0;
		while (list.Count < visibleCharacterCount * 6)
		{
			bool flag = false;
			if (component.supportRichText && match != null && match.Index == num)
			{
				num += match.Length - 1;
				match = null;
				if (enumerator.MoveNext())
				{
					match = (Match)enumerator.Current;
				}
				flag = true;
			}
			if (!flag)
			{
				for (int i = 0; i < 6; i++)
				{
					UIVertex item = m_Verts[num * 6 + i];
					list.Add(item);
				}
			}
			num++;
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}

	private MatchCollection GetRegexMatchedTags(string text, out int lengthWithoutTags)
	{
		MatchCollection matchCollection = Regex.Matches(text, "<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>");
		lengthWithoutTags = 0;
		int num = 0;
		foreach (Match item in matchCollection)
		{
			num += item.Length;
		}
		lengthWithoutTags = text.Length - num;
		return matchCollection;
	}
}
