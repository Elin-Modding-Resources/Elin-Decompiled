using System;
using System.Collections.Generic;
using UnityEngine;

public class DramaEventTalk : DramaEvent
{
	public string text;

	public string idCancelJump;

	public List<DramaChoice> choices = new List<DramaChoice>();

	public bool center;

	public bool canCancel;

	public bool unknown;

	public bool ignoreClickSound;

	public float timer;

	public Func<string> funcText;

	public DramaEventTalk()
	{
	}

	public DramaEventTalk(string _idActor, Func<string> func)
	{
		if (_idActor.Contains("?"))
		{
			unknown = true;
		}
		idActor = _idActor.Replace("?", "");
		funcText = func;
	}

	public DramaEventTalk(string _idActor, string _text, List<DramaChoice> listChoice = null)
	{
		if (_idActor.Contains("?"))
		{
			unknown = true;
		}
		idActor = _idActor.Replace("?", "");
		text = _text;
		if (listChoice == null)
		{
			return;
		}
		foreach (DramaChoice item in listChoice)
		{
			AddChoice(item);
		}
	}

	public void AddChoice(DramaChoice choice)
	{
		choices.Add(choice);
	}

	public override bool Play()
	{
		if (progress == 0)
		{
			InitDialog();
			if (sequence.firstTalk == null)
			{
				sequence.firstTalk = this;
			}
			timer = 0f;
			sequence.dialog.SetActive(enable: true);
			base.actor.Talk(sequence.message + ((funcText != null) ? funcText() : text), choices, center, unknown);
			sequence.message = "";
			progress++;
		}
		else
		{
			timer += Time.deltaTime;
			if (timer < 0.08f)
			{
				return false;
			}
			if (canCancel && (EInput.rightMouse.down || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.LeftShift)))
			{
				ResetDialog();
				if (temp)
				{
					sequence.tempEvents.Clear();
				}
				if (!ignoreClickSound)
				{
					EClass.Sound.Play("click_chat");
				}
				if (idCancelJump == "back")
				{
					return true;
				}
				if (idCancelJump.IsEmpty())
				{
					sequence.Exit();
				}
				else
				{
					sequence.Play(idCancelJump);
				}
				return false;
			}
			int num = 0;
			foreach (DramaChoice choice in choices)
			{
				if (choice.activeCondition != null && !choice.activeCondition())
				{
					continue;
				}
				string inputString = Input.inputString;
				if ((bool)choice.button && inputString == ((num + 1).ToString() ?? ""))
				{
					if ((bool)choice.button.soundClick)
					{
						SoundManager.current.Play(choice.button.soundClick);
					}
					choice.button.onClick.Invoke();
					return false;
				}
				num++;
			}
			if (num == 0 && (EInput.leftMouse.down || EInput.rightMouse.down || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.LeftControl) || EInput.action == EAction.Confirm || EInput.action == EAction.Wait || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)))
			{
				ResetDialog();
				if (EClass.ui.GetTopLayer() == base.layer)
				{
					Typewriter typewriter = sequence.dialog.textMain.typewriter;
					if ((bool)typewriter && !typewriter.IsFinished)
					{
						typewriter.Skip();
						return false;
					}
					base.actor.gameObject.SetActive(value: false);
					if (!ignoreClickSound)
					{
						EClass.Sound.Play("click_chat");
					}
					if (temp)
					{
						sequence.tempEvents.Clear();
						if (!idJump.IsEmpty())
						{
							sequence.Play(idJump);
							return false;
						}
						return true;
					}
					if (idJump.IsEmpty())
					{
						sequence.PlayNext();
					}
					else
					{
						sequence.Play(idJump);
					}
					return false;
				}
			}
		}
		return false;
	}

	public void InitDialog()
	{
		if (idActor == "*")
		{
			base.manager.SetDialog("Mono");
		}
		if (idActor == "*2")
		{
			base.manager.SetDialog("Default2");
		}
		base.manager.imageBG.color = ((idActor == "*") ? new Color(0.6f, 0.6f, 0.6f) : Color.white);
	}

	public void ResetDialog()
	{
		base.manager.imageBG.color = Color.white;
		if (idActor == "*" || idActor == "*2")
		{
			base.manager.SetDialog();
		}
	}
}
