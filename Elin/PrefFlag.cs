using System;

[Flags]
public enum PrefFlag
{
	UsePref = 1,
	Test1 = 2,
	Float = 4,
	Surface = 8,
	FloatUnderwater = 0x10
}
