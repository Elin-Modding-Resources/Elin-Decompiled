using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Person : EClass
{
	[JsonProperty]
	public string name;

	[JsonProperty]
	public string idPortrait;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string tones;

	[JsonProperty]
	public int uidChara;

	[JsonProperty]
	public int gender;

	[JsonProperty]
	public SerializableColor colorHair;

	public RefChara refChara = new RefChara();

	public Chara _tempChara;

	public string tempName;

	public SourcePerson.Row source => EClass.sources.persons.map.TryGetValue(id.IsEmpty("narrator"));

	public Chara chara => _tempChara ?? refChara.GetAndCache(uidChara);

	public bool hasChara => chara != null;

	public bool HumanSpeak
	{
		get
		{
			if (chara != null)
			{
				if (!chara.IsPC)
				{
					return chara.IsHumanSpeak;
				}
				return true;
			}
			return true;
		}
	}

	public string Name => tempName.IsEmpty(name.IsEmpty(chara?.Name ?? source?.GetText("name", returnNull: true) ?? "null")).ToTitleCase();

	public string NameBraced => tempName.IsEmpty(name.IsEmpty(chara?.NameBraced ?? source?.GetText("name", returnNull: true) ?? "null")).ToTitleCase();

	public string Aka => chara?.Aka ?? source?.GetText("aka", returnNull: true) ?? "";

	public Person()
	{
	}

	public Person(string _id, Chara c = null)
	{
		id = _id;
		SetChara(c);
	}

	public Person(Religion r)
	{
		id = r.id;
		name = r.Name;
		idPortrait = "UN_" + r.id;
		if (!Portrait.modPortraits.dict.ContainsKey(idPortrait))
		{
			idPortrait = "UN_eyth";
		}
	}

	public Person(Chara c)
	{
		SetChara(c);
	}

	public void SetChara(Chara c)
	{
		if (c != null)
		{
			_tempChara = c;
			uidChara = c.uid;
			name = chara.Name;
			idPortrait = chara.GetIdPortrait();
			gender = chara.bio.gender;
			tones = chara.c_idTone;
		}
	}

	public string ApplyTone(string text)
	{
		return chara?.ApplyTone(text) ?? text;
	}

	public string GetDramaTitle()
	{
		if (!tempName.IsEmpty())
		{
			return tempName;
		}
		if (chara != null)
		{
			Biography bio = chara.bio;
			string text = ((!EClass.debug.showTone || !Lang.setting.useTone || chara.c_idTalk.IsEmpty() || chara.c_idTone.IsEmpty()) ? "" : ("  (" + chara.c_idTone.Split('|')[0].TrimEnd('-') + "な" + MOD.listTalk.GetTalk(chara.c_idTalk, "name") + ")"));
			string dramaText = chara.trait.GetDramaText();
			if (!dramaText.IsEmpty())
			{
				text = text + " " + dramaText;
			}
			return chara.Name.ToTitleCase() + " - " + chara.race.GetText().ToTitleCase() + " " + bio.TextAge(chara) + " " + Lang._gender(bio.gender) + text;
		}
		if (Name.IsEmpty())
		{
			return Aka;
		}
		if (Aka.IsEmpty())
		{
			return "「" + Name + "」";
		}
		return Aka.ToTitleCase() + "「" + Name + "」";
	}

	public void SetImage(Image image)
	{
		Sprite sprite = chara?.GetSprite() ?? Resources.Load<Sprite>("Media/Drama/Actor/" + source.idActor) ?? SpriteSheet.Get(source.idActor);
		if ((bool)sprite)
		{
			image.sprite = sprite;
			image.SetNativeSize();
		}
	}
}
