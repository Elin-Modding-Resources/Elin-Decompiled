using System.Collections.Generic;

public class EDramaScriptState : EScriptState
{
	public DramaManager dm;

	public Dictionary<string, string> line;

	public Chara pc => EClass.pc;

	public Chara tg => dm.GetActor("tg").owner.chara;

	public string text
	{
		get
		{
			return line["text"];
		}
		set
		{
			line["text"] = value;
		}
	}
}
