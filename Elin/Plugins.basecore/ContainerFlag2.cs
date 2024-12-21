using System;

[Flags]
public enum ContainerFlag2
{
	none = 0,
	meal = 1,
	foodstuff = 2,
	meat = 4,
	vegi = 8,
	fruit = 0x10,
	mushroom = 0x20,
	egg = 0x21,
	fish = 0x40,
	nuts = 0x80,
	foodstuff_raw = 0x100,
	seasoning = 0x200,
	rod = 0x400,
	junk = 0x800,
	garbage = 0x1000,
	bill = 0x2000,
	scroll = 0x4000,
	spellbook = 0x8000,
	card = 0x10000,
	figure = 0x20000,
	bait = 0x40000,
	seed = 0x80000,
	stone = 0x100000,
	textile = 0x200000,
	flora = 0x400000,
	bodyparts = 0x800000,
	fertilizer = 0x1000000,
	milk = 0x2000000,
	wood = 0x4000000,
	ore = 0x8000000,
	ammo = 0x10000000,
	bed = 0x20000000
}
