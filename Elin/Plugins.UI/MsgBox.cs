using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MsgBox : MonoBehaviour
{
	[Serializable]
	public class Prefabs
	{
		public MsgBlock block;

		public MsgLine line;

		public UIText text;

		public Transform image;

		public UIButton button;
	}

	public VerticalLayoutGroup layout;

	public VerticalLayoutGroup layoutLog;

	public GameObject goLog;

	public GameObject goBox;

	public Prefabs prefabs;

	public Image imageBg;

	public UIDragPanel dragPanel;

	public int maxBlock;

	public int maxLog;

	public bool reverseOrder;

	public bool multiLine;

	public bool fadeLines;

	private SpriteAsset bg;

	private Color bgColor;

	[NonSerialized]
	public bool newblock;

	[NonSerialized]
	public bool isShowingLog;

	[NonSerialized]
	public MsgBlock currentBlock;

	[NonSerialized]
	public List<MsgBlock> blocks = new List<MsgBlock>();

	[NonSerialized]
	public List<MsgBlock> logs = new List<MsgBlock>();

	public float maxWidth => this.Rect().rect.width / BaseCore.Instance.uiScale;

	public void Init()
	{
		goLog.SetActive(value: false);
		isShowingLog = false;
	}

	public void Append(string s, Color col)
	{
		SetCurrentBlock();
		currentBlock.Append(s, col);
	}

	public void Append(Sprite s, bool fitLine = false)
	{
		SetCurrentBlock();
		currentBlock.Append(s, fitLine);
	}

	public UIItem Load(string id)
	{
		SetCurrentBlock();
		return currentBlock.Load(id);
	}

	public void CreateNewBlock()
	{
		currentBlock = PoolManager.Spawn(prefabs.block, layout);
		currentBlock.box = this;
		currentBlock.bg.sprite = bg.sprite;
		currentBlock.bg.Rect().sizeDelta = bg.size;
		currentBlock.bg.color = bgColor;
		currentBlock.Reset();
		newblock = false;
		if (reverseOrder)
		{
			currentBlock.transform.SetAsFirstSibling();
		}
		blocks.Add(currentBlock);
		if (blocks.Count > maxBlock)
		{
			int num = blocks.Count - maxBlock;
			for (int i = 0; i < num; i++)
			{
				AddLog();
			}
		}
		if (isShowingLog)
		{
			currentBlock.transform.SetParent(layoutLog.transform, worldPositionStays: false);
			Image image = currentBlock.bg;
			bool flag2 = (currentBlock.cg.enabled = false);
			image.enabled = flag2;
		}
		else
		{
			Image image2 = currentBlock.bg;
			bool flag2 = (currentBlock.cg.enabled = true);
			image2.enabled = flag2;
			RefreshAlpha();
		}
	}

	private void SetCurrentBlock()
	{
		if (!currentBlock || (newblock && currentBlock.countElements > 0))
		{
			CreateNewBlock();
		}
	}

	public void MarkNewBlock()
	{
		newblock = true;
	}

	public void RefreshAlpha()
	{
		for (int i = 0; i < blocks.Count; i++)
		{
			float alpha = 1f;
			if (fadeLines && blocks.Count >= 3)
			{
				switch (i)
				{
				case 1:
					alpha = 0.8f;
					break;
				case 0:
					alpha = 0.65f;
					break;
				}
			}
			blocks[i].cg.alpha = alpha;
		}
	}

	public void SetBG(SpriteAsset s, Color c)
	{
		bg = s;
		bgColor = c;
		foreach (MsgBlock item in blocks.Concat(logs))
		{
			item.bg.sprite = bg.sprite;
			item.bg.color = bgColor;
			item.bg.Rect().sizeDelta = bg.size;
		}
	}

	public void Clear()
	{
		foreach (MsgBlock block in blocks)
		{
			PoolManager.Despawn(block);
		}
		foreach (MsgBlock log in logs)
		{
			PoolManager.Despawn(log);
		}
		blocks.Clear();
		logs.Clear();
		currentBlock = null;
		newblock = false;
	}

	public void AddLog()
	{
		MsgBlock msgBlock = blocks[0];
		blocks.RemoveAt(0);
		logs.Add(msgBlock);
		if (!isShowingLog)
		{
			msgBlock.transform.SetParent(layoutLog.transform, worldPositionStays: false);
		}
		if (logs.Count > maxLog)
		{
			msgBlock = logs[0];
			logs.RemoveAt(0);
			PoolManager.Despawn(msgBlock);
		}
	}

	public void ToggleLog()
	{
		ToggleLog(!isShowingLog);
	}

	public void ToggleLog(bool show)
	{
		isShowingLog = show;
		goBox.SetActive(!show);
		goLog.SetActive(show);
		if (show)
		{
			foreach (MsgBlock block in blocks)
			{
				block.transform.SetParent(layoutLog.transform, worldPositionStays: false);
			}
			MsgBlock[] componentsInChildren = layoutLog.GetComponentsInChildren<MsgBlock>();
			foreach (MsgBlock msgBlock in componentsInChildren)
			{
				CanvasGroup cg = msgBlock.cg;
				bool flag2 = (msgBlock.bg.enabled = false);
				cg.enabled = flag2;
			}
			return;
		}
		foreach (MsgBlock block2 in blocks)
		{
			block2.transform.SetParent(layout.transform, worldPositionStays: false);
			CanvasGroup cg2 = block2.cg;
			bool flag2 = (block2.bg.enabled = true);
			cg2.enabled = flag2;
		}
		RefreshAlpha();
		layout.RebuildLayout();
	}
}
