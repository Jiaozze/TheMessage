using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardNameEnum
{
	Cheng_Qing = 0, // 澄清
	Shi_Tan = 1, // 试探
	Bi_Po = 2, // 逼迫
	Li_You = 3, // 利诱
	Ping_Heng = 4, // 平衡
	Po_Yi = 5, // 破译
	Jie_Huo = 6, // 截获
	Diao_Bao = 7, // 掉包
	Wu_Dao = 8, // 误导
}

public enum CardColorEnum
{
	Black = 0,// 对于身份，则是绿色（神秘人）；对于卡牌，则是黑色
	Red = 1, // 红色
	Blue = 2,  // 蓝色
}

public enum DirectionEnum
{
	Up = 0,    // 向上
	Left = 1,  // 向左
	Right = 2, // 向右

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
	public int id = -1;
	public CardNameEnum name;
	public List<CardColorEnum> color;
	public DirectionEnum direction;
	//public CardTransmitType transType;
	//public Dictionary<CardColorEnum, List<TestAction>> test = new Dictionary<CardColorEnum, List<TestAction>>();

	public bool isHand;

	public CardFS(card card)
    {
		id = (int)card.CardId;
		name = (CardNameEnum)card.CardType;
		color = new List<CardColorEnum>();
		foreach(var color in card.CardColor)
        {
			this.color.Add((CardColorEnum)color);
        }
		direction = (DirectionEnum)card.CardDir;
    }
}
