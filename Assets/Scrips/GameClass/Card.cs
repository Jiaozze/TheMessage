using System.Collections.Generic;

public enum CardNameEnum
{
    None = -1,
    ChengQing = 0, // 澄清
    ShiTan = 1, // 试探
    WeiBi = 2, // 威逼
    LiYou = 3, // 利诱
    PingHeng = 4, // 平衡
    PoYi = 5, // 破译
    JieHuo = 6, // 截获
    DiaoBao = 7, // 掉包
    WuDao = 8, // 误导
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

public class CardFS
{
    public int id = -1;
    public CardNameEnum cardName;
    public List<CardColorEnum> color;
    public DirectionEnum direction;
    public bool canLock;
    //public CardTransmitType transType;
    //public Dictionary<CardColorEnum, List<TestAction>> test = new Dictionary<CardColorEnum, List<TestAction>>();
    public List<PlayerColorEnum> shiTanColor;

    public CardFS(card card)
    {
        if (card == null) return;
        this.id = (int)card.CardId;
        this.cardName = (CardNameEnum)card.CardType;
        this.color = new List<CardColorEnum>();
        foreach (var color in card.CardColor)
        {
            this.color.Add((CardColorEnum)color);
        }
        direction = (DirectionEnum)card.CardDir;
        canLock = card.CanLock;
        if (cardName == CardNameEnum.ShiTan)
        {
            // Debug.LogError("==================" + card.CardId);
            this.shiTanColor = new List<PlayerColorEnum>();
            foreach (var color in card.WhoDrawCard)
            {
                shiTanColor.Add((PlayerColorEnum)color);
            }

        }
    }

    public string GetCardInfo()
    {
        string s = " ";
        foreach (var color in color)
        {
            s = s + LanguageUtils.GetColorName(color);
        }
        s += LanguageUtils.GetCardName(cardName);
        return s;
    }
}
