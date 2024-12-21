using System;

[Serializable]
public class ExcelSheetImportSetting
{
	public enum ClassTemplate
	{
		Default,
		ThingV
	}

	public string name;

	public string _baseClass;

	public string _rowClass;

	public string _templateFile;

	public bool intID;

	public SourceData.AutoID autoID;

	public ClassTemplate template;

	public string idTemplate => "ClassTemplate" + ((template == ClassTemplate.Default) ? "" : ("_" + template));

	public string baseClass
	{
		get
		{
			if (!string.IsNullOrEmpty(_baseClass))
			{
				return _baseClass;
			}
			if (!intID)
			{
				return "SourceDataString";
			}
			return "SourceDataInt";
		}
	}

	public string rowClass
	{
		get
		{
			if (!string.IsNullOrEmpty(_rowClass))
			{
				return _rowClass;
			}
			return "BaseRow";
		}
	}

	public override string ToString()
	{
		return name;
	}
}
