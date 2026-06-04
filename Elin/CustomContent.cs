using Newtonsoft.Json;

public abstract class CustomContent : EClass, ICustomContent
{
	[JsonIgnore]
	public string ContentId { get; protected set; }

	[JsonIgnore]
	public EMod Owner { get; protected set; }

	public virtual void OnGameLoad(GameIOContext context)
	{
	}

	public virtual void OnGameSave(GameIOContext context)
	{
	}

	public override string ToString()
	{
		return ContentId;
	}
}
