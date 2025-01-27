using System;
using System.Collections.Generic;
using UnityEngine;

public class CardRenderer : RenderObject
{
	public TCOrbit orbit;

	public Card owner;

	public CardActor actor;

	public bool hasActor;

	public bool hasText;

	public bool isChara;

	public bool skip;

	public Vector3 position;

	public TransAnime anime;

	public List<TC> listTC = new List<TC>();

	public TCSimpleText simpleText;

	public RendererReplacer replacer;

	public virtual bool IsMoving => false;

	public Vector3 PositionCenter()
	{
		return new Vector3(position.x + data.size.x + data.offset.x * 0.5f, position.y + data.size.y + data.offset.y, position.z);
	}

	public virtual void SetOwner(Card c)
	{
		owner = c;
		if (data == null)
		{
			data = owner.sourceCard.renderData;
		}
		isChara = c.isChara;
		usePass = data.pass != null;
	}

	public override void Draw(RenderParam p)
	{
		Vector3 v = p.NewVector3;
		Draw(p, ref v, drawShadow: true);
	}

	public override void RenderToRenderCam(RenderParam p)
	{
		Vector3 v = EClass.scene.camSupport.renderPos;
		if (data.multiSize)
		{
			v.y -= 0.8f;
		}
		p.x = v.x;
		p.y = v.y;
		p.z = v.z;
		Draw(p, ref v, drawShadow: false);
	}

