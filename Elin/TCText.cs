using System;
using UnityEngine;
using UnityEngine.UI;

public class TCText : TCUI
{
	public static string[] popIDs = new string[2] { "PopTextSys", "PopTextGod" };

	public PopManager pop;

	[NonSerialized]
	public PopItemText lastEmo;

	public override Vector3 FixPos => TC._setting.textPos;

	public void Say(string s, float duration = 0f)
	{
		PopItem p;
		switch (s[0])
		{
		case '(':
			p = pop.PopText(s, null, EMono.core.config.ui.balloonBG ? "PopText_alt" : "PopTextThinking", default(Color), default(Vector3), duration);
			break;
		case '*':
			p = pop.PopText(s, null, "PopTextOno", default(Color), default(Vector3), duration);
			break;
		case '@':
		{
			int num = int.Parse(s[1].ToString());
			p = pop.PopText(s.Substring(2), null, popIDs[num], default(Color), default(Vector3), duration);
			break;
		}
		case '^':
			p = pop.PopText(s.Substring(1), null, "PopTextBroadcast", default(Color), default(Vector3), duration);
			break;
		case '|':
			p = pop.PopText(s.Substring(1), null, "PopTextAbility", default(Color), default(Vector3), duration);
			break;
		default:
			p = pop.PopText(s, null, EMono.core.config.ui.balloonBG ? "PopText_alt" : "PopText", default(Color), default(Vector3), duration);
			break;
		}
		if (!p)
		{
			return;
		}
		EMono.core.actionsNextFrame.Add(delegate
		{
			if (p != null && p.gameObject != null)
			{
				p.RebuildLayout(recursive: true);
				ContentSizeFitter[] componentsInChildren = p.GetComponentsInChildren<ContentSizeFitter>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
				LayoutGroup[] componentsInChildren2 = p.GetComponentsInChildren<LayoutGroup>();
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					componentsInChildren2[i].enabled = false;
				}
			}
		});
	}

	public void ShowEmo(Emo emo, float duration)
	{
		Sprite sprite = SpriteSheet.Get("Media/Graphics/Icon/icons_32", "emo_" + emo);
		if (lastEmo != null)
		{
			pop.Kill(lastEmo);
		}
		lastEmo = pop.PopText("", sprite, "PopTextEmo", default(Color), default(Vector3), duration);
	}

	public override void OnDraw(ref Vector3 pos)
	{
		if (!pop.enabled)
		{
			render.RemoveTC(this);
			return;
		}
		Vector3 pos2 = pos;
		lastPos = pos;
		base.OnDraw(ref pos2);
	}

	public override void OnKill()
	{
		DrawImmediate(ref lastPos);
		pop.CopyAll(EMono.ui.rectDynamic);
		pop.KillAll(instant: true);
	}
}
