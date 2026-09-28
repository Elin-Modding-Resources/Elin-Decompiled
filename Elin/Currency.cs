public class Currency
{
	public static string ToID(CurrencyType currency)
	{
		return currency switch
		{
			CurrencyType.Medal => "medal", 
			CurrencyType.Money2 => "money2", 
			CurrencyType.Influence => "influence", 
			CurrencyType.Casino_coin => "casino_coin", 
			CurrencyType.Ecopo => "ecopo", 
			CurrencyType.Money3 => "money3", 
			CurrencyType.Plat => "plat", 
			_ => "money", 
		};
	}
}
