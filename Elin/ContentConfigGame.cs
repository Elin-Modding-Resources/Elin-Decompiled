using UnityEngine.Events;
using UnityEngine.UI;

public class ContentConfigGame : ContentConfig
{
	public UIButton toggleFogBounds;

	public UIButton toggleOffhand;

	public UIButton toggleAntiSpider;

	public UIButton toggleShowInvBG;

	public UIButton toggleConfirmGive;

	public UIButton toggleWarnCrime;

	public UIButton toggleWarnMana;

	public UIButton toggleWarnDisassemble;

	public UIButton toggleHoldMiddle;

	public UIButton toggleHoldDoubleClick;

	public UIButton toggleConfirmExit;

	public UIButton toggleUseAbilityOnHotkey;

	public UIButton toggleHideWeapon;

	public UIButton toggleNetSend;

	public UIButton toggleNet;

	public UIButton toggleDisableAutoStairs;

	public UIButton toggleSmoothPick;

	public UIButton toggleMarkStack;

	public UIButton toggleRefIcon;

	public UIButton toggleShippingResult;

	public UIButton toggleShiftToUseNegativeAbility;

	public UIButton toggleAutoBackup;

	public UIButton toggleSortEach;

	public UISelectableGroup groupBackerFilter;

	public UISelectableGroup groupBorder;

	public UISelectableGroup groupRide;

	public Slider sliderWait;

	public Slider sliderBackupNum;

	public Slider sliderBackupInterval;

	public UIButton toggleWaitOnDebuff;

	public UIButton toggleWaitOnRange;

	public UIButton toggleWaitOnMelee;

	public UIButton toggleTutorial;

	public UIButton toggleAdvanceMenu;

	public UIButton toggleHaltOnSpotEnemy;

	public UIButton toggleHaltOnSpotTrap;

