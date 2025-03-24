using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class EClass
{
	public static Core core;

	public static Game game => core.game;

	public static bool AdvMode => ActionMode.IsAdv;

	public static Player player => core.game.player;

	public static Chara pc => core.game.player.chara;

	public static UI ui => core.ui;

	public static Map _map => core.game.activeZone.map;

	public static Zone _zone => core.game.activeZone;

	public static FactionBranch Branch => core.game.activeZone.branch;

	public static FactionBranch BranchOrHomeBranch => Branch ?? pc.homeBranch;

	public static Faction Home => core.game.factions.Home;

	public static Faction Wilds => core.game.factions.Wilds;

	public static Scene scene => core.scene;

	public static BaseGameScreen screen => core.screen;

	public static GameSetting setting => core.gameSetting;

	public static GameData gamedata => core.gamedata;

	public static ColorProfile Colors => core.Colors;

	public static World world => core.game.world;

	public static SourceManager sources => core.sources;

	public static SourceManager editorSources => Core.SetCurrent().sources;

	public static SoundManager Sound => SoundManager.current;

	public static CoreDebug debug => core.debug;

	public static int rndSeed(int a, int seed)
	{
		Rand.SetSeed(seed);
		int result = rnd(a);
		Rand.SetSeed();
		return result;
	}

	public static int rnd(int a)
	{
		return Rand.rnd(a);
	}

	public static int curve(int a, int start, int step, int rate = 75)
	{
		if (a <= start)
		{
			return a;
		}
		for (int i = 0; i < 10; i++)
		{
			int num = start + i * step;
			if (a > num)
			{
				a = num + (a - num) * rate / 100;
				continue;
			}
			return a;
		}
		return a;
	}

	public static int rndHalf(int a)
	{
		return a / 2 + Rand.rnd(a / 2);
	}

	public static float rndf(float a)
	{
		return Rand.rndf(a);
	}

	public static int rndSqrt(int a)
	{
		return Rand.rndSqrt(a);
	}

	public static void Wait(float a, Card c)
	{
		Game.Wait(a, c);
	}

	public static void Wait(float a, Point p)
	{
		Game.Wait(a, p);
	}

	public static int Bigger(int a, int b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	public static int Smaller(int a, int b)
	{
		if (a >= b)
		{
			return b;
		}
		return a;
	}
}
