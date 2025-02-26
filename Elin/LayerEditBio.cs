using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class LayerEditBio : ELayer
{
	public UICharaMaker maker;

	public Chara chara;

	public EmbarkActor moldActor;

	public Image imageBG;

	public override void OnAfterAddLayer()
	{
		if (ELayer.game == null)
		{
			Game.Create();
		}
		if (!LayerTitle.actor)
		{
			LayerTitle.actor = Util.Instantiate(moldActor);
		}
		SetChara(ELayer.pc);
		ELayer.ui.hud.hint.Show("hintEmbarkTop".lang(), icon: false);
		imageBG.SetAlpha(0f);
		imageBG.DOFade(0.15f, 10f);
	}

	public void SetChara(Chara c, UnityAction _onKill = null)
	{
		chara = c ?? ELayer.player.chara;
		maker.SetChara(chara);
		if (_onKill != null)
		{
			onKill.AddListener(_onKill);
		}
	}

	public void OnClickStart()
	{
		if (!ELayer.ui.GetLayer<LayerWorldSetting>())
		{
			ELayer.ui.AddLayer<LayerWorldSetting>();
		}
	}

	public void OnClickHelp()
	{
		LayerHelp.Toggle("general", "3");
	}
}
