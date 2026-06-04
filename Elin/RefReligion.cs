using System.Collections.Generic;
using Newtonsoft.Json;

public class RefReligion : EClass
{
	[JsonProperty]
	public string uid;

	private Religion _religion;

	public Religion Instance => _religion ?? (_religion = EClass.game.religions.dictAll.GetValueOrDefault(uid, EClass.game.religions.Eyth));

	public RefReligion()
	{
	}

	public RefReligion(Religion religion)
	{
		_religion = religion;
		uid = religion.id;
	}
}
