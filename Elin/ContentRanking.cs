using UnityEngine;

public class ContentRanking : EContent
{
	public UIList list;

	public UIText textTitle;

	public UIText textFactionName;

	public Sprite[] spriteTrophies;

	public GameObject comingSoon;

	public GameObject wet;

	public override void OnSwitchContent(int idTab)
	{
		SwitchRanking("contribution");
	}

	public void SwitchRanking(string id)
	{
		textTitle.text = Lang.Get("rank_" + id);
		textFactionName.text = EClass.Home.name;
		switch (id)
		{
		}
		list.callbacks = new UIList.Callback<Chara, ButtonChara>
		{
			onInstantiate = delegate(Chara a, ButtonChara b)
			{
				b.SetChara(a, ButtonChara.Mode.Journal);
				b.item.text1.text = "123456";
				b.item.text2.text = "contribution".lang();
			}
		};
		list.Clear();
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.faction == EClass.Home)
			{
				list.Add(chara);
			}
		}
		list.Refresh();
		for (int num = 0; num < list.buttons.Count; num++)
		{
			ButtonChara buttonChara = list.buttons[num].component as ButtonChara;
			buttonChara.item.text3.text = "rank".lang((num + 1).ToString() ?? "");
			buttonChara.item.image1.SetActive(num < 3);
			if (num < 3)
			{
				buttonChara.item.image1.sprite = spriteTrophies[num];
			}
		}
		comingSoon.SetActive(id != "contribution");
		wet.SetActive(id == "wettunic");
		list.SetActive(id == "contribution");
		this.RebuildLayout(recursive: true);
	}
}
