using System;

public class TraitASMR : Trait
{
	public int tick;

	public override bool HaveUpdate => true;

	public override void Update()
	{
		if (IsOn)
		{
			tick++;
			string[] source = Lang.Get("_ASMR").Split(Environment.NewLine.ToCharArray());
			if (tick % 5 == 0)
			{
				owner.TalkRaw(source.RandomItem());
			}
		}
	}
}
