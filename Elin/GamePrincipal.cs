using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class GamePrincipal : EClass
{
	public enum Type
	{
		Oath,
		Workaround,
		Legacy
	}

	public class Item
	{
		public string id;

		public Type type;

		public Func<bool> _get;

		public Action<bool> _set;

		public int grade;

		public bool IsEmbark
		{
			get
			{
				if (EClass.core.IsGameStarted)
				{
					return EClass.player.resetPrincipal;
				}
				return true;
			}
		}

		public bool Get()
		{
			return _get();
		}

		public void Set(bool value)
		{
			_set(value);
		}

		public bool IsModified()
		{
			return Get() != EClass.game.principal.GetField<bool>(id);
		}

		public bool WasSealed()
		{
			if (EClass.game.principal.modified.Contains(id) || IsEmbark)
			{
				return false;
			}
			bool num = type == Type.Oath;
			bool field = EClass.game.principal.GetField<bool>(id);
			if (num)
			{
				return field;
			}
			return !field;
		}

		public bool IsSealed()
		{
			if (EClass.game.principal.modified.Contains(id))
			{
				return false;
			}
			bool num = type == Type.Oath;
			bool flag = Get();
			bool flag2 = (IsEmbark ? flag : EClass.game.principal.GetField<bool>(id));
			if (num)
			{
				return flag2 && flag;
			}
			if (!flag2)
			{
				return !flag;
			}
			return false;
		}
	}

	public class ItemSlider : Item
	{
		public int max;

		public Func<int> _getInt;

		public Action<int> _setInt;

		public Func<int, string> funcText;

		public int GetInt()
		{
			return _getInt();
		}

		public void SetInt(int value)
		{
			_setInt(value);
		}
	}

	[JsonProperty]
	public int id;

	[JsonProperty]
	public int socre;

	[JsonProperty]
	public int dropRateMtp;

	[JsonProperty]
	public int petFeatExpMtp;

	[JsonProperty]
	public bool ignoreEvaluate;

	[JsonProperty]
	public bool disableDeathPenaltyProtection;

	[JsonProperty]
	public bool tax;

	[JsonProperty]
	public bool opMilk;

	[JsonProperty]
	public bool disableManualSave;

	[JsonProperty]
	public bool permadeath;

	[JsonProperty]
	public bool infiniteMarketFund;

	[JsonProperty]
	public bool disableUsermapBenefit;

	[JsonProperty]
	public bool realAdv;

	[JsonProperty]
	public bool dropRate;

	[JsonProperty]
	public bool petFeatExp;

	[JsonProperty]
	public bool disableVoidBlessing;

	[JsonProperty]
	public bool enableDamageReduction;

	[JsonProperty]
	public HashSet<string> modified = new HashSet<string>();

	public bool IsCustom => id == -1;

	public List<Item> ListItems()
	{
		List<Item> list = new List<Item>();
		Add(1, Type.Oath, "disableManualSave", () => disableManualSave, delegate(bool a)
		{
			disableManualSave = a;
		});
		Add(1, Type.Oath, "disableDeathPenaltyProtection", () => disableDeathPenaltyProtection, delegate(bool a)
		{
			disableDeathPenaltyProtection = a;
		});
		Add(1, Type.Oath, "disableUsermapBenefit", () => disableUsermapBenefit, delegate(bool a)
		{
			disableUsermapBenefit = a;
		});
		Add(3, Type.Oath, "permadeath", () => permadeath, delegate(bool a)
		{
			permadeath = a;
		});
		Add(2, Type.Workaround, "disableVoidBlessing", () => disableVoidBlessing, delegate(bool a)
		{
			disableVoidBlessing = a;
		});
		AddSlider(2, Type.Workaround, "dropRate", () => dropRate, delegate(bool a)
		{
			dropRate = a;
		}, () => dropRateMtp, delegate(int a)
		{
			dropRateMtp = a;
		}, (int a) => 0.5f + 0.5f * (float)a + "x", 5);
		AddSlider(2, Type.Workaround, "petFeatExp", () => petFeatExp, delegate(bool a)
		{
			petFeatExp = a;
		}, () => petFeatExpMtp, delegate(int a)
		{
			petFeatExpMtp = a;
		}, (int a) => 0.5f + 0.5f * (float)a + "x", 3);
		Add(-1, Type.Legacy, "enableDamageReduction", () => enableDamageReduction, delegate(bool a)
		{
			enableDamageReduction = a;
		});
		return list;
		void Add(int grade, Type type, string id, Func<bool> _get, Action<bool> _set)
		{
			list.Add(new Item
			{
				type = type,
				grade = grade,
				id = id,
				_get = _get,
				_set = _set
			});
		}
		void AddSlider(int grade, Type type, string id, Func<bool> _get, Action<bool> _set, Func<int> _getInt, Action<int> _setInt, Func<int, string> funcText, int max)
		{
			list.Add(new ItemSlider
			{
				type = type,
				grade = grade,
				id = id,
				_get = _get,
				_set = _set,
				_getInt = _getInt,
				_setInt = _setInt,
				funcText = funcText,
				max = max
			});
		}
	}
}
