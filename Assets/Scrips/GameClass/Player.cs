public enum PlayerColorEnum
{
    Green = 0, // ������ݣ�������ɫ�������ˣ������ڿ��ƣ����Ǻ�ɫ
    Red = 1,   // ��ɫ
    Blue = 2,  // ��ɫ
    Unknown = 3, //δ֪
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
