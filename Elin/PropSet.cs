using System.Collections.Generic;

public class PropSet : HashSet<Card>
{
	public int num;

	public new void Add(Card c)
	{
		ModNum(c.Num);
		base.Add(c);
	}

	public new void Remove(Card c)
	{
		ModNum(-c.Num);
		base.Remove(c);
	}

	public virtual void ModNum(int a)
	{
		num += a;
	}

	public void OnChangeNum(int a)
	{
		ModNum(a);
	}
}
