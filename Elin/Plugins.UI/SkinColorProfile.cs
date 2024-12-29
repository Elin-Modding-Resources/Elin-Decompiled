using UnityEngine;

public class SkinColorProfile : ScriptableObject
{
	public Color highlight;

	public Color textDefault;

	public Color textTopic;

	public Color textTopic2;

	public Color textHeader;

	public Color textHeaderDark;

	public Color textHeaderMenu;

	public Color textPassive;

	public Color textTopMenu;

	public Color textChat;

	public Color textGood;

	public Color textGreat;

	public Color textUtil;

	public Color textBad;

	public Color textWarning;

	public Color textFlavor;

	public Color textItemName;

	public Color textInteraction;

	public Color textTitle;

	public Color textNews;

	public Color textTag;

	public Color textCharge;

	public Color textGray;

	public Color textMyth;

	public Color textFoodQuality;

	public Color textFoodMisc;

	public Color textMsgDefault;

	public Color textQuestObjective;

	public Color textBlue;

	public Color textEther;

	public Color buttonGeneral;

	public Color buttonSelectable;

	public Color buttonBig;

	public Color buttonBottom;

	public Color buttonGrid;

	public Color buttonGrid2;

	public Color buttonGrid3;

	public Color buttonSide;

	public Color dropdown;

	public Color tab;

	public Color colorLine;

	public Color textUnidentified;

	public Color textIdentified;

	public Color textBlessed;

	public Color textCursed;

	public Color textDoomed;

	public Gradient gradientHP;

	public Gradient gradientMP;

	public Gradient gradientSP;

	public Gradient gradientTool;

	public UD_String_Gradient gradients;

	[Range(0f, 2f)]
	public float contrast = 1f;

	[Range(-1f, 1f)]
	public float strength;

	public Color GetTextColor(FontColor fontColor)
	{
		return fontColor switch
		{
			FontColor.ButtonGeneral => buttonGeneral, 
			FontColor.ButtonSelectable => buttonSelectable, 
			FontColor.ButtonBig => buttonBig, 
			FontColor.ButtonBottom => buttonBottom, 
			FontColor.ButtonGrid => buttonGrid, 
			FontColor.ButtonGrid2 => buttonGrid2, 
			FontColor.ButtonGrid3 => buttonGrid3, 
			FontColor.ButtonSide => buttonSide, 
			FontColor.Header => textHeader, 
			FontColor.HeaderDark => textHeaderDark, 
			FontColor.HeaderMenu => textHeaderMenu, 
			FontColor.Topic => textTopic, 
			FontColor.Topic2 => textTopic2, 
			FontColor.Passive => textPassive, 
			FontColor.Title => textTitle, 
			FontColor.News => textNews, 
			FontColor.Good => textGood, 
			FontColor.Great => textGreat, 
			FontColor.Util => textUtil, 
			FontColor.Bad => textBad, 
			FontColor.Warning => textWarning, 
			FontColor.Flavor => textFlavor, 
			FontColor.ItemName => textItemName, 
			FontColor.Interaction => textInteraction, 
			FontColor.MsgDefault => textMsgDefault, 
			FontColor.Dropdown => dropdown, 
			FontColor.Tab => tab, 
			FontColor.QuestObjective => textQuestObjective, 
			FontColor.Ether => textEther, 
			FontColor.Tag => textTag, 
			FontColor.Charge => textCharge, 
			FontColor.Myth => textMyth, 
			FontColor.Gray => textGray, 
			FontColor.FoodQuality => textFoodQuality, 
			FontColor.FoodMisc => textFoodMisc, 
			_ => textDefault, 
		};
	}
}