	public override void OnInstantiate()
	{
		toggleDisableAutoStairs.SetToggle(base.config.game.disableAutoStairs, delegate(bool on)
		{
			base.config.game.disableAutoStairs = on;
		});
		toggleConfirmGive.SetToggle(base.config.game.confirmGive, delegate(bool on)
		{
			base.config.game.confirmGive = on;
		});
		toggleWarnCrime.SetToggle(base.config.game.warnCrime, delegate(bool on)
		{
			base.config.game.warnCrime = on;
		});
		toggleWarnMana.SetToggle(base.config.game.warnMana, delegate(bool on)
		{
			base.config.game.warnMana = on;
		});
		toggleWarnDisassemble.SetToggle(base.config.game.warnDisassemble, delegate(bool on)
		{
			base.config.game.warnDisassemble = on;
		});
		toggleFogBounds.SetToggle(base.config.game.dontRenderOutsideMap, delegate(bool on)
		{
			base.config.game.dontRenderOutsideMap = on;
		});
		toggleShowInvBG.SetToggle(base.config.game.showInvBG, delegate(bool on)
		{
			base.config.game.showInvBG = on;
		});
		toggleHaltOnSpotEnemy.SetToggle(base.config.game.haltOnSpotEnemy, delegate(bool on)
		{
			base.config.game.haltOnSpotEnemy = on;
		});
		toggleHaltOnSpotTrap.SetToggle(base.config.game.haltOnSpotTrap, delegate(bool on)
		{
			base.config.game.haltOnSpotTrap = on;
		});
		toggleHideWeapon.SetToggle(base.config.game.hideWeapons, delegate(bool on)
		{
			base.config.game.hideWeapons = on;
		});
		toggleSmoothPick.SetToggle(base.config.game.smoothPick, delegate(bool on)
		{
			base.config.game.smoothPick = on;
		});
		toggleMarkStack.SetToggle(base.config.game.markStack, delegate(bool on)
		{
			base.config.game.markStack = on;
		});
		toggleWaitOnDebuff.SetToggle(base.config.game.waitOnDebuff, delegate(bool on)
		{
			base.config.game.waitOnDebuff = on;
		});
		toggleWaitOnRange.SetToggle(base.config.game.waitOnRange, delegate(bool on)
		{
			base.config.game.waitOnRange = on;
		});
		toggleWaitOnMelee.SetToggle(base.config.game.waitOnMelee, delegate(bool on)
		{
			base.config.game.waitOnMelee = on;
		});
		toggleTutorial.SetToggle(base.config.game.tutorial, delegate(bool on)
		{
			base.config.game.tutorial = on;
		});
		toggleShippingResult.SetToggle(base.config.game.showShippingResult, delegate(bool on)
		{
			base.config.game.showShippingResult = on;
		});
		toggleAdvanceMenu.SetToggle(base.config.game.advancedMenu, delegate(bool on)
		{
			base.config.game.advancedMenu = on;
		});
		toggleSortEach.SetToggle(base.config.game.sortEach, delegate(bool on)
		{
			base.config.game.sortEach = on;
		});
		toggleAntiSpider.SetToggle(base.config.game.antiSpider, delegate(bool on)
		{
			base.config.game.antiSpider = on;
		});
		toggleOffhand.SetToggle(base.config.game.showOffhand, delegate(bool on)
		{
			base.config.game.showOffhand = on;
			EClass.pc.SetTempHand();
		});
		toggleHoldMiddle.SetToggle(base.config.game.holdMiddleButtonToHold, delegate(bool on)
		{
			base.config.game.holdMiddleButtonToHold = on;
		});
		toggleShiftToUseNegativeAbility.SetToggle(base.config.game.shiftToUseNegativeAbilityOnSelf, delegate(bool on)
		{
			base.config.game.shiftToUseNegativeAbilityOnSelf = on;
		});
		toggleHoldDoubleClick.SetToggle(base.config.game.doubleClickToHold, delegate(bool on)
		{
			base.config.game.doubleClickToHold = on;
		});
		toggleConfirmExit.SetToggle(base.config.game.confirmMapExit, delegate(bool on)
		{
			base.config.game.confirmMapExit = on;
		});
		toggleUseAbilityOnHotkey.SetToggle(base.config.game.useAbilityOnHotkey, delegate(bool on)
		{
			base.config.game.useAbilityOnHotkey = on;
		});
		toggleNetSend.SetToggle(base.config.net.sendEvent, delegate(bool on)
		{
			base.config.net.sendEvent = on;
		});
		SetSlider(sliderWait, base.config.game.waiter, delegate(float a)
		{
			base.config.game.waiter = (int)a;
			return Lang.Get("gameWait_duration") + "(" + 25f * a + "%)";
		});
		SetSlider(sliderBackupNum, base.config.game.numBackup, delegate(float a)
		{
			base.config.game.numBackup = (int)a;
			return "backupNum".lang(a.ToString() ?? "");
		});
		SetSlider(sliderBackupInterval, base.config.game.backupInterval, delegate(float a)
		{
			base.config.game.backupInterval = (int)a;
			return "backupInterval".lang((a * 30f).ToString() ?? "");
		});
		toggleAutoBackup.SetToggle(base.config.game.autoBackup, delegate(bool on)
		{
			base.config.game.autoBackup = on;
			sliderBackupInterval.SetActive(on);
		});
		sliderBackupInterval.SetActive(base.config.game.autoBackup);
		SetGroup(groupRide, base.config.game.showRide, delegate(int a)
		{
			base.config.game.showRide = a;
			base.config.Apply();
		});
		SetGroup(groupBackerFilter, base.config.backer.filter, delegate(int a)
		{
			base.config.backer.filter = a;
			base.config.Apply();
		});
		SetGroup(groupBorder, base.config.game.showBorder, delegate(int a)
		{
			base.config.game.showBorder = a;
			base.config.Apply();
		});
	}

	public void SetGroup(UISelectableGroup group, int value, UnityAction<int> action)
	{
		group.Init(value, action);
	}
}
