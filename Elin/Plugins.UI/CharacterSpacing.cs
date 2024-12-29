using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Character Spacing", 7)]
[RequireComponent(typeof(Text))]
public class CharacterSpacing : BaseMeshEffect
{
	private const string REGEX_TAGS = "<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";

	[SerializeField]
	private float m_Offset;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public float offset
	{
		get
		{
			return m_Offset;
		}
		set
		{
			if (m_Offset != value)
			{
				m_Offset = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	protected CharacterSpacing()
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
		List<string> list = new List<string>();
		for (int i = 0; i < component.cachedTextGenerator.lineCount; i++)
		{
			int startCharIdx = component.cachedTextGenerator.lines[i].startCharIdx;
			int num = ((i < component.cachedTextGenerator.lineCount - 1) ? component.cachedTextGenerator.lines[i + 1].startCharIdx : component.text.Length);
			list.Add(component.text.Substring(startCharIdx, num - startCharIdx));
		}
		float num2 = offset * (float)component.fontSize / 100f;
		float num3 = 0f;
		IEnumerator enumerator = null;
		Match match = null;
		if (component.alignment == TextAnchor.LowerLeft || component.alignment == TextAnchor.MiddleLeft || component.alignment == TextAnchor.UpperLeft)
		{
			num3 = 0f;
		}
		else if (component.alignment == TextAnchor.LowerCenter || component.alignment == TextAnchor.MiddleCenter || component.alignment == TextAnchor.UpperCenter)
		{
			num3 = 0.5f;
		}
		else if (component.alignment == TextAnchor.LowerRight || component.alignment == TextAnchor.MiddleRight || component.alignment == TextAnchor.UpperRight)
		{
			num3 = 1f;
		}
		bool flag = true;
		int num4 = 0;
		for (int j = 0; j < list.Count; j++)
		{
			if (!flag)
			{
				break;
			}
			string text = list[j];
			int lengthWithoutTags = text.Length;
			if (lengthWithoutTags > component.cachedTextGenerator.characterCountVisible - num4)
			{
				lengthWithoutTags = component.cachedTextGenerator.characterCountVisible - num4;
				text = text.Substring(0, lengthWithoutTags) + " ";
				lengthWithoutTags++;
			}
			if (component.supportRichText)
			{
				enumerator = GetRegexMatchedTags(text, out lengthWithoutTags).GetEnumerator();
				match = null;
				if (enumerator.MoveNext())
				{
					match = (Match)enumerator.Current;
				}
			}
			bool flag2 = list[j].Length > 0 && (list[j][list[j].Length - 1] == ' ' || list[j][list[j].Length - 1] == '\n');
			float num5 = (float)(-(lengthWithoutTags - 1 - (flag2 ? 1 : 0))) * num2 * num3;
			float num6 = 0f;
			for (int k = 0; k < text.Length; k++)
			{
				if (!flag)
				{
					break;
				}
				if (component.supportRichText && match != null && match.Index == k)
				{
					k += match.Length - 1;
					num4 += match.Length - 1;
					num6 -= 1f;
					match = null;
					if (enumerator.MoveNext())
					{
						match = (Match)enumerator.Current;
					}
				}
				if (num4 * 6 + 5 >= m_Verts.Count)
				{
					flag = false;
					break;
				}
				for (int l = 0; l < 6; l++)
				{
					UIVertex value = m_Verts[num4 * 6 + l];
					value.position += Vector3.right * (num2 * num6 + num5);
					m_Verts[num4 * 6 + l] = value;
				}
				num4++;
				num6 += 1f;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
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