	public override void Draw(RenderParam p, ref Vector3 v, bool drawShadow)
	{
		if (skip)
		{
			skip = false;
			return;
		}
		sync = RenderObject.syncFrame;
		RenderObject.currentParam = p;
		p.dir = owner.dir;
		if (!isSynced)
		{
			OnEnterScreen();
			RenderObject.syncList.Add(this);
		}
		if ((bool)orbit)
		{
			orbit.Refresh();
		}
		else
		{
			TrySpawnOrbit();
		}
		if (isChara && owner.parent == EClass.game.activeZone)
		{
			if (owner.Chara.bossText && !EClass.ui.IsActive && !SplashText.Instance)
			{
				SplashText splashText = Util.Instantiate<SplashText>("Media/Text/SplashText_boss2", EClass.ui.rectDynamic);
				string text = owner.Chara.Aka.ToTitleCase(wholeText: true);
				string text2 = owner.Chara.NameSimple.ToTitleCase();
				if (!Lang.isBuiltin && Lang.langCode != "CN")
				{
					text = owner.Chara.source.aka.ToTitleCase(wholeText: true);
					text2 = owner.Chara.source.name.ToTitleCase();
				}
				splashText.textSmall.text = text;
				splashText.textBig.text = text2;
				owner.Chara.bossText = false;
			}
			if (owner.Chara.host == null)
			{
				UpdatePosition(ref v, p);
			}
			if (owner.Chara.ai is AI_Trolley { IsRunning: not false } aI_Trolley)
			{
				drawShadow = false;
				if (aI_Trolley.trolley.HideChara)
				{
					if (hasActor)
					{
						actor.SetActive(enable: false);
					}
					return;
				}
			}
			if (hasActor)
			{
				actor.SetActive(enable: true);
			}
		}
		else
		{
			p.x = (position.x = v.x);
			p.y = (position.y = v.y);
			p.z = (position.z = v.z);
		}
		if (anime != null)
		{
			anime.Update();
		}
		if (!isChara && !owner.IsInstalled && owner.category.tileDummy != 0 && !owner.isRoofItem && owner.ExistsOnMap && owner.trait.UseDummyTile)
		{
			SubPassData.Current = SubPassData.Default;
			RenderDataObjDummy rendererObjDummy = EClass.scene.screenElin.tileMap.rendererObjDummy;
			p.tile = rendererObjDummy.tile;
			p.dir = 0;
			if (hasActor)
			{
				actor.SetActive(enable: false);
			}
			rendererObjDummy.Draw(p);
			if (drawShadow && !owner.pos.cell.ignoreObjShadow)
			{
				EClass.scene.screenElin.tileMap.passShadow.AddShadow(position.x + rendererObjDummy.offsetShadow.x, position.y + rendererObjDummy.offsetShadow.y + p.shadowFix, position.z + rendererObjDummy.offsetShadow.z, ShadowData.Instance.items[rendererObjDummy.shadowPref.shadow], rendererObjDummy.shadowPref, 0, p.snow);
			}
		}
		else
		{
			SubPassData.Current = owner.GetSubPassData();
			SourcePref pref = GetPref();
			RenderData renderData = data;
			int prefIndex = owner.PrefIndex;
			if (Player.seedHallucination != 0 && CanBeHallucinated())
			{
				Rand.SetSeed(Player.seedHallucination + owner.uid);
				CardRow cardRow = null;
				bool flag = false;
				for (int i = 0; i < 20; i++)
				{
					if (owner.isChara)
					{
						cardRow = EClass.sources.charas.rows.RandomItem();
						if (cardRow.multisize)
						{
							continue;
						}
					}
					else
					{
						cardRow = EClass.sources.things.rows.RandomItem();
						if (cardRow.tileType != owner.TileType)
						{
							continue;
						}
					}
					if (cardRow.chance != 0 && cardRow._tiles.Length != 0 && cardRow.idActor.IsEmpty() && cardRow.idExtra.IsEmpty() && SubPassData.Current == SubPassData.Default && !(cardRow._idRenderData != owner.sourceCard._idRenderData))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					renderData = cardRow.renderData;
					pref = cardRow.pref;
					cardRow.model.dir = owner.dir;
					cardRow.model.SetRenderParam(p);
					prefIndex = cardRow.model.PrefIndex;
				}
				Rand.SetSeed();
			}
			if (owner.trait is TraitFigure)
			{
				if (!owner.c_idRefCard.IsEmpty() && (owner.IsInstalled || EClass.pc.held == owner || !owner.ExistsOnMap || owner.isRoofItem))
				{
					TraitFigure traitFigure = owner.trait as TraitFigure;
					SourceChara.Row row = EClass.sources.charas.map.TryGetValue(owner.c_idRefCard) ?? EClass.sources.charas.map["putty"];
					renderData = row.renderData;
					pref = row.pref;
					if (row._tiles.Length == 0 || data.pass == null)
					{
						renderData = owner.sourceCard.renderData;
						pref = owner.sourceCard.pref;
					}
					else
					{
						p.tile = row._tiles[owner.refVal % row._tiles.Length] * ((owner.dir % 2 == 0) ? 1 : (-1));
						p.matColor = traitFigure.GetMatColor();
						drawShadow = traitFigure.ShowShadow;
						pref = row.pref;
					}
					if (renderData.pass != null && renderData.pass.name == "pass charaL" && EClass.pc.held == owner)
					{
						int num = (int)renderData.pass.pmesh.tiling.x;
						p.tile = (int)p.tile / num * num * 2 + (int)p.tile % num;
					}
				}
				else
				{
					renderData = owner.sourceCard.renderData;
				}
			}
			if (replacer != null)
			{
				renderData = replacer.data;
				pref = replacer.pref;
				SubPassData.Current = SubPassData.Default;
			}
			if (isChara)
			{
				p.x += pref.x * (float)((!owner.flipX) ? 1 : (-1));
			}
			p.y += pref.y;
			int shadow = pref.shadow;
			bool flag2 = isChara && owner.isHidden && !EClass.pc.canSeeInvisible && (!EClass.pc.hasTelepathy || !owner.Chara.race.visibleWithTelepathy);
			if (drawShadow && shadow != 1 && SubPassData.Current.shadow && (!flag2 || owner.IsPC))
			{
				bool flag3 = ((!isChara) ? (owner.dir % 2 == 1) : (owner.dir == 1 || owner.dir == 2));
				EClass.scene.screenElin.tileMap.passShadow.AddShadow(position.x + (flag3 ? (-1f) : 1f) * renderData.offsetShadow.x, position.y + renderData.offsetShadow.y + (owner.TileType.UseMountHeight ? 0f : p.shadowFix) - RenderObject.altitudeFix * (float)owner.altitude, position.z + renderData.offsetShadow.z, ShadowData.Instance.items[shadow], pref, prefIndex, p.snow);
			}
			if (usePass)
			{
				if (owner.noSnow)
				{
					p.snow = false;
				}
				if (!flag2)
				{
					renderData.Draw(p);
				}
			}
			else if (hasActor)
			{
				if (owner.isChara && owner.Chara.ride != null && (EClass.core.config.game.showRide == 2 || (EClass.core.config.game.showRide == 1 && !owner.Cell.HasRoof && !EClass._map.IsIndoor)) && !owner.IsDeadOrSleeping)
				{
					Chara ride = owner.Chara.ride;
					CharaActorPCC charaActorPCC = ride.renderer.actor as CharaActorPCC;
					CharaActorPCC charaActorPCC2 = actor as CharaActorPCC;
					ride.angle = owner.angle;
					if (charaActorPCC != null && charaActorPCC2 != null)
					{
						charaActorPCC.provider.currentDir = charaActorPCC2.provider.currentDir;
						charaActorPCC.provider.currentFrame = charaActorPCC2.provider.currentFrame;
						charaActorPCC.provider.SetSpriteMain();
						charaActorPCC.RefreshSprite();
					}
					PCCData.RideData ride2 = (ride.renderer as CharaRenderer).pccData.ride;
					float x = p.x;
					float y = p.y;
					float z = p.z;
					Vector3 v2 = new Vector3(v.x, v.y, v.z);
					ride.renderer.Draw(p, ref v2, drawShadow: false);
					int currentDir = actor.currentDir;
					p.x = x + RenderObject.renderSetting.ridePos[currentDir].x + ride2.x * (float)(currentDir switch
					{
						2 => -1, 
						1 => 1, 
						_ => 0, 
					});
					p.y = y + RenderObject.renderSetting.ridePos[currentDir].y + ride2.y + ride2.jump * (float)((actor.GetFrame() % 2 == 1) ? 1 : 0);
					p.z = z + RenderObject.renderSetting.ridePos[currentDir].z - ride2.z;
				}
				if (flag2)
				{
					actor.SetActive(enable: false);
				}
				else
				{
					actor.SetActive(enable: true);
					actor.OnRender(p);
				}
			}
			if (isChara)
			{
				if (owner.Chara.parasite != null)
				{
					owner.Chara.parasite.renderer.position = position;
				}
				if (owner.Chara.ride != null)
				{
					owner.Chara.ride.renderer.position = position;
				}
			}
			SubPassData.Current = SubPassData.Default;
		}
		if (listTC.Count > 0)
		{
			RenderObject.tempV = position;
			RenderObject.tempV.y += data.offset.y + data.size.y;
			for (int num2 = listTC.Count - 1; num2 >= 0; num2--)
			{
				listTC[num2].OnDraw(ref RenderObject.tempV);
			}
		}
		if (owner.trait.RenderExtra)
		{
			owner.trait.OnRenderExtra(p);
		}
		bool CanBeHallucinated()
		{
			if (!usePass || !owner.ExistsOnMap || !owner.trait.CanBeHallucinated)
			{
				return false;
			}
			if (owner.isThing)
			{
				if (owner.altitude != 0 || owner.TileType.UseMountHeight || owner.pos.LastThing != owner)
				{
					return false;
				}
				if (listTC.Count > 0)
				{
					return false;
				}
			}
			return true;
		}
	}

