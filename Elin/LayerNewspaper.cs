using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LayerNewspaper : ELayer
{
	public UIText textDate;

	public UIText textFortune;

	public UIText textVote;

	public UIText textVoteRemaining;

	public UIList listChat;

	public UIList listWeather;

	public UIList listVote;

	public UIList listNews;

	public Image imageHeadline;

	public Image imageAdRight;

	public Image moldAdBottom;

	public List<Sprite> spritesAd;

	public List<Sprite> spritesSmallAd;

	public List<Sprite> spritesHeadline;

	public LayoutGroup layoutAdBottom;

	public LayoutGroup layoutPortrait;

	public Portrait moldPortrait;

	public List<string> desiredNews;

	public UIButton buttonPage;

	public GameObject goTop;

	public GameObject goBack;

	public Sprite spriteComicBig;

	public bool topPage;

	private int seed;

	public DayData data => ELayer.world.dayData;

	public override void OnInit()
	{
		if (ELayer.debug.enable)
		{
			NewsList.dict = null;
			ELayer.world.CreateDayData();
		}
		seed = (ELayer.debug.enable ? (-1) : ELayer.world.dayData.seed);
		Rand.UseSeed(seed, delegate
		{
			imageAdRight.sprite = spritesAd.RandomItem();
			imageAdRight.SetNativeSize();
		});
		textDate.text = "news_date".lang(Lang.GetList("monthEng")[ELayer.world.date.month - 1], ELayer.world.date.day.ToString() ?? "", ELayer.world.date.year.ToString() ?? "");
		int luck = (int)ELayer.world.dayData.luck;
		textFortune.text = "news_tellLuck".lang(Lang.GetList("dayLuck")[luck], Lang.GetList("dayLuck2")[luck]);
		RefreshAD();
		RefreshChat();
		RefreshVote();
		RefreshWeather();
		RefreshNews();
		RefreshPage();
	}

	public void OnClickPage()
	{
		topPage = !topPage;
		buttonPage.mainText.SetText((topPage ? "news_firstPage" : "news_secondPage").lang());
		SE.Play("click_scroll");
		RefreshPage();
	}

	public void RefreshPage()
	{
		goTop.SetActive(topPage);
		goBack.SetActive(!topPage);
	}

	public void OnClickComic()
	{
		SE.Play("click_recipe");
		ELayer.ui.AddLayer<LayerImage>().SetImage(spriteComicBig);
	}

	public async void RefreshNews()
	{
		moldPortrait = layoutPortrait.CreateMold<Portrait>();
		List<NewsList.Item> list = NewsList.GetNews(seed);
		if (desiredNews.Count > 0)
		{
			for (int i = 0; i < desiredNews.Count; i++)
			{
				foreach (NewsList.Item item2 in NewsList.listAll)
				{
					if (item2.title.Contains(desiredNews[i]))
					{
						list[i] = item2;
						break;
					}
				}
			}
		}
		listNews.callbacks = new UIList.Callback<NewsList.Item, UIItem>
		{
			onInstantiate = delegate(NewsList.Item a, UIItem b)
			{
				b.text1.text = a.title;
				b.text2.SetText(a.content);
			},
			onList = delegate
			{
				foreach (NewsList.Item item3 in list)
				{
					listNews.Add(item3);
				}
			}
		};
		listNews.List();
		Rand.UseSeed(seed, delegate
		{
			imageHeadline.sprite = spritesHeadline.RandomItem();
		});
		foreach (NewsList.Item item4 in list)
		{
			List<string> listImageId = item4.listImageId;
			if (listImageId.Count == 0)
			{
				continue;
			}
			Sprite sprite = Resources.Load<Sprite>("UI/Layer/LayerNewspaper/Headline/headline_" + listImageId[0]);
			if (!sprite)
			{
				continue;
			}
			imageHeadline.sprite = sprite;
			if (listImageId.Count <= 1)
			{
				break;
			}
			for (int num = 1; num < listImageId.Count; num++)
			{
				ModItem<Sprite> item = Portrait.modPortraits.GetItem(listImageId[num], returnNull: true);
				if (item != null)
				{
					Util.Instantiate(moldPortrait, layoutPortrait).SetPortrait(item.id);
				}
			}
			layoutPortrait.RebuildLayout();
			break;
		}
	}

	public void RefreshAD()
	{
		Rand.SetSeed(seed);
		moldAdBottom = layoutAdBottom.CreateMold<Image>();
		float num = layoutAdBottom.Rect().sizeDelta.x - 20f;
		IList<Sprite> list = spritesSmallAd.Copy().Shuffle();
		for (int i = 0; i < list.Count; i++)
		{
			Sprite sprite = list[i];
			if (num > sprite.textureRect.width)
			{
				Image image = Util.Instantiate(moldAdBottom, layoutAdBottom);
				num = num - sprite.textureRect.width - 5f;
				image.sprite = sprite;
				image.SetNativeSize();
			}
		}
		layoutAdBottom.RebuildLayout();
		Rand.SetSeed();
	}

	public async void RefreshChat()
	{
		List<Net.ChatLog> logs = await Net.GetChat(ChatCategory.Test, Lang.langCode);
		if (isDestroyed || logs.Count == 0)
		{
			return;
		}
		listChat.callbacks = new UIList.Callback<Net.ChatLog, UIItem>
		{
			onInstantiate = delegate(Net.ChatLog a, UIItem b)
			{
				b.text1.SetText(a.msg);
			},
			onList = delegate
			{
				foreach (Net.ChatLog item in logs)
				{
					listChat.Add(item);
				}
			}
		};
		listChat.List();
	}

	public async void RefreshVote()
	{
		List<Net.VoteLog> logs = await Net.GetVote(Lang.langCode);
		if (isDestroyed || logs.Count == 0)
		{
			return;
		}
		textVote.SetText(logs[0].name.StripBrackets());
		int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		int num2 = (logs[0].time - num) / 60 / 60 / 24;
		textVoteRemaining.SetText("news_voteLeft".lang(num2.ToString() ?? ""));
		logs.RemoveAt(0);
		logs.Sort((Net.VoteLog a, Net.VoteLog b) => b.count - a.count);
		listVote.callbacks = new UIList.Callback<Net.VoteLog, UIItem>
		{
			onInstantiate = delegate(Net.VoteLog a, UIItem b)
			{
				b.text1.SetText(a.name);
				b.text2.SetText(a.count.ToString() ?? "");
				b.button1.SetOnClick(delegate
				{
					SE.Click();
					Net.SendVote(a.index, Lang.langCode);
					a.count++;
					b.text2.SetText(a.count.ToString() ?? "");
					foreach (UIList.ButtonPair button in listVote.buttons)
					{
						(button.component as UIItem).button1.SetActive(enable: false);
					}
				});
			},
			onList = delegate
			{
				foreach (Net.VoteLog item in logs)
				{
					listVote.Add(item);
				}
			}
		};
		listVote.List();
	}

	public void RefreshWeather()
	{
		List<Weather.WeatherForecast> list = ELayer.world.weather.GetWeatherForecast();
		listWeather.callbacks = new UIList.Callback<Weather.WeatherForecast, UIItem>
		{
			onInstantiate = delegate(Weather.WeatherForecast a, UIItem b)
			{
				string text = a.date.month + "/" + a.date.day;
				foreach (KeyValuePair<Weather.Condition, int> item in a.cons.OrderByDescending((KeyValuePair<Weather.Condition, int> keyValuePair) => keyValuePair.Value))
				{
					if (b.image1.sprite == null)
					{
						b.image1.sprite = ELayer.core.refs.icons.weather[item.Key];
					}
					else if (b.image2.sprite == null)
					{
						b.image2.sprite = ELayer.core.refs.icons.weather[item.Key];
						b.image2.SetActive(enable: true);
					}
				}
				b.text1.text = text;
			},
			onList = delegate
			{
				for (int i = 0; i < 7; i++)
				{
					if (list.Count > i && list[i].cons.Count > 0)
					{
						listWeather.Add(list[i]);
					}
				}
			}
		};
		listWeather.List();
	}
}
