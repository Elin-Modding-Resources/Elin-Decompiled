using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace SFB;

public class StandaloneFileBrowserWindows : IStandaloneFileBrowser
{
	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
	{
		VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
		vistaOpenFileDialog.Title = title;
		if (extensions != null)
		{
			vistaOpenFileDialog.Filter = GetFilterFromFileExtensionList(extensions);
			vistaOpenFileDialog.FilterIndex = 1;
		}
		else
		{
			vistaOpenFileDialog.Filter = string.Empty;
		}
		vistaOpenFileDialog.Multiselect = multiselect;
		if (!string.IsNullOrEmpty(directory))
		{
			vistaOpenFileDialog.FileName = GetDirectoryPath(directory);
		}
		string[] result = ((vistaOpenFileDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK) ? vistaOpenFileDialog.FileNames : new string[0]);
		vistaOpenFileDialog.Dispose();
		return result;
	}

	public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb)
	{
		cb(OpenFilePanel(title, directory, extensions, multiselect));
	}

	public string[] OpenFolderPanel(string title, string directory, bool multiselect)
	{
		VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
		vistaFolderBrowserDialog.Description = title;
		if (!string.IsNullOrEmpty(directory))
		{
			vistaFolderBrowserDialog.SelectedPath = GetDirectoryPath(directory);
		}
		string[] result = ((vistaFolderBrowserDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) != DialogResult.OK) ? new string[0] : new string[1] { vistaFolderBrowserDialog.SelectedPath });
		vistaFolderBrowserDialog.Dispose();
		return result;
	}

	public void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
	{
		cb(OpenFolderPanel(title, directory, multiselect));
	}

	public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
	{
		VistaSaveFileDialog vistaSaveFileDialog = new VistaSaveFileDialog();
		vistaSaveFileDialog.Title = title;
		string text = "";
		if (!string.IsNullOrEmpty(directory))
		{
			text = GetDirectoryPath(directory);
		}
		if (!string.IsNullOrEmpty(defaultName))
		{
			text += defaultName;
		}
		vistaSaveFileDialog.FileName = text;
		if (extensions != null)
		{
			vistaSaveFileDialog.Filter = GetFilterFromFileExtensionList(extensions);
			vistaSaveFileDialog.FilterIndex = 1;
			vistaSaveFileDialog.DefaultExt = extensions[0].Extensions[0];
			vistaSaveFileDialog.AddExtension = true;
		}
		else
		{
			vistaSaveFileDialog.DefaultExt = string.Empty;
			vistaSaveFileDialog.Filter = string.Empty;
			vistaSaveFileDialog.AddExtension = false;
		}
		string result = ((vistaSaveFileDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK) ? vistaSaveFileDialog.FileName : "");
		vistaSaveFileDialog.Dispose();
		return result;
	}

	public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
	{
		cb(SaveFilePanel(title, directory, defaultName, extensions));
	}

	private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
	{
		string text = "";
		for (int i = 0; i < extensions.Length; i++)
		{
			ExtensionFilter extensionFilter = extensions[i];
			text = text + extensionFilter.Name + "(";
			string[] extensions2 = extensionFilter.Extensions;
			foreach (string text2 in extensions2)
			{
				text = text + "*." + text2 + ",";
			}
			text = text.Remove(text.Length - 1);
			text += ") |";
			extensions2 = extensionFilter.Extensions;
			foreach (string text3 in extensions2)
			{
				text = text + "*." + text3 + "; ";
			}
			text += "|";
		}
		return text.Remove(text.Length - 1);
	}

	private static string GetDirectoryPath(string directory)
	{
		string text = Path.GetFullPath(directory);
		if (!text.EndsWith("\\"))
		{
			text += "\\";
		}
		if (Path.GetPathRoot(text) == text)
		{
			return directory;
		}
		string directoryName = Path.GetDirectoryName(text);
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		return directoryName + directorySeparatorChar;
	}
}
