public enum PlayerColorEnum
{
    Green = 0, // 对于身份，则是绿色（神秘人）；对于卡牌，则是黑色
    Red = 1,   // 红色
    Blue = 2,  // 蓝色
    Unknown = 3, //未知
}

public class Player
{
    public int playerId;
    public PlayerColorEnum playerColor = PlayerColorEnum.Unknown;
    public int cardCount;
    //public int 

    public int DrawCard(int count)
    {
        cardCount = cardCount + count;
        return cardCount;
    }
}
