using UnityEngine;

public class ActorEx : Actor
{
	public enum Type
	{
		Default,
		JukeBox
	}

	public Type type;

	public Card owner;

	public bool alwaysOn;

	public float outsideRoomVolume = 0.5f;

	public float outsideLotVolume = 0.5f;

	public int maxDistance;

	public int minInterval = 1;

	public int randomInterval;

	private int next;

	public AudioSource audioSource;

	public SoundData data;

	public void SetOwner(Card c)
	{
		owner = c;
		if (type == Type.JukeBox)
		{
			data = EMono.core.refs.dictBGM.TryGetValue(owner.refVal);
		}
		if ((bool)audioSource)
		{
			audioSource.clip = data.clip;
			audioSource.pitch = data.pitch * (1f + ((data.randomPitch == 0f) ? 0f : Rand.Range(0f - data.randomPitch, data.randomPitch)));
		}
		Refresh();
	}

	public float GetVolume()
	{
		if (!EMono._zone.isStarted || !owner.IsInstalled)
		{
			return 0f;
		}
		Room room = EMono.pc.Cell.room;
		Room room2 = owner.Cell.room;
		bool flag = room2 == null || room2.data.atrium || !room2.HasRoof;
		bool flag2 = room == room2 || (room2 == null && (room.data.atrium || !room.HasRoof));
		bool num = flag2 || room?.lot == room2?.lot;
		float num2 = ((alwaysOn || owner.isOn) ? 1f : 0f);
		if (!num)
		{
			num2 *= outsideLotVolume;
		}
		if (!flag2 && room2 != null)
		{
			num2 *= outsideRoomVolume;
		}
		if (num2 <= 0f)
		{
			return 0f;
		}
		if (type == Type.JukeBox)
		{
			num2 *= 0.5f + (flag ? 0.25f : 0f);
			if (flag2)
			{
				int num3 = EMono.pc.Dist(owner);
				if (num3 < 8)
				{
					float num4 = (float)(8 - num3) * SoundManager.current.jukeboxMod - 0.2f;
					if (num4 < SoundManager.bgmDumpMod)
					{
						SoundManager.bgmDumpMod = num4;
					}
				}
			}
		}
		return num2;
	}

	public void Refresh()
	{
		if (owner.parent != EMono.pc.currentZone)
		{
			Debug.LogWarning(owner);
			return;
		}
		next--;
		if (next > 0)
		{
			return;
		}
		next = minInterval + Rand.rnd(randomInterval + 1);
		if (maxDistance != 0)
		{
			int num = EMono.pc.Dist(owner);
			if (base.gameObject.activeSelf)
			{
				if (num > maxDistance)
				{
					base.gameObject.SetActive(value: false);
					return;
				}
			}
			else
			{
				if (num > maxDistance)
				{
					return;
				}
				base.gameObject.SetActive(value: true);
			}
		}
		float volume = GetVolume();
		if (maxDistance != 0)
		{
			base.gameObject.SetActive(volume > 0f);
		}
		if ((bool)audioSource)
		{
			audioSource.volume = data.volume * volume;
			if (!audioSource.isPlaying && volume > 0f)
			{
				audioSource.Play();
			}
		}
		if (!(volume <= 0f))
		{
			Vector3 vector = owner.pos.PositionCenter();
			vector.z = 0f;
			vector.x -= 0.64f;
			if ((bool)audioSource)
			{
				base.transform.position = vector;
			}
			else
			{
				SoundManager.current.Play(data, vector, volume);
			}
		}
	}

	public void Kill()
	{
		Object.Destroy(base.gameObject);
	}
}