	public virtual void UpdatePosition(ref Vector3 destPos, RenderParam p)
	{
	}

	public virtual void DrawHeld()
	{
	}

	public SourcePref GetPref()
	{
		if (isChara)
		{
			if (owner.IsPCC || (owner.HasHost && owner.Chara.host.ride == owner))
			{
				return EClass.core.refs.prefs.pcc;
			}
			if (owner.idSkin > 0)
			{
				switch (owner.sourceCard.tiles.TryGet(owner.idSkin))
				{
				case 2319:
				case 2619:
				case 2621:
				case 2623:
				case 2625:
					return EClass.core.refs.prefs.sonwputit1;
				case 2320:
				case 2620:
				case 2622:
				case 2624:
				case 2626:
					return EClass.core.refs.prefs.snowputit2;
				}
			}
		}
		return owner.Pref;
	}

	public void RefreshSprite()
	{
		if (hasActor)
		{
			actor.RefreshSprite();
		}
	}

	public override void OnEnterScreen()
	{
		if (isSynced)
		{
			Debug.LogError("renderer alraedy synced:" + owner);
		}
		isSynced = true;
		if (!usePass)
		{
			if (!hasActor)
			{
				actor = data.CreateActor();
				if (actor == null)
				{
					if (owner.sourceCard.idActor.IsEmpty())
					{
						actor = PoolManager.Spawn(Resources.Load<CardActor>("Scene/Render/Actor/" + (owner.isChara ? "CharaActor" : "ThingActor")));
					}
					else
					{
						actor = PoolManager.Spawn(Resources.Load<CardActor>("Scene/Render/Actor/" + owner.sourceCard.idActor[0]));
					}
				}
				hasActor = true;
			}
			actor.SetOwner(owner);
		}
		RefreshStateIcon();
		RefreshExtra();
		if (owner.isCensored)
		{
			SetCensored(enable: true);
		}
		owner.trait.OnEnterScreen();
	}

