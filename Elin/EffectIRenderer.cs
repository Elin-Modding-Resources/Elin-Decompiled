using UnityEngine;

public class EffectIRenderer : Effect
{
	public Card card;

	public Card origin;

	public Point from;

	public float snapInterval;

	public bool follow;

	public TransAnimeData animeData;

	public TransAnime anime;

	private float snapTimer;

	private Vector3 v;

	public EffectIRenderer Play(Card _card, Card _origin, Vector3 _to)
	{
		if (!EMono.core.IsGameStarted)
		{
			Kill();
			return null;
		}
		card = _card;
		origin = _origin;
		destV = _to;
		from = _origin.pos.Copy();
		snapTimer = -0.01f;
		Activate();
		OnUpdate();
		if ((bool)animeData)
		{
			anime = new TransAnime
			{
				data = animeData,
				renderer = card.renderer
			}.Init();
		}
		return this;
	}

	public EffectIRenderer Play(Card origin, Card _card, Point from, Point to = null, float fixY = 0f)
	{
		card = _card;
		snapTimer = -0.01f;
		this.from = from.Copy();
		if (origin.ExistsOnMap)
		{
			_Play(from, origin.renderer.position, fixY, to);
		}
		else
		{
			Play(from, fixY, to);
		}
		if ((bool)animeData)
		{
			anime = new TransAnime
			{
				data = animeData,
				renderer = card.renderer
			}.Init();
		}
		return this;
	}

	public override void OnUpdate()
	{
		RenderParam renderParam = card.GetRenderParam();
		renderParam.color = EMono.scene.screenElin.tileMap.GetApproximateBlocklight(from.cell);
		timer += Core.delta;
		snapTimer -= Core.delta;
		anime?.Update();
		if (snapTimer < 0f || v == Vector3.zero)
		{
			if (follow)
			{
				if (origin.renderer != null)
				{
					Vector3 position = origin.renderer.position;
					Vector3 vector = destV * timer / duration * speed;
					v.x = position.x + posFix.x + vector.x;
					v.y = position.y + posFix.y + vector.y;
					v.z = position.z + posFix.z + vector.z;
				}
			}
			else
			{
				v.x = base.transform.position.x;
				v.y = base.transform.position.y;
				v.z = base.transform.position.z;
			}
			if (anime != null)
			{
				v.x += anime.v.x;
				v.y += anime.v.y;
				v.z += anime.v.z;
			}
			snapTimer += snapInterval;
		}
		renderParam.x = v.x;
		renderParam.y = v.y;
		renderParam.z = v.x;
		card.renderer.skip = true;
		if (card.renderer.usePass)
		{
			card.renderer.data.Draw(renderParam);
			return;
		}
		if (!card.renderer.hasActor)
		{
			card.renderer.OnEnterScreen();
		}
		if ((bool)card.renderer.actor)
		{
			card.renderer.actor.OnRender(renderParam);
			card.renderer.isSynced = false;
		}
	}

	public override void OnKill()
	{
		if (card != null && card.renderer.hasActor)
		{
			card.renderer.KillActor();
		}
	}
}
