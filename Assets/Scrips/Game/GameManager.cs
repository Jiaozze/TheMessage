using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public int SelfPlayerId = 0;
    public bool dir;
    public GameUI gameUI;

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public Dictionary<int, CardFS> cardsHand = new Dictionary<int, CardFS>(); //<id, card>

    public PhaseEnum curPhase { get; private set; }
    public SecretTaskEnum task { get; private set; }

    public int CurTurnPlayerId { get; private set; }
    public int CurMessagePlayerId { get; private set; }
    public int CurWaitingPlayerId { get; private set; }
    public bool IsBingShiTan { get; private set; }

    public int SelectCardId
    {
        get { return _SelectCardId; }
        set
        {
            if (gameUI.Cards.ContainsKey(_SelectCardId)) gameUI.Cards[_SelectCardId].OnSelect(false);
            if (_SelectCardId == value)
            {
                _SelectCardId = -1;
            }
            else if(value == -1)
            {
                _SelectCardId = value;
            }
            else
            {
                _SelectCardId = value;
                gameUI.Cards[_SelectCardId].OnSelect(true);
            }
            SelectPlayerId = -1;
            Debug.Log("cardId" + _SelectCardId);
        }
    }
    private int _SelectCardId = -1;

    public int SelectPlayerId
    {
        get { return _SelectPlayerId; }
        set
        {
            // ȡ��ѡ�����
            if (value == -1)
            {
                if (gameUI.Players.ContainsKey(_SelectPlayerId)) gameUI.Players[_SelectPlayerId].OnSelect(false);
                _SelectPlayerId = value;
            }
            // �жϳ���ʱѡ�����
            else if (cardsHand.ContainsKey(_SelectCardId))
            {
                switch (cardsHand[_SelectCardId].cardName)
                {
                    case CardNameEnum.Shi_Tan:
                        if (gameUI.Players.ContainsKey(_SelectPlayerId)) gameUI.Players[_SelectPlayerId].OnSelect(false);
                        if (_SelectPlayerId == value)
                        {
                            _SelectPlayerId = -1;
                        }
                        else if (value == SelfPlayerId)
                        {
                            Debug.LogError("����ѡ�Լ���Ϊ��̽��Ŀ��");
                        }
                        else
                        {
                            _SelectPlayerId = value;
                            gameUI.Players[_SelectPlayerId].OnSelect(true);
                        }
                        break;
                }
            }
            Debug.Log("_SelectPlayerId" + _SelectPlayerId);
        }
    }
    private int _SelectPlayerId = -1;

    private uint seqId;
    //public int topColor; // ��ɫ����������ɫ
    //public int topCardCount;
    //public int wantColor;

    public int onTurnPlayerId = -1;
    //public 
    private static GameManager gameManager;
    private int DeckNum;

    public static GameManager Singleton
    {
        get
        {
            if (gameManager == null)
            {
                gameManager = new GameManager();
                gameManager.Init();
            }
            return gameManager;
        }
    }
    private GameManager()
    {

    }

    public void Init()
    {
        GameObject windowGo = GameObject.Find("GameUI");
        if (gameUI == null && windowGo != null)
        {
            gameUI = windowGo.GetComponent<GameUI>();
            if (gameUI == null)
            {
                gameUI = windowGo.AddComponent<GameUI>();
            }
        }
        else
        {
            //TODO
        }
    }

    private void InitDatas()
    {

    }
    public void InitPlayers(int num)
    {
        players.Clear();
        for (int i = 0; i < num; i++)
        {
            Player player = new Player();
            players.Add(i, player);
        }
    }
    private void InitCards(List<CardFS> cards)
    {
        foreach (var card in cards)
        {
            card.isHand = true;
            this.cardsHand.Add(card.id, card);
        }
    }

    public PlayerColorEnum GetPlayerColor()
    {
        return players[SelfPlayerId].playerColor;
    }


    public bool IsCardAvailable(int color, int num)
    {
        return true;
    }

    public Color GetColorById(int colorId)
    {
        Color color;
        switch (colorId)
        {
            case 0:
                color = Color.black;
                break;
            case 1:
                color = Color.red;
                break;
            case 2:
                color = Color.green;
                break;
            case 3:
                color = Color.yellow;
                break;
            case 4:
                color = Color.blue;
                break;

            default:
                color = Color.gray;

                break;
        }
        return color;
    }

    public string GetCardInfo(int colorId, int num)
    {
        Color color1 = GetColorById(colorId);

        string nunStr;

        if (num < 10)
        {
            nunStr = "" + num;
        }
        else
        {
            switch (num)
            {
                case 10:
                    nunStr = "����";
                    break;
                case 11:
                    nunStr = "����";
                    break;
                case 12:
                    nunStr = "+2";
                    break;
                case 13:
                    nunStr = "��ɫ";
                    break;
                case 14:
                    nunStr = "+4";
                    break;
                default:
                    break;
            }
        }

        string colorStr = "<color=#" + ColorUtility.ToHtmlStringRGBA(color1) + ">" + num + "</color>";
        return colorStr;
    }

    public string GetCardsInfo(List<UICard> cards)
    {
        string ret = "";
        foreach (var card in cards)
        {
            //ret += GetCardInfo(card.color, card.num);
            //ret += ",";
        }
        return ret;
    }

    #region ��������Ϣ����

    // ֪ͨ�ͻ��ˣ���ʼ����Ϸ
    public void OnReceiveGameStart(int player_num, PlayerColorEnum playerColor, SecretTaskEnum secretTask)
    {
        task = secretTask;

        InitPlayers(player_num);
        players[SelfPlayerId].playerColor = playerColor;
        gameUI.InitPlayers(player_num);

        InitCards(new List<CardFS>());
        gameUI.InitCards(0);
        //gameUI.AddMsg(string.Format("������{0}����, {1}", cards.Count, GetCardsInfo(cards)));

    }
    // �Լ�����
    public void OnReceivePlayerDrawCards(List<CardFS> cards)
    {
        string cardInfo = "";
        foreach (var card in cards)
        {
            cardsHand[card.id] = card;
            //cardInfo += GetCardInfo((int)card.Color, (int)card.Num) + ",";
        }
        //DeckNum = DeckNum - 1;
        //SetDeckNum(DeckNum);
        int total = players[SelfPlayerId].DrawCard(cards.Count);
        gameUI.DrawCards(cards);
        if (gameUI.Players[SelfPlayerId] != null) gameUI.Players[SelfPlayerId].OnDrawCard(total, cards.Count);
        gameUI.AddMsg(string.Format("������{0}����; {1}", cards.Count, cardInfo));

    }
    //�������
    public void OnReceiveDiscards(int playerId, List<CardFS> cards)
    {
        if (players.ContainsKey(playerId))
        {
            players[playerId].cardCount = players[playerId].cardCount - cards.Count;
        }
        if (gameUI.Players.ContainsKey(playerId))
        {
            gameUI.Players[playerId].Discard(cards);
        }

        if (playerId == SelfPlayerId)
        {
            foreach (var card in cards)
            {
                int cardId = card.id;
                if (cardsHand.ContainsKey(cardId)) cardsHand.Remove(cardId);
                if (gameUI.Cards.ContainsKey(cardId))
                {
                    gameUI.Cards[cardId].OnDiscard();
                    gameUI.Cards.Remove(cardId);
                }
            }
        }

    }
    //������ɫ����
    public void OnReceiveOtherDrawCards(int id, int num)
    {
        int total = players[id].DrawCard(num);
        if (gameUI.Players[id] != null)
        {
            gameUI.Players[id].OnDrawCard(total, num);
        }
        gameUI.AddMsg(string.Format("{0}���������{1}����", id, num));
    }
    // ֪ͨ�ͻ��ˣ���˭���ĸ��׶���
    public void OnReceiveTurn(int playerId, int messagePlayerId, int waitingPlayerId, PhaseEnum phase, int waitSecond, uint seqId)
    {
        Debug.Log("____________________OnTurn:" + playerId + "," + messagePlayerId + "," + waitingPlayerId);
        if (waitingPlayerId == 0)
        {
            this.seqId = seqId;
        }
        curPhase = phase;
        if (CurTurnPlayerId != playerId)
        {
            gameUI.Players[CurTurnPlayerId].OnTurn(false);
        }

        if (gameUI.Players[CurTurnPlayerId] != null)
        {
            gameUI.Players[playerId]?.OnTurn(true);
        }
        CurTurnPlayerId = playerId;

        if (CurMessagePlayerId != messagePlayerId)
        {
            gameUI.Players[CurMessagePlayerId].OnMessageTurnTo(false);
        }
        if (gameUI.Players[CurTurnPlayerId] != null)
        {
            gameUI.Players[messagePlayerId]?.OnMessageTurnTo(true);
        }
        CurMessagePlayerId = messagePlayerId;

        if (CurWaitingPlayerId != messagePlayerId)
        {
            gameUI.Players[CurWaitingPlayerId].OnWaiting(0);
        }
        if (gameUI.Players[CurTurnPlayerId] != null)
        {
            gameUI.Players[waitingPlayerId]?.OnWaiting(waitSecond);
        }
        CurWaitingPlayerId = waitingPlayerId;

        //gameUI.SetTurn();
        //gameUI.AddMsg(string.Format("{0}����һغϿ�ʼ", id));
    }

    // ֪ͨ�ͻ��ˣ�˭��˭ʹ������̽
    public void OnRecerveUseShiTan(int user, int targetUser, int cardId = 0)
    {
        CardFS card = null;
        if (players.ContainsKey(user))
        {
            players[user].cardCount = players[user].cardCount - 1;
        }
        if (user == SelfPlayerId && cardsHand.ContainsKey(cardId))
        {
            card = cardsHand[cardId];
            cardsHand.Remove(cardId);
        }
        //Debug.LogError("________________ OnRecerveUseShiTan," + cardId);
        gameUI.OnUseCard(user, targetUser, card);
    }
    // ����̽��չʾ��̽�����ȴ���Ӧ
    public void OnReceiveShowShiTan(int user, int targetUser, CardFS card, int waitingTime, uint seqId)
    {
        this.seqId = seqId;
        if (gameUI.Players.ContainsKey(CurWaitingPlayerId))
        {
            gameUI.Players[CurWaitingPlayerId].OnWaiting(0);
        }
        if (gameUI.Players.ContainsKey(targetUser))
        {
            gameUI.Players[targetUser].OnWaiting(waitingTime);
        }
        //�Լ��Ǳ�ʹ���ߣ�չʾ
        if (targetUser == SelfPlayerId)
        {
            IsBingShiTan = true;
            gameUI.ShowShiTanInfo(card, waitingTime);
        }
    }
    // ����̽��ִ����̽
    public void OnReceiveExcuteShiTan(int playerId, bool isDrawCard)
    {
        if(playerId == SelfPlayerId)
        {
            gameUI.HideShiTanInfo();
        }
    }
    // ֪ͨ�ͻ���ʹ�����յĽ��
    public void OnRecerveUseLiYou(int user, int target, CardFS card, bool isJoinHand)
    {
        if (players.ContainsKey(user))
        {
            players[user].cardCount = players[user].cardCount - 1;
        }
        if (user == SelfPlayerId && cardsHand.ContainsKey(card.id))
        {
            cardsHand.Remove(card.id);
        }
        gameUI.OnUseCard(user, target, card);

    }
    #endregion


    #region ���������������
    public void SendEndWaiting()
    {
        ProtoHelper.SendEndWaiting(seqId);
    }

    public void SendUseCard()
    {
        if (SelectCardId != -1 && cardsHand.ContainsKey(SelectCardId))
        {
            //ʹ����̽
            if (cardsHand[SelectCardId].cardName == CardNameEnum.Shi_Tan)
            {
                if (SelectPlayerId != -1 && SelectPlayerId != 0)
                {
                    ProtoHelper.SendUseCardMessage_ShiTan(SelectCardId, SelectPlayerId, this.seqId);
                }
                else
                {
                    Debug.LogError("��ѡ����ȷ����̽Ŀ��");
                }
            }
            //ʹ������
            else if (cardsHand[SelectCardId].cardName == CardNameEnum.Li_You)
            {
                if (SelectPlayerId != -1 && SelectPlayerId != 0)
                {
                    ProtoHelper.SendUseCardMessage_LiYou(SelectCardId, SelectPlayerId, this.seqId);
                }
                else
                {
                    Debug.LogError("��ѡ����ȷ����̽Ŀ��");
                }
            }
        }
        SelectCardId = -1;
    }
    public void SendDoShiTan(int cardId)
    {
        ProtoHelper.SendDoShiTan(cardId, seqId);
        SelectCardId = -1;
    }

    public void DrawCard()
    {
        //ProtoHelper.SendDiscardMessage(0, 0);

    }

    #endregion
}

public enum SecretTaskEnum
{
    Killer = 0, // ��Ļغ��У�һ����ɫ����ɫ�鱨�ϼƲ�����2�ŵ�������
    Stealer = 1, // ��Ļغ��У�������ʤ�����������ʤ��
    Collector = 2, // ����3�ź�ɫ�鱨����3����ɫ�鱨
}

public enum PhaseEnum
{
    Draw_Phase = 0,   // ���ƽ׶�
    Main_Phase = 1,   // ���ƽ׶�
    Send_Phase = 2,   // ���ݽ׶�
    Fight_Phase = 3,   // ����׶�
    Receive_Phase = 4, // ���ս׶�
}
