using System;

namespace SFB;

public class StandaloneFileBrowser
{
	private static IStandaloneFileBrowser _platformWrapper;

	static StandaloneFileBrowser()
	{
		_platformWrapper = new StandaloneFileBrowserWindows();
	}

	public static string[] OpenFilePanel(string title, string directory, string extension, bool multiselect)
	{
		ExtensionFilter[] extensions = (string.IsNullOrEmpty(extension) ? null : new ExtensionFilter[1]
		{
			new ExtensionFilter("", extension)
		});
		return OpenFilePanel(title, directory, extensions, multiselect);
	}

	public static string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
	{
		return _platformWrapper.OpenFilePanel(title, directory, extensions, multiselect);
	}

	public static void OpenFilePanelAsync(string title, string directory, string extension, bool multiselect, Action<string[]> cb)
	{
		ExtensionFilter[] extensions = (string.IsNullOrEmpty(extension) ? null : new ExtensionFilter[1]
		{
			new ExtensionFilter("", extension)
		});
		OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
	}

	public static void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb)
	{
		_platformWrapper.OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
	}

	public static string[] OpenFolderPanel(string title, string directory, bool multiselect)
	{
		return _platformWrapper.OpenFolderPanel(title, directory, multiselect);
	}

	public static void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
	{
		_platformWrapper.OpenFolderPanelAsync(title, directory, multiselect, cb);
	}

	public static string SaveFilePanel(string title, string directory, string defaultName, string extension)
	{
		ExtensionFilter[] extensions = (string.IsNullOrEmpty(extension) ? null : new ExtensionFilter[1]
		{
			new ExtensionFilter("", extension)
		});
		return SaveFilePanel(title, directory, defaultName, extensions);
	}

	public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
	{
		return _platformWrapper.SaveFilePanel(title, directory, defaultName, extensions);
	}

	public static void SaveFilePanelAsync(string title, string directory, string defaultName, string extension, Action<string> cb)
	{
		ExtensionFilter[] extensions = (string.IsNullOrEmpty(extension) ? null : new ExtensionFilter[1]
		{
			new ExtensionFilter("", extension)
		});
		SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
	}

	public static void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
	{
		_platformWrapper.SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
	}
}
