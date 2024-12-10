using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerEditPCC : ELayer
{
	public UIPCC.Mode mode;

	public UIPCC uiPCC;

	public Portrait portrait;

	public Chara chara;

	public UISlider sliderPortrait;

	public PCCData backup;

	public string backupPortrait;

	public bool applied;

	private Color lastHair;

	public bool UniformMode
	{
		get
		{
			if (mode != UIPCC.Mode.UniformM)
			{
				return mode == UIPCC.Mode.UniformF;
			}
			return true;
		}
	}

	public bool IsMale => mode == UIPCC.Mode.UniformM;

	public void Activate(Chara _chara, UIPCC.Mode m, string idUniform = null, Action _onKill = null)
	{
		chara = (_chara.IsPCC ? _chara : ELayer.pc);
		mode = m;
		uiPCC.exportPath = CorePath.user + "PCC/";
		uiPCC.onShowPalette = delegate(PCC pcc, string id, PCC.Part part, TestActor actor)
		{
			LayerColorPicker layerColorPicker = ELayer.ui.AddLayer<LayerColorPicker>();
			Color color = pcc.data.GetColor(id);
			layerColorPicker.SetColor(color, color, delegate(PickerState state, Color _c)
			{
				pcc.data.SetColor(id, _c);
				actor.Reset();
			});
		};
		PCCData newData = new PCCData();
		newData.Set(chara.pccData);
		backup = IO.DeepCopy(newData);
		backupPortrait = chara.c_idPortrait;
		if (UniformMode)
		{
			ELayer.game.uniforms.Apply(newData, idUniform, IsMale, canUseOtherGender: false);
			windows[0].SetCaption("_uniform".lang(ELayer.sources.jobs.map[idUniform].GetName().ToTitleCase(), IsMale ? "male".lang() : "female".lang()));
		}
		else
		{
			ELayer.game.uniforms.Apply(newData, PCCUniformManager.dummy);
		}
		uiPCC.Init(newData, mode);
		SetPortraitSlider();
		windows[0].AddBottomButton("removeAllCloth", delegate
		{
			uiPCC.RemoveAll();
			ELayer.Sound.Play("offering");
		});
		windows[0].AddBottomButton("reset", delegate
		{
			if (UniformMode)
			{
				ELayer.game.uniforms.RemoveUniform(newData);
			}
			uiPCC.Reset(backup);
			chara.c_idPortrait = backupPortrait;
			SetPortraitSlider();
			ELayer.Sound.Play("offering");
			newData.colors.Clear();
		});
		if (UniformMode)
		{
			PCCData otherUni = ELayer.game.uniforms.GetUniform(idUniform, !IsMale);
			if (otherUni != null)
			{
				windows[0].AddBottomButton("copyUniform", delegate
				{
					PCCData pCCData = new PCCData();
					pCCData.Set(chara.pccData);
					ELayer.game.uniforms.Apply(pCCData, otherUni);
					uiPCC.Reset(pCCData);
					ELayer.Sound.Play("offering");
				});
			}
		}
		windows[2].SetActive(enable: true);
		portrait.SetActive(!UniformMode);
		if (_onKill != null)
		{
			SetOnKill(_onKill);
		}
	}

	public void Apply()
	{
		PCCData data = uiPCC.pcc.data;
		ELayer.game.uniforms.Apply(data, chara.job.id, IsMale, canUseOtherGender: true);
		chara.pccData.Set(data);
		PCC.Get(chara.pccData).Build();
		chara.SetInt(105, IntColor.ToInt(chara.pccData.GetHairColor()));
		applied = true;
	}

	public void OnClickChangePortrait()
	{
		ELayer.ui.AddLayer<LayerEditPortrait>().Activate(chara, uiPCC.pcc.data, delegate
		{
			portrait.SetChara(chara, uiPCC.actor.data);
			uiPCC.actor.Reset();
		});
	}

	public override void OnKill()
	{
		Apply();
	}

	private void Update()
	{
		Color hairColor = uiPCC.actor.data.GetHairColor();
		if (hairColor != lastHair)
		{
			portrait.SetChara(chara, uiPCC.actor.data);
			lastHair = hairColor;
		}
		int wheel = EInput.wheel;
		if (InputModuleEX.IsPointerOver(windows[1].transform))
		{
			if (wheel < 0)
			{
				uiPCC.actor.provider.NextDir();
			}
			if (wheel > 0)
			{
				uiPCC.actor.provider.PrevDir();
			}
		}
	}

	public void SetPortraitSlider()
	{
		if ((bool)sliderPortrait)
		{
			List<ModItem<Sprite>> list = Portrait.ListPlayerPortraits(chara.bio.gender, chara != ELayer.pc);
			list.Sort((ModItem<Sprite> a, ModItem<Sprite> b) => Lang.comparer.Compare(a.id, b.id));
			sliderPortrait.SetList(list.Find((ModItem<Sprite> a) => a.id == chara.c_idPortrait), list, delegate(int a, ModItem<Sprite> b)
			{
				chara.c_idPortrait = b.id;
				portrait.SetChara(chara, uiPCC.actor.data);
			}, (ModItem<Sprite> a) => (!a.id.IsEmpty()) ? a.id : "None");
		}
	}
}
