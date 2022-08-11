using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public bool dir;
    public GameUI gameUI;

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public Dictionary<int, CardFS> cardsHand = new Dictionary<int, CardFS>(); //<id, card>

    public PhaseEnum curPhase { get; private set; }
    public SecretTaskEnum task { get; private set; }

    public int CurTurnPlayerId { get; private set; }
    public int CurMessagePlayerId { get; private set; }
    public int CurWaitingPlayerId { get; private set; }

    private int _SelectCardId = -1;
    public int SelectCardId
    {
        get { return _SelectCardId; }
        set
        {
            if(gameUI.Cards.ContainsKey(_SelectCardId)) gameUI.Cards[_SelectCardId].OnSelect(false);
            if(_SelectCardId == value)
            {
                _SelectCardId = -1;
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
    private int _SelectPlayerId = -1;
    public int SelectPlayerId
    {
        get { return _SelectPlayerId; }
        set
        {
            // ȡ��ѡ�����
            if(value == -1)
            {
                if (gameUI.Players.ContainsKey(_SelectPlayerId)) gameUI.Players[_SelectPlayerId].OnSelect(false);
                _SelectPlayerId = value;
            }
            // �жϳ���ʱѡ�����
            else if (cardsHand.ContainsKey(_SelectCardId))
            {
                switch(cardsHand[_SelectCardId].cardName)
                {
                    case CardNameEnum.Shi_Tan:
                        if(gameUI.Players.ContainsKey(_SelectPlayerId)) gameUI.Players[_SelectPlayerId].OnSelect(false);
                        if(_SelectPlayerId == value)
                        {
                            _SelectPlayerId = -1;
                        }
                        else if (value == 0)
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
    public void SetDeckNum(int num)
    {
        DeckNum = num;
        gameUI.SetDeckNum(num);
    }



    public bool IsCardAvailable(int color, int num)
    {
        return true;
    }

    public Color GetColorById(int colorId)
    {
        Color color = Color.black;
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

        string nunStr = "";

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
        players[0].playerColor = playerColor;
        gameUI.InitPlayers(player_num);

        InitCards(new List<CardFS>());
        gameUI.InitCards(0);
        //gameUI.AddMsg(string.Format("������{0}����, {1}", cards.Count, GetCardsInfo(cards)));

    }
    // �Լ�����
    public void OnPlayerDrawCards(List<CardFS> cards)
    {
        string cardInfo = "";
        foreach (var card in cards)
        {
            cardsHand[card.id] = card;
            //cardInfo += GetCardInfo((int)card.Color, (int)card.Num) + ",";
        }
        //DeckNum = DeckNum - 1;
        //SetDeckNum(DeckNum);
        int total = players[0].DrawCard(cards.Count);
        gameUI.DrawCards(cards);
        if (gameUI.Players[0] != null) gameUI.Players[0].OnDrawCard(total, cards.Count);
        gameUI.AddMsg(string.Format("������{0}����; {1}", cards.Count, cardInfo));

    }
    //������ɫ����
    public void OnOtherDrawCards(int id, int num)
    {
        int total = players[id].DrawCard(num);
        if (gameUI.Players[id] != null)
        {
            gameUI.Players[id].OnDrawCard(total, num);
        }
        gameUI.AddMsg(string.Format("{0}���������{1}����", id, num));
    }
    // ֪ͨ�ͻ��ˣ���˭���ĸ��׶���
    public void OnTurn(int playerId, int messagePlayerId, int waitingPlayerId, PhaseEnum phase, int waitSecond, uint seqId)
    {
        if(waitingPlayerId == 0)
        {
            this.seqId = seqId;
        }    
        curPhase = phase;
        if (CurTurnPlayerId != playerId && gameUI.Players[CurTurnPlayerId] != null)
        {
            gameUI.Players[CurTurnPlayerId].OnTurn(false);
            gameUI.Players[playerId]?.OnTurn(true);
        }
        CurTurnPlayerId = playerId;

        if (CurMessagePlayerId != messagePlayerId && gameUI.Players[CurTurnPlayerId] != null)
        {
            gameUI.Players[CurMessagePlayerId].OnMessageTurnTo(false);
            gameUI.Players[messagePlayerId]?.OnMessageTurnTo(true);
        }
        CurMessagePlayerId = messagePlayerId;

        if (CurWaitingPlayerId != messagePlayerId && gameUI.Players[CurTurnPlayerId] != null)
        {
            gameUI.Players[CurWaitingPlayerId].DoWaiting(0);
            gameUI.Players[waitingPlayerId]?.DoWaiting(waitSecond);
        }
        CurWaitingPlayerId = waitingPlayerId;

        //gameUI.SetTurn();
        //gameUI.AddMsg(string.Format("{0}����һغϿ�ʼ", id));
    }

    public void OnDeckNumTo(int num)
    {
        DeckNum = num;
        SetDeckNum(num);
    }

    public void OnDisCard(int playerId, int id, int color, int num, int colorEx)
    {
        string log = "";
        if (playerId > 999)
        {
            log = "���ƶѶ�����һ���ƣ�{0}";
        }
        else if (playerId == 0)
        {
            log = "������һ���ƣ�{0}";
        }
        else
        {
            log = "" + playerId + "����Ҵ��һ���ƣ�{0}";
        }
        gameUI.AddMsg(string.Format(log, GetCardInfo(color, num)));
    }
    #endregion


    #region ���������������
    public void UseCard()
    {
        if (SelectCardId != -1 && cardsHand.ContainsKey(SelectCardId))
        {
            if(cardsHand[SelectCardId].cardName == CardNameEnum.Shi_Tan)
            {
                if(SelectPlayerId != -1 && SelectPlayerId != 0)
                {
                    ProtoHelper.SendUserCardMessage_ShiTan(SelectCardId, SelectPlayerId, this.seqId);
                }
                else
                {
                    Debug.LogError("��ѡ����ȷ����̽Ŀ��");
                }
            }
        }
    }
    public void Discard()
    {

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