	public void AddExtra(string id)
	{
		if (GetTC(id) == null)
		{
			TC tC = AddTC<TC>(PoolManager.Spawn<TC>("tcExtra_" + id, "Scene/Render/Actor/Component/Extra/" + id));
			if (tC.isUI)
			{
				tC.transform.SetParent(EClass.ui.rectDynamic, worldPositionStays: false);
			}
			tC.name = id;
		}
	}

	public void RefreshExtra()
	{
		string idExtra = owner.sourceCard.idExtra;
		if (!idExtra.IsEmpty())
		{
			AddExtra(idExtra);
		}
		if (isChara && owner.rarity >= Rarity.Legendary && owner.rarity != Rarity.Artifact && !owner.Chara.IsHomeMember())
		{
			if (owner.c_bossType == BossType.Evolved)
			{
				AddExtra("c_unique_evolved");
			}
			else
			{
				AddExtra("c_unique");
			}
		}
	}

	public void RemoveExtra(string id)
	{
		TC tC = GetTC(id);
		if (tC != null)
		{
			RemoveTC(tC);
		}
	}

	public override void OnLeaveScreen()
	{
		isSynced = false;
		if (hasActor && (!owner.ExistsOnMap || !data.persistActor))
		{
			KillActor();
		}
		for (int num = listTC.Count - 1; num >= 0; num--)
		{
			RemoveTC(num);
		}
		if (hasText)
		{
			DespawnSimpleText();
		}
		if ((bool)orbit)
		{
			PoolManager.Despawn(orbit);
			orbit = null;
		}
	}

	public void KillActor()
	{
		actor.Kill();
		hasActor = false;
		actor = null;
	}

	public void PlayAnime(AnimeID id, bool force)
	{
		PlayAnime(id, default(Vector3), force);
	}

	public void PlayAnime(AnimeID id, Card dest)
	{
		PlayAnime(id, (dest.renderer.position - position).normalized);
	}

	public void PlayAnime(AnimeID id, Point dest)
	{
		PlayAnime(id, (dest.PositionAuto() - position).normalized);
	}

	public void PlayAnime(AnimeID id, Vector3 dest = default(Vector3), bool force = false)
	{
		if (!force && owner.parent is Zone && (!owner.pos.IsValid || !owner.pos.IsSync))
		{
			return;
		}
		TransAnimeData transAnimeData = ResourceCache.Load<TransAnimeData>("Scene/Render/Anime/" + id);
		if (transAnimeData == null)
		{
			anime = null;
			return;
		}
		anime = new TransAnime
		{
			data = transAnimeData,
			renderer = this
		}.Init();
		if (id == AnimeID.Attack || (uint)(id - 18) <= 1u)
		{
			anime.dest = dest;
		}
	}

	public virtual void KillAnime()
	{
		anime = null;
	}

	public virtual void IdleFrame()
	{
		if (hasActor)
		{
			actor.IdleFrame();
		}
	}

	public virtual void NextFrame()
	{
		if (hasActor)
		{
			actor.NextFrame();
		}
	}

	public virtual void NextDir()
	{
		if (hasActor)
		{
			actor.NextDir();
		}
	}

