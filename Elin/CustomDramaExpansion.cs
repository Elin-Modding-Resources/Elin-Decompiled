using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class CustomDramaExpansion : EClass
{
	private static readonly Dictionary<string, DramaActionParser> _actionParsers = new Dictionary<string, DramaActionParser>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, DramaInvokeDetail> _invokes = new Dictionary<string, DramaInvokeDetail>(StringComparer.OrdinalIgnoreCase);

	internal static readonly Regex _expressionRegex = new Regex("^\\s*(?<f>\\w+)\\s*(?:\\(\\s*(?<p>.*)\\s*\\))?\\s*$", RegexOptions.Compiled);

	public static bool ParseAction(string action, DramaManager dm, Dictionary<string, string> line)
	{
		if (_actionParsers.TryGetValue(action, out var value))
		{
			return value(dm, line);
		}
		return false;
	}

	public static void AddDramaActionParser(string action, DramaActionParser parser)
	{
		_actionParsers[action] = SafeInvoke;
		Debug.Log("#drama added new action parser '" + action + "' from '" + parser.Method.TryToString() + "'");
		bool SafeInvoke(DramaManager dm, Dictionary<string, string> line)
		{
			try
			{
				return parser(dm, line);
			}
			catch (Exception ex)
			{
				ModUtil.LogModError("exception while parsing drama action '" + action + "'\n" + ex.Message, parser.Method.DeclaringType);
				Debug.LogException(ex);
			}
			return false;
		}
	}

	public static void AddDramaInvokeMethod(string name, MethodInfo method, string contract = null)
	{
		_invokes[name] = new DramaInvokeDetail(method, contract);
	}

	public static void AddDramaInvokeMethod(string name, DramaInvokeFunc func, string contract = null)
	{
		AddDramaInvokeMethod(name, func.Method, contract);
	}

	internal static (DramaInvokeDetail invoke, string[] parameters) BuildInvokeExpression(string expression)
	{
		(string, string[]) tuple = ParseInvokeExpression(expression);
		if (!tuple.Item1.IsEmpty() && _invokes.TryGetValue(tuple.Item1, out var value))
		{
			return (invoke: value, parameters: tuple.Item2);
		}
		if (expression.Length <= 1)
		{
			return (invoke: new DramaInvokeDetail(null), parameters: Array.Empty<string>());
		}
		return expression[0] switch
		{
			'!' => BuildInvokeExpression("not(" + expression[1..] + ")"), 
			'&' => BuildInvokeExpression("and(" + expression[1..] + ")"), 
			'?' => BuildInvokeExpression("or(" + expression[1..] + ")"), 
			_ => (invoke: new DramaInvokeDetail(null), parameters: Array.Empty<string>()), 
		};
	}

	internal static (string funcName, string[] parameters) ParseInvokeExpression(string expression)
	{
		if (expression.IsEmpty())
		{
			return (funcName: "", parameters: Array.Empty<string>());
		}
		Match match = _expressionRegex.Match(expression);
		return (funcName: match.Groups["f"].Value, parameters: SplitParams(match.Groups["p"].Value));
		static string[] SplitParams(string args)
		{
			if (args.IsEmpty())
			{
				return Array.Empty<string>();
			}
			List<string> list = new List<string>();
			int num = 0;
			int num2 = 0;
			char c = '\0';
			for (int i = 0; i < args.Length; i++)
			{
				char c2 = args[i];
				if (c != 0)
				{
					if (c2 == c)
					{
						c = '\0';
					}
				}
				else
				{
					switch (c2)
					{
					case '"':
					case '\'':
						c = c2;
						break;
					case '(':
						num++;
						break;
					case ')':
						num--;
						break;
					case ',':
						if (num == 0)
						{
							string s2 = args[num2..i].Trim();
							s2 = Unquote(s2);
							list.Add(s2);
							num2 = i + 1;
						}
						break;
					}
				}
			}
			if (num2 < args.Length)
			{
				string text = args[num2..].Trim();
				if (text.Length > 0)
				{
					list.Add(Unquote(text));
				}
			}
			return list.ToArray();
		}
		static string Unquote(string s)
		{
			if (s.Length >= 2 && ((s[0] == '"' && s[^1] == '"') || (s[0] == '\'' && s[^1] == '\'')))
			{
				return s[1..^1];
			}
			return s;
		}
	}

	[ElinDramaActionParser("i*")]
	[ElinDramaActionParser("invoke*")]
	internal static bool DefaultDramaInvokeExtActionParser(DramaManager dm, Dictionary<string, string> line)
	{
		string text = line["param"].Trim().RemoveNewline();
		if (text.StartsWith("//"))
		{
			return true;
		}
		var (invoke, parameters) = BuildInvokeExpression(text);
		if (invoke.Method == null)
		{
			ModUtil.LogModError("invalid drama invoke* expression '" + text + "'", new FileInfo(dm.path));
			return true;
		}
		string jump = line["jump"];
		if (!jump.IsEmpty())
		{
			dm.AddEvent(new DramaEventMethod(null)
			{
				jumpFunc = () => (!invoke.SafeInvoke(dm, line, parameters)) ? "" : jump
			});
			return true;
		}
		dm.AddEvent(new DramaEventMethod(delegate
		{
			invoke.SafeInvoke(dm, line, parameters);
		}));
		return true;
	}

	[ElinDramaActionParser("eval")]
	internal static bool DefaultDramaEvalActionParser(DramaManager dm, Dictionary<string, string> line)
	{
		string expr = line["param"];
		if (expr.IsEmpty())
		{
			return true;
		}
		EScriptSubmission submission = EScriptSubmission.Create(dm.setup.book);
		EDramaScriptState state = new EDramaScriptState
		{
			dm = dm,
			line = line
		};
		string jump = line["jump"];
		bool flag = !jump.IsEmpty();
		bool flag2 = !line["id"].IsEmpty();
		if (!flag && flag2)
		{
			Dictionary<string, string> item = new Dictionary<string, string>(line)
			{
				["action"] = "",
				["param"] = ""
			};
			dm.ParseLine(item);
			dm.lastTalk.activeCondition = delegate
			{
				object obj2 = DeferredCompileAndRun();
				return obj2 is bool && (bool)obj2;
			};
			return true;
		}
		if (flag)
		{
			dm.AddEvent(new DramaEventMethod(null)
			{
				jumpFunc = delegate
				{
					object obj = DeferredCompileAndRun();
					if (obj is string result)
					{
						return result;
					}
					return (obj is bool && !(bool)obj) ? "" : jump;
				}
			});
			return true;
		}
		dm.AddEvent(new DramaEventMethod(delegate
		{
			DeferredCompileAndRun();
		}));
		return true;
		object DeferredCompileAndRun()
		{
			if (expr.StartsWith("<<<"))
			{
				string text = expr[3..].Trim();
				string path = Path.Combine(Path.GetDirectoryName(dm.path), text);
				if (!File.Exists(path))
				{
					throw new FileNotFoundException(text);
				}
				expr = File.ReadAllText(path);
			}
			return submission.Compile<EDramaScriptState>(expr)?.Invoke(state);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ElinDramaActionInvoke("nodiscard,passthrough")]
	public static bool eval(DramaManager dm, Dictionary<string, string> line, params string[] parameters)
	{
		string text = string.Join(',', parameters);
		if (text.IsEmpty())
		{
			return false;
		}
		EScriptSubmission eScriptSubmission = EScriptSubmission.Create(dm.setup.book);
		EDramaScriptState arg = new EDramaScriptState
		{
			dm = dm,
			line = line
		};
		if (text.StartsWith("<<<"))
		{
			string text2 = text[3..].Trim();
			string path = Path.Combine(Path.GetDirectoryName(dm.path), text2);
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(text2);
			}
			text = File.ReadAllText(path);
		}
		object obj = eScriptSubmission.Compile<EDramaScriptState>(text)?.Invoke(arg);
		if (obj is bool)
		{
			return (bool)obj;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ElinDramaActionInvoke("nodiscard")]
	public static bool and(DramaManager dm, Dictionary<string, string> line, params string[] parameters)
	{
		return parameters.All(delegate(string expr)
		{
			var (dramaInvokeDetail, parameters2) = BuildInvokeExpression(expr);
			return dramaInvokeDetail.SafeInvoke(dm, line, parameters2);
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ElinDramaActionInvoke("nodiscard")]
	public static bool or(DramaManager dm, Dictionary<string, string> line, params string[] parameters)
	{
		return parameters.Any(delegate(string expr)
		{
			var (dramaInvokeDetail, parameters2) = BuildInvokeExpression(expr);
			return dramaInvokeDetail.SafeInvoke(dm, line, parameters2);
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ElinDramaActionInvoke("nodiscard")]
	public static bool not(DramaManager dm, Dictionary<string, string> line, params string[] parameters)
	{
		return parameters.All(delegate(string expr)
		{
			var (dramaInvokeDetail, parameters2) = BuildInvokeExpression(expr);
			return !dramaInvokeDetail.SafeInvoke(dm, line, parameters2);
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ElinDramaActionInvoke(null)]
	public static bool console_cmd(DramaManager dm, Dictionary<string, string> line, params string[] parameters)
	{
		string.Join(' ', parameters).EvaluateAsCommand();
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool add_item(DramaManager dm, Dictionary<string, string> line, string itemId, string materialAlias = "wood", int lv = -1, int count = 1)
	{
		Chara chara = dm.GetChara(line["actor"]);
		SourceMaterial.Row row = EClass.sources.materials.alias.TryGetValue(materialAlias, "wood");
		Thing t = ThingGen.Create(itemId, row.id, lv).SetNum(count);
		chara.Pick(t);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool apply_condition(DramaManager dm, Dictionary<string, string> line, string conditionAlias, int power = 100)
	{
		dm.GetChara(line["actor"]).AddCondition(conditionAlias, power, force: true);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool equip_item(DramaManager dm, Dictionary<string, string> line, string itemId)
	{
		dm.GetChara(line["actor"]).body.Equip(ThingGen.Create(itemId));
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool destroy_item(DramaManager dm, Dictionary<string, string> line, string itemId, int count = -1)
	{
		List<Thing> list = dm.GetChara(line["actor"]).things.List((Thing t) => t.id == itemId);
		if (count < 0)
		{
			foreach (Thing item in list)
			{
				item.Destroy();
			}
		}
		else
		{
			count = Math.Max(count, 0);
			foreach (Thing item2 in list)
			{
				if (count == 0)
				{
					break;
				}
				if (item2.Num >= count)
				{
					item2.ModNum(-count);
					continue;
				}
				count -= item2.Num;
				item2.Destroy();
			}
		}
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool remove_condition(DramaManager dm, Dictionary<string, string> line, string conditionAlias)
	{
		foreach (Condition item in dm.GetChara(line["actor"]).conditions.ToList())
		{
			if (item.source.alias == conditionAlias)
			{
				item.Kill();
			}
		}
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool join_faith(DramaManager dm, Dictionary<string, string> line, string religionId = "")
	{
		Chara chara = dm.GetChara(line["actor"]);
		if (religionId.IsEmpty())
		{
			chara.faith?.LeaveFaith(chara, EClass.game.religions.Eyth, Religion.ConvertType.Default);
		}
		else
		{
			EClass.game.religions.Find(religionId)?.JoinFaith(chara);
		}
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool join_party(DramaManager dm, Dictionary<string, string> line)
	{
		Chara chara = dm.GetChara(line["actor"]);
		EClass.Sound.Play("good");
		chara.MakeAlly();
		return true;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_affinity(DramaManager dm, Dictionary<string, string> line, DramaValueExpression expr)
	{
		Chara chara = dm.GetChara(line["actor"]);
		return expr.Compare(chara._affinity);
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_condition(DramaManager dm, Dictionary<string, string> line, string conditionAlias, DramaValueExpression expr = null)
	{
		if (expr == null)
		{
			expr = ">=1";
		}
		foreach (Condition condition in dm.GetChara(line["actor"]).conditions)
		{
			if (condition.source.alias == conditionAlias)
			{
				return expr.Compare(condition.value);
			}
		}
		return false;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_currency(DramaManager dm, Dictionary<string, string> line, string currencyId, DramaValueExpression expr)
	{
		Chara chara = dm.GetChara(line["actor"]);
		return expr.Compare(chara.GetCurrency(currencyId));
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_element(DramaManager dm, Dictionary<string, string> line, string elementAlias, DramaValueExpression expr)
	{
		Chara chara = dm.GetChara(line["actor"]);
		if (chara.HasElement(elementAlias))
		{
			return expr.Compare(chara.Evalue(elementAlias));
		}
		return false;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_faith(DramaManager dm, Dictionary<string, string> line, string religionId, DramaValueExpression expr = null)
	{
		if (expr == null)
		{
			expr = ">=0";
		}
		Religion faith = dm.GetChara(line["actor"]).faith;
		if (faith.id == religionId)
		{
			return expr.Compare(faith.giftRank);
		}
		return false;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_fame(DramaManager dm, Dictionary<string, string> line, DramaValueExpression expr)
	{
		return expr.Compare(EClass.player.fame);
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_flag(DramaManager dm, Dictionary<string, string> line, string flagKey, DramaValueExpression expr = null)
	{
		if (expr == null)
		{
			expr = ">=1";
		}
		Chara chara = dm.GetChara(line["actor"]);
		return expr.Compare(chara.GetInt(flagKey));
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_has_item(DramaManager dm, Dictionary<string, string> line, string itemId, DramaValueExpression expr = null)
	{
		if (expr == null)
		{
			expr = ">=1";
		}
		Chara chara = dm.GetChara(line["actor"]);
		return expr.Compare(chara.things.List((Thing t) => t.id == itemId).Count);
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_hostility(DramaManager dm, Dictionary<string, string> line, string expression)
	{
		Chara chara = dm.GetChara(line["actor"]);
		Match match = Regex.Match(expression, "^(?<op>>=|<=|>|<|=|!=|==)?(?<h>.+)$");
		if (!match.Success)
		{
			throw new ArgumentException("invalid expression " + expression);
		}
		string value = match.Groups["op"].Value;
		if (!Enum.TryParse<Hostility>(match.Groups["h"].Value, ignoreCase: true, out var result))
		{
			throw new ArgumentException("invalid hostility " + match.Groups["h"].Value);
		}
		return new DramaValueExpression($"{value}{result:D}").Compare(chara._cints[4]);
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_in_party(DramaManager dm, Dictionary<string, string> line, bool isInParty = true)
	{
		return dm.GetChara(line["actor"]).IsPCParty == isInParty;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_keyitem(DramaManager dm, Dictionary<string, string> line, string keyitemId, DramaValueExpression expr = null)
	{
		if (expr == null)
		{
			expr = ">0";
		}
		if (EClass.sources.keyItems.alias.TryGetValue(keyitemId, out var value) && EClass.player.keyItems.TryGetValue(value.id, out var value2))
		{
			return expr.Compare(value2);
		}
		return false;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_lv(DramaManager dm, Dictionary<string, string> line, DramaValueExpression expr)
	{
		Chara chara = dm.GetChara(line["actor"]);
		return expr.Compare(chara.LV);
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_race(DramaManager dm, Dictionary<string, string> line, string raceId)
	{
		return dm.GetChara(line["actor"]).race.id == raceId;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_tag(DramaManager dm, Dictionary<string, string> line, string tag)
	{
		return dm.GetChara(line["actor"]).source.tag.Contains(tag);
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_zone(DramaManager dm, Dictionary<string, string> line, string zoneId, int zoneLv = 99999)
	{
		Zone currentZone = dm.GetChara(line["actor"]).currentZone;
		if (currentZone == null)
		{
			return false;
		}
		if (currentZone.id == zoneId)
		{
			if (zoneLv != 99999)
			{
				return currentZone.lv == zoneLv;
			}
			return true;
		}
		return false;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool if_zone_2(DramaManager dm, Dictionary<string, string> line, string zoneFullName)
	{
		Zone currentZone = dm.GetChara(line["actor"]).currentZone;
		Zone zone = ModUtil.FindZoneByFullName(zoneFullName);
		return currentZone == zone;
	}

	[ElinDramaActionInvoke(null)]
	public static bool mod_affinity(DramaManager dm, Dictionary<string, string> line, DramaValueExpression expr)
	{
		Chara chara = dm.GetChara(line["actor"]);
		chara.ModAffinity(EClass.pc, expr.Diff(chara._affinity));
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool mod_currency(DramaManager dm, Dictionary<string, string> line, string currencyId, DramaValueExpression expr)
	{
		Chara chara = dm.GetChara(line["actor"]);
		int currency = chara.GetCurrency(currencyId);
		chara.ModCurrency(expr.Diff(currency), currencyId);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool mod_element(DramaManager dm, Dictionary<string, string> line, string elementAlias, int value = 1, int potential = 100)
	{
		Chara chara = dm.GetChara(line["actor"]);
		Element orCreateElement = chara.elements.GetOrCreateElement(elementAlias);
		if (orCreateElement != null)
		{
			if (orCreateElement is Feat)
			{
				chara.SetFeat(orCreateElement.id, value, msg: true);
			}
			else
			{
				chara.elements.SetBase(orCreateElement.id, value, potential);
			}
		}
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool mod_element_exp(DramaManager dm, Dictionary<string, string> line, string elementAlias, DramaValueExpression expr)
	{
		Chara chara = dm.GetChara(line["actor"]);
		Element orCreateElement = chara.elements.GetOrCreateElement(elementAlias);
		if (orCreateElement == null)
		{
			return true;
		}
		chara.ModExp(orCreateElement.id, expr.Diff(orCreateElement.vExp));
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool mod_fame(DramaManager dm, Dictionary<string, string> line, DramaValueExpression expr)
	{
		EClass.player.ModFame(expr.Diff(EClass.player.fame));
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool mod_flag(DramaManager dm, Dictionary<string, string> line, string flagKey, DramaValueExpression expr = null)
	{
		if (expr == null)
		{
			expr = "=1";
		}
		Chara chara = dm.GetChara(line["actor"]);
		chara.SetInt(flagKey, expr.ModOrSet(chara.GetInt(flagKey)));
		return true;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool mod_keyitem(DramaManager dm, Dictionary<string, string> line, string keyitemId, DramaValueExpression expr = null)
	{
		if (!EClass.sources.keyItems.alias.TryGetValue(keyitemId, out var value))
		{
			return false;
		}
		if (expr == null)
		{
			expr = "=1";
		}
		Dictionary<int, int> keyItems = EClass.player.keyItems;
		keyItems.TryAdd(value.id, 0);
		int num = keyItems[value.id];
		int num2 = expr.ModOrSet(keyItems[value.id]);
		if (num < num2)
		{
			SE.Play("keyitem");
			Msg.Say("get_keyItem", value.GetName());
		}
		else if (num > num2)
		{
			SE.Play("keyitem_lose");
			Msg.Say("lose_keyItem", value.GetName());
		}
		keyItems[value.id] = num2;
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool move_next_to(DramaManager dm, Dictionary<string, string> line, string charaId)
	{
		Chara chara = dm.GetChara(line["actor"]);
		Chara chara2 = dm.GetChara(charaId);
		if (chara2 == null)
		{
			return false;
		}
		Point nearestPoint = chara2.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: true, ignoreCenter: true);
		chara.TryMove(nearestPoint, allowDestroyPath: false);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool move_tile(DramaManager dm, Dictionary<string, string> line, int xOffset, int yOffset)
	{
		Chara chara = dm.GetChara(line["actor"]);
		Point newPoint = chara.pos + new Point(xOffset, yOffset);
		chara.TryMove(newPoint, allowDestroyPath: false);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool move_to(DramaManager dm, Dictionary<string, string> line, int x, int y)
	{
		Chara chara = dm.GetChara(line["actor"]);
		Point newPoint = new Point(x, y);
		chara.TryMove(newPoint, allowDestroyPath: false);
		return true;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool move_zone(DramaManager dm, Dictionary<string, string> line, string zoneId, int zoneLv = 99999)
	{
		Chara chara = dm.GetChara(line["actor"]);
		if (zoneLv == 99999)
		{
			zoneLv = 0;
		}
		Zone zone = ModUtil.FindZoneByFullName($"{zoneId}@{zoneLv}");
		if (zone == null)
		{
			return false;
		}
		chara.MoveZone(zone, new ZoneTransition
		{
			state = ZoneTransition.EnterState.RandomVisit
		});
		return true;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool move_zone_2(DramaManager dm, Dictionary<string, string> line, string zoneFullName)
	{
		Chara chara = dm.GetChara(line["actor"]);
		Zone zone = ModUtil.FindZoneByFullName(zoneFullName);
		if (zone == null)
		{
			return false;
		}
		chara.MoveZone(zone, new ZoneTransition
		{
			state = ZoneTransition.EnterState.RandomVisit
		});
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool play_anime(DramaManager dm, Dictionary<string, string> line, AnimeID animeId)
	{
		dm.GetChara(line["actor"]).PlayAnime(animeId, force: true);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool play_effect(DramaManager dm, Dictionary<string, string> line, string effectId)
	{
		dm.GetChara(line["actor"]).PlayEffect(effectId);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool play_effect_at(DramaManager dm, Dictionary<string, string> line, string effectId, int x, int y)
	{
		Effect.Get(effectId)?.Play(new Point(x, y));
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool play_emote(DramaManager dm, Dictionary<string, string> line, Emo emote, float duration = 1f)
	{
		dm.GetChara(line["actor"]).ShowEmo(emote, duration, skipSame: false);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool play_screen_effect(DramaManager dm, Dictionary<string, string> line, string effectId)
	{
		ScreenEffect.Play(effectId);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool pop_text(DramaManager dm, Dictionary<string, string> line, string langText)
	{
		dm.GetChara(line["actor"]).HostRenderer.Say(langText.lang());
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool set_portrait(DramaManager dm, Dictionary<string, string> line, string portraitId = null)
	{
		Person person = dm.GetPerson(line["actor"]);
		if (person == null)
		{
			return false;
		}
		string text = person.chara?.GetIdPortrait();
		if (!portraitId.IsEmpty())
		{
			string text2 = person.id.IsEmpty(person.chara?.id);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			hashSet.Add("UN_" + text2 + "_" + text + ".png");
			hashSet.Add(text + ".png");
			hashSet.Add(text2 + "_" + text + ".png");
			string text3 = hashSet.FirstOrDefault(Portrait.allIds.Contains);
			if (text3 != null)
			{
				text = text3[..^4];
			}
		}
		person.idPortrait = text;
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool set_portrait_override(DramaManager dm, Dictionary<string, string> line, string portraitId = null)
	{
		dm.GetChara(line["actor"]).SetPortraitOverride(portraitId);
		return true;
	}

	[ElinDramaActionInvoke(null)]
	public static bool set_sprite(DramaManager dm, Dictionary<string, string> line, string spriteId = null)
	{
		dm.GetChara(line["actor"]).SetSpriteOverride(spriteId);
		return true;
	}

	[ElinDramaActionInvoke("nodiscard")]
	public static bool show_book(DramaManager dm, Dictionary<string, string> line, string bookEntry)
	{
		if (BookList.dict == null)
		{
			BookList.Init();
		}
		string[] array = bookEntry.Split('/');
		string text = array[0];
		string text2 = array[1];
		if (text2.EndsWith(".txt"))
		{
			text2 = text2[..^4];
		}
		if (!BookList.dict.TryGetValue(text, out var value) || !value.TryGetValue(text2, out var value2))
		{
			return false;
		}
		bool flag = text == "Scroll";
		EClass.ui.AddLayer<LayerHelp>(flag ? "LayerParchment" : "LayerBook").book.Show((flag ? "Scroll/" : "Book/") + value2.id, null, value2.title, value2);
		return true;
	}
}
