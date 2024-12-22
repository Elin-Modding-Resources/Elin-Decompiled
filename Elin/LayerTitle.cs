using System;
using UnityEngine;
using UnityEngine.UI;

public class LayerTitle : ELayer, IChangeLanguage, IChangeResolution
{
	[Serializable]
	public class TitleType
	{
		public Color light;

		public Color fog;

		public Sprite bg;

		public BGMData bgm;

		public BGMData bgmTitle;
	}

	public TitleType[] types;

	public static LayerTitle Instance;

	public static EmbarkActor actor;

	public LayerEditBio embark;

	public UIText textVersion;

	public UIText textIntro;

	public Transform transBook;

	public CanvasGroup cgBook;

	public CanvasGroup cgBG;

	public float speed;

	public GraphicRaycaster ray;

	public Transform titleActor;

	public Image uiLight;

	public RawImage imageFog;

	public RawImage imageBG;

	public bool toggle;

	public bool altTitle;

	public TitleType CurrentTitle => types[altTitle ? 1 : 0];

	public override void OnInit()
	{
		Instance = this;
		textVersion.text = ELayer.core.version.GetText() ?? "";
		textIntro.text = IO.LoadText(CorePath.CorePackage.TextData + "intro.txt");
		ELayer.ui.SetLight(enable: true);
		titleActor = Util.Instantiate(titleActor);
		Refresh();
		SoundManager.current.PlayBGM(CurrentTitle.bgmTitle);
	}

	public void Refresh()
	{
		titleActor.GetComponentInChildren<SpriteRenderer>().sprite = CurrentTitle.bg;
		uiLight.color = CurrentTitle.light;
		imageFog.color = CurrentTitle.fog;
		OnChangeResolution();
	}

	public void OnClickStart()
	{
		if (ELayer.ui.GetTopLayer() is LayerTitle)
		{
			Core.TryWarnMod(delegate
			{
				embark = ELayer.ui.AddLayer<LayerEditBio>();
				SoundManager.current.PlayBGM(CurrentTitle.bgm);
			}, ELayer.core.mods.CountUserMod() > 0);
		}
	}

	public void OnChangeResolution()
	{
		imageBG.transform.localScale = new Vector3(Mathf.Max(1f, 1.7777778f / ((float)Screen.width / (float)Screen.height)), 1f, 1f);
	}

	public void OnClickContinue()
	{
		if (ELayer.ui.GetTopLayer() is LayerTitle)
		{
			Core.TryWarnMod(delegate
			{
				ELayer.ui.AddLayer<LayerLoadGame>().Init(_backup: false);
			}, ELayer.core.mods.CountUserMod() > 0);
		}
	}

	public void OnClickConfig()
	{
		ELayer.ui.AddLayer<LayerConfig>();
	}

	public void OnClickMod()
	{
		ELayer.ui.AddLayer<LayerMod>();
	}

	public void OnClickCredit()
	{
		ELayer.ui.AddLayer<LayerCredit>();
	}

	public void OnClickAnnounce()
	{
		ELayer.ui.AddLayer("LayerAnnounce");
	}

	public void OnClickAbout()
	{
		ELayer.ui.AddLayer("LayerAbout");
	}

	public void OnClickFeedback()
	{
		ELayer.ui.ToggleFeedback();
	}

	public void OnClickExit()
	{
		if (Application.isEditor)
		{
			ELayer.ui.RemoveLayer(this);
			if ((bool)actor)
			{
				UnityEngine.Object.DestroyImmediate(actor.gameObject);
			}
			ELayer.ui.AddLayer<LayerTitle>();
		}
		else
		{
			ELayer.core.Quit();
		}
	}

	public override void OnKill()
	{
		UnityEngine.Object.DestroyImmediate(titleActor.gameObject);
		if ((bool)embark)
		{
			ELayer.ui.RemoveLayer(embark);
			UnityEngine.Object.DestroyImmediate(embark);
		}
		ELayer.ui.SetLight(enable: false);
	}

	private void Update()
	{
		LayerEditBio layer = ELayer.ui.GetLayer<LayerEditBio>();
		if (!layer && SoundManager.current.currentBGM.id != CurrentTitle.bgmTitle.name)
		{
			if (toggle)
			{
				altTitle = !altTitle;
			}
			SoundManager.current.PlayBGM(CurrentTitle.bgmTitle);
			Refresh();
		}
		float num = Mathf.Clamp(cgBook.alpha + Time.smoothDeltaTime * speed * (float)((!layer) ? 1 : (-1)), 0f, 1f);
		if (cgBook.alpha != num)
		{
			cgBook.alpha = num;
		}
		if (cgBG.alpha != num)
		{
			cgBG.alpha = num;
		}
		ray.enabled = num == 1f;
	}

	public void OnChangeLanguage()
	{
		textIntro.text = IO.LoadText(CorePath.CorePackage.TextData + "intro.txt");
	}

	public static void KillActor()
	{
		if ((bool)actor)
		{
			UnityEngine.Object.DestroyImmediate(actor.gameObject);
		}
	}
}
