using System;
using UnityEngine;
using UnityEngine.UI;

public class TCText : TCUI
{
	public static string[] popIDs = new string[4] { "PopTextSys", "PopTextGod", "PopTextASMR", "PopTextOPA" };

	public PopManager pop;

	[NonSerialized]
	public PopItemText lastEmo;

	public override Vector3 FixPos => TC._setting.textPos;

	public void Say(string s, float duration = 0f)
	{
		if (s.IsEmpty())
		{
			return;
		}
		string id;
		float chance;
		while (ExtractSoundTag(ref s, out id, out chance))
		{
			if (Rand.rndf(1f) <= chance)
			{
				base.owner.PlaySound(id);
			}
		}
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
			if (num == 3)
			{
				Shaker.ShakeCam("opa");
			}
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

	private static bool ExtractSoundTag(ref string text, out string id, out float chance)
	{
		id = null;
		chance = 1f;
		int num = text.IndexOf("<sound", StringComparison.Ordinal);
		if (num == -1)
		{
			return false;
		}
		int num2 = text.IndexOf('>', num);
		if (num2 == -1)
		{
			return false;
		}
		int num3 = num + 6;
		string text2 = text.Substring(num3, num2 - num3);
		int num4 = text2.IndexOf(',');
		if (num4 == -1)
		{
			id = text2;
		}
		else
		{
			id = text2[..num4];
			float.TryParse(text2[(num4 + 1)..], out chance);
		}
		text = text.Remove(num, num2 - num + 1);
		chance = Mathf.Clamp01(chance);
		return true;
	}
}
