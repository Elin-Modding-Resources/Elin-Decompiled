using System;

[Flags]
public enum ContainerFlag
{
	none = 0,
	resource = 1,
	food = 2,
	drink = 4,
	weapon = 8,
	armor = 0x10,
	tool = 0x20,
	item = 0x40,
	book = 0x80,
	currency = 0x100,
	furniture = 0x200,
	block = 0x400,
	other = 0x800
}
