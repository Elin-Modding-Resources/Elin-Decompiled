public struct ActRef
{
	public Act act;

	public string aliasEle;

	public string n1;

	public Thing refThing;

	public Chara origin;

	public bool isPerfume;

	public bool noFriendlyFire;

	public int refVal;

	public int idEle => EClass.sources.elements.alias[aliasEle].id;
}
