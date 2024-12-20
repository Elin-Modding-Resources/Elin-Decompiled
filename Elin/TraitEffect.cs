using Newtonsoft.Json;
using UnityEngine;

public class TraitEffect : TraitItem
{
	public class Data
	{
		[JsonProperty]
		public int sprite;

		[JsonProperty]
		public float interval;

		[JsonProperty]
		public float delay;

		[JsonProperty]
		public SerializableColor color = new SerializableColor(0, 0, 0, 0);
	}

	public float timer;

	public Data _data;

	public float Interval
	{
		get
		{
			if (data.interval != 0f)
			{
				return data.interval;
			}
			return 3f;
		}
	}

	public int id
	{
		get
		{
			return owner.refVal;
		}
		set
		{
			owner.refVal = value;
		}
	}

	public Effect Effect => EClass.core.refs.fireworks[id % EClass.core.refs.fireworks.Count];

	public virtual string Path => "";

	public override bool UseExtra => owner.isOn;

	public Data data
	{
		get
		{
			if (_data == null)
			{
				_data = owner.GetObj<Data>(7);
				if (_data == null)
				{
					_data = new Data();
					owner.SetObj(7, _data);
				}
			}
			return _data;
		}
		set
		{
			owner.SetObj(7, value);
		}
	}

	public void Proc(Vector3 v = default(Vector3))
	{
		Effect effect = Effect.Get(Path);
		if (data.color.Get().a != 0f)
		{
			effect.SetParticleColor(data.color.Get());
		}
		if (data.sprite != 0)
		{
			Sprite sprite = effect.sprites[data.sprite % effect.sprites.Length];
			ParticleSystem[] componentsInChildren = effect.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.ShapeModule shape = componentsInChildren[i].shape;
				if (shape.shapeType == ParticleSystemShapeType.Sprite && (bool)shape.sprite)
				{
					shape.sprite = sprite;
					shape.texture = sprite.texture;
				}
			}
		}
		if (effect.dirs.Length != 0)
		{
			effect.transform.localEulerAngles = effect.dirs[owner.dir % effect.dirs.Length];
		}
		effect.Play((!(v == default(Vector3))) ? v : ((owner.parent == EClass._zone) ? owner.renderer.position : EClass.pc.renderer.position));
	}

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (p.input != ActInput.AllAction)
		{
			return;
		}
		if (Effect.sprites.Length != 0)
		{
			p.TrySetAct("actChangeSymbol", delegate
			{
				UIContextMenu uIContextMenu4 = EClass.ui.CreateContextMenuInteraction();
				uIContextMenu4.AddSlider("adjustment", (float a) => ((int)a).ToString() ?? "", data.sprite, delegate(float b)
				{
					data.sprite = (int)b;
				}, 0f, Effect.sprites.Length - 1, isInt: true, hideOther: false);
				uIContextMenu4.Show();
				return false;
			}, owner);
		}
		if (Effect.systems.Length != 0)
		{
			p.TrySetAct("actChangeColor", delegate
			{
				EClass.ui.AddLayer<LayerColorPicker>().SetColor(data.color.Get(), new Color(0f, 0f, 0f, 0f), delegate(PickerState state, Color _c)
				{
					data.color.Set(_c);
				});
				return false;
			}, owner);
		}
		p.TrySetAct("actChangeInterval", delegate
		{
			UIContextMenu uIContextMenu3 = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu3.AddSlider("adjustment", (float a) => (0.01f * (float)(int)(a * 10f)).ToString() ?? "", data.interval * 10f, delegate(float b)
			{
				data.interval = b * 0.1f;
			}, 0f, 200f, isInt: true, hideOther: false);
			uIContextMenu3.Show();
			return false;
		}, owner);
		p.TrySetAct("actChangeDelay", delegate
		{
			UIContextMenu uIContextMenu2 = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu2.AddSlider("adjustment", (float a) => (0.01f * (float)(int)(a * 10f)).ToString() ?? "", data.delay * 10f, delegate(float b)
			{
				data.delay = b * 0.1f;
			}, 0f, 200f, isInt: true, hideOther: false);
			uIContextMenu2.Show();
			return false;
		}, owner);
		if (!EClass.debug.enable)
		{
			return;
		}
		p.TrySetAct("actChangeType", delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddSlider("adjustment", (float a) => a.ToString() ?? "", id, delegate(float b)
			{
				id = (int)b;
				Proc();
			}, 0f, EClass.core.refs.fireworks.Count - 1, isInt: true, hideOther: false);
			uIContextMenu.Show();
			return false;
		}, owner);
	}

	public override int CompareTo(Card b)
	{
		return b.refVal - owner.refVal;
	}
}
