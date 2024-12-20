using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DramaEventEndRoll : DramaEvent
{
	private float timer;

	public UIDynamicList list => sequence.manager.listCredit;

	public override bool Play()
	{
		if (progress == 0)
		{
			sequence.manager.endroll.SetActive(enable: true);
			Init();
			progress++;
		}
		else
		{
			timer += Time.deltaTime;
			if (timer < 0.08f)
			{
				return false;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				return true;
			}
			if (list.dsv.contentAnchoredPosition <= 0f - list.dsv.contentSize + list.dsv.viewportSize + 1f)
			{
				if ((!EInput.rightMouse.pressedLong || !Application.isEditor) && EInput.IsAnyKeyDown())
				{
					return true;
				}
			}
			else
			{
				float num = 1f;
				if (EInput.leftMouse.pressing)
				{
					num = 10f;
				}
				if (EInput.rightMouse.pressing)
				{
					num = (Application.isEditor ? 200f : 30f);
				}
				list.dsv.contentAnchoredPosition += Time.deltaTime * sequence.manager.creditSpeed * num;
			}
		}
		return false;
	}

	public void Init()
	{
		ExcelData excelData = new ExcelData("Data/Raw/credit", 1);
		List<Dictionary<string, string>> items = excelData.BuildList();
		int index = 0;
		list.Clear();
		list.callbacks = new UIList.Callback<string, UIItem>
		{
			onRedraw = delegate(string a, UIItem b, int i)
			{
				b.text1.SetText(BackerContent.ConvertName(a));
			},
			onList = delegate
			{
				List<string> list = IO.LoadTextArray(CorePath.CorePackage.TextCommon + "endroll").ToList();
				for (int k = 0; k < Screen.height / 32 + 1; k++)
				{
					Space(newline: true);
				}
				foreach (string item in list)
				{
					if (item.StartsWith("#"))
					{
						string[] array = item.Replace("#", "").Split(',');
						string text = array[0];
						int num2 = array[1].ToInt();
						foreach (Dictionary<string, string> item2 in items)
						{
							if (index % 5 < num2)
							{
								for (int l = 0; l < num2; l++)
								{
									this.list.Add("");
									index++;
								}
							}
							if ((text == "Abandon" && item2["abandon"] == "Yes") || item2["Pledge"] == text)
							{
								AddBacker(item2["Name"]);
							}
							if (index % 5 >= 5 - num2)
							{
								for (int n = 0; n < num2; n++)
								{
									this.list.Add("");
									index++;
								}
							}
						}
					}
					else
					{
						Add(item);
					}
				}
				for (int num3 = 0; num3 < Screen.height / 32 / 2 - 1; num3++)
				{
					Space(newline: true);
				}
			}
		};
		list.List();
		void Add(string s)
		{
			Space(newline: false);
			list.Add("");
			list.Add("");
			list.Add(s);
			list.Add("");
			list.Add("");
			index += 5;
		}
		void AddBacker(string s)
		{
			list.Add(s);
			index++;
		}
		void Space(bool newline)
		{
			int num = (newline ? 5 : 0);
			if (index % 5 != 0)
			{
				num += 5 - index % 5;
			}
			for (int j = 0; j < num; j++)
			{
				list.Add("");
				index++;
			}
		}
	}
}