	public T AddTC<T>(TC tc) where T : TC
	{
		listTC.Add(tc);
		tc.SetOwner(this);
		return tc as T;
	}

	public T GetTC<T>() where T : TC
	{
		if (listTC.Count == 0)
		{
			return null;
		}
		for (int num = listTC.Count - 1; num >= 0; num--)
		{
			if (listTC[num] is T)
			{
				return listTC[num] as T;
			}
		}
		return null;
	}

	public TC GetTC(string id)
	{
		if (listTC.Count == 0)
		{
			return null;
		}
		for (int num = listTC.Count - 1; num >= 0; num--)
		{
			if (listTC[num].name == id)
			{
				return listTC[num];
			}
		}
		return null;
	}

	public T GetOrCreateTC<T>(Func<T> func) where T : TC
	{
		return GetTC<T>() ?? AddTC<T>(func());
	}

	public void RemoveTC<T>() where T : TC
	{
		if (listTC.Count == 0)
		{
			return;
		}
		for (int num = listTC.Count - 1; num >= 0; num--)
		{
			if (listTC[num] is T)
			{
				listTC[num].Kill();
				listTC.RemoveAt(num);
				break;
			}
		}
	}

	public void RemoveTC(TC tc)
	{
		RemoveTC(listTC.IndexOf(tc));
	}

	public void RemoveTC(int index)
	{
		if (index != -1)
		{
			listTC[index].Kill();
			listTC.RemoveAt(index);
		}
	}

	public void TrySpawnOrbit()
	{
		if (owner.isChara)
		{
			string text = (owner.IsPC ? "tcOrbitPC" : "tcOrbitChara");
			orbit = PoolManager.Spawn<TCOrbitChara>(text, "Scene/Render/Actor/Component/" + text);
			orbit.SetOwner(this);
		}
		else if (owner.trait.ShowOrbit)
		{
			string text2 = "tcOrbitThing";
			orbit = PoolManager.Spawn<TCOrbitThing>(text2, "Scene/Render/Actor/Component/" + text2);
			orbit.SetOwner(this);
		}
	}

	public bool IsTalking()
	{
		TCText tC = GetTC<TCText>();
		if ((bool)tC)
		{
			return tC.pop.items.Count > 0;
		}
		return false;
	}

	public TCSimpleText SpawnSimpleText()
	{
		if (!hasText)
		{
			hasText = true;
			simpleText = PoolManager.Spawn(EClass.core.refs.tcs.simpleText, EClass.screen);
		}
		RenderObject.tempV.x = position.x + TC._setting.simpleTextPos.x;
		RenderObject.tempV.y = position.y + TC._setting.simpleTextPos.y;
		RenderObject.tempV.z = position.z + TC._setting.simpleTextPos.z;
		simpleText.transform.position = RenderObject.tempV;
		return simpleText;
	}

	public void DespawnSimpleText()
	{
		if (hasText)
		{
			PoolManager.Despawn(simpleText);
			hasText = false;
		}
	}

	public void SetCensored(bool enable)
	{
		if (!isSynced)
		{
			return;
		}
		if (enable)
		{
			GetOrCreateTC(() => PoolManager.Spawn(EClass.core.refs.tcs.censored));
		}
		else
		{
			RemoveTC<TCCensored>();
		}
	}

	public virtual void RefreshStateIcon()
	{
	}

	public void Say(string text, Color c = default(Color), float duration = 0f)
	{
		(GetTC<TCText>() ?? AddTC<TCText>(PoolManager.Spawn(EClass.core.refs.tcs.text, EClass.ui.rectDynamic))).Say(text, duration);
	}

	public void ShowEmo(Emo emo, float duration = 0f)
	{
		if (isSynced)
		{
			(GetTC<TCText>() ?? AddTC<TCText>(PoolManager.Spawn(EClass.core.refs.tcs.text, EClass.ui.rectDynamic))).ShowEmo(emo, duration);
		}
	}

	public virtual void SetFirst(bool first, Vector3 pos)
	{
	}

	public virtual void SetFirst(bool first)
	{
	}

	public virtual void Refresh()
	{
	}
}
