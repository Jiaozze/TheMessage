using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardName
{
	ZHUAN_YI = 0,
	SUO_DING = 1,
	SHI_PO = 2,
	JIE_HUO = 3,
	DIAO_HU_LI_SHAN = 4,
	WEI_XIAN_QING_BAO = 5,
	GONG_KAI_WEN_BEN = 6,
	ZENG_YUAN = 7,
	SHAO_HUI = 8,
	PO_YI = 9,
	DIAO_BAO = 10,
	LI_JIAN = 11,
	JI_MI_WEN_JIAN = 12,
}

public enum CardColor
{
	BLACK_OR_GREAN = 0,
	RED = 1,
	BLUE = 2,
}

public enum CardTransmitType
{
	TEXT = 0,
	SECRET = 1,
	NONSTOP = 2,
}

public enum TestAction
{
	DISCARD_1 = 0,
	DRAW_1 = 1,
	DRAW_2 = 2,
	BE_TAKEN_AWAY_1 = 3,
	I_AM_SPY = 4,
	I_AM_GOOD_MAN = 5,
	I_AM_UNDERCOVER = 6,
	LISTEN_TO_WIND = 7, // 秘密下达-听风
	WATCH_RAIN = 8, // 秘密下达-看雨
	SUNSET = 9, // 秘密下达-日落
}
public class CardFS 
{
	public int id;
	public CardName name;
	public CardColor color;
	public CardTransmitType transType;
	public Dictionary<CardColor, List<TestAction>> test = new Dictionary<CardColor, List<TestAction>>();

	public bool isHand;

}
