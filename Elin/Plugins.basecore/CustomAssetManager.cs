using System.Collections.Generic;
using UnityEngine;

public class CustomAssetManager : ScriptableObject, ISerializationCallbackReceiver
{
	public static CustomAssetManager Instance;

	public string pathEditor = "Assets/Plugins/Essential/DB/Editor/";

	public string exportPath = "Assets/Resources/Data/";

	public string assembly = "Plugins.BaseCore";

	public List<string> pathRawImport;

	public List<ExcelBookImportSetting> books;

	private void Awake()
	{
		Instance = this;
	}

	public void OnBeforeSerialize()
	{
		Instance = this;
	}

	public void OnAfterDeserialize()
	{
		Instance = this;
	}

	public ExcelBookImportSetting GetBook(string id)
	{
		foreach (ExcelBookImportSetting book in books)
		{
			if (book.name == id)
			{
				return book;
			}
		}
		return null;
	}

	public ExcelBookImportSetting GetOrCreateBook(string id)
	{
		ExcelBookImportSetting book = GetBook(id);
		if (book != null)
		{
			return book;
		}
		book = new ExcelBookImportSetting
		{
			name = id
		};
		books.Add(book);
		return book;
	}
}
