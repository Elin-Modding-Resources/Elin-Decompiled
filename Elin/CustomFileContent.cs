using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public abstract class CustomFileContent : CustomContent
{
	protected DateTime LastModified { get; set; } = DateTime.MinValue;


	[JsonIgnore]
	public FileInfo File { get; set; }

	public bool HasFileChanged()
	{
		File.Refresh();
		return File.LastWriteTimeUtc != LastModified;
	}

	public virtual void OnSetLang(string lang)
	{
	}

	public void Load()
	{
		if (!HasFileChanged())
		{
			return;
		}
		try
		{
			LoadContent();
		}
		catch (Exception exception)
		{
			ModUtil.LogModError("exception while loading file '" + base.ContentId + "'", base.Owner);
			Debug.LogException(exception);
		}
		finally
		{
			LastModified = File.LastWriteTimeUtc;
		}
	}

	protected abstract void LoadContent();
}
