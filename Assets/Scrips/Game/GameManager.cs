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
            // 取消选中玩家
            if(value == -1)
            {
                if (gameUI.Players.ContainsKey(_SelectPlayerId)) gameUI.Players[_SelectPlayerId].OnSelect(false);
                _SelectPlayerId = value;
            }
            // 判断出牌时选中玩家
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
                            Debug.LogError("不能选自己作为试探的目标");
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
    //public int topColor; // 黑色牌声明的颜色
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
                    nunStr = "跳过";
                    break;
                case 11:
                    nunStr = "反向";
                    break;
                case 12:
                    nunStr = "+2";
                    break;
                case 13:
                    nunStr = "变色";
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

    #region 服务器消息处理

    // 通知客户端：初始化游戏
    public void OnReceiveGameStart(int player_num, PlayerColorEnum playerColor, SecretTaskEnum secretTask)
    {
        task = secretTask;

        InitPlayers(player_num);
        players[0].playerColor = playerColor;
        gameUI.InitPlayers(player_num);

        InitCards(new List<CardFS>());
        gameUI.InitCards(0);
        //gameUI.AddMsg(string.Format("你摸了{0}张牌, {1}", cards.Count, GetCardsInfo(cards)));

    }
    // 自己摸牌
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
        gameUI.AddMsg(string.Format("你摸了{0}张牌; {1}", cards.Count, cardInfo));

    }
    //其他角色摸牌
    public void OnOtherDrawCards(int id, int num)
    {
        int total = players[id].DrawCard(num);
        if (gameUI.Players[id] != null)
        {
            gameUI.Players[id].OnDrawCard(total, num);
        }
        gameUI.AddMsg(string.Format("{0}号玩家摸了{1}张牌", id, num));
    }
    // 通知客户端，到谁的哪个阶段了
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
        //gameUI.AddMsg(string.Format("{0}号玩家回合开始", id));
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
            log = "从牌堆顶翻出一张牌：{0}";
        }
        else if (playerId == 0)
        {
            log = "你打出了一张牌：{0}";
        }
        else
        {
            log = "" + playerId + "号玩家打出一张牌：{0}";
        }
        gameUI.AddMsg(string.Format(log, GetCardInfo(color, num)));
    }
    #endregion


    #region 向服务器发送请求
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
                    Debug.LogError("请选择正确的试探目标");
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
    Killer = 0, // 你的回合中，一名红色和蓝色情报合计不少于2张的人死亡
    Stealer = 1, // 你的回合中，有人宣胜，则你代替他胜利
    Collector = 2, // 你获得3张红色情报或者3张蓝色情报
}

public enum PhaseEnum
{
    Draw_Phase = 0,   // 摸牌阶段
    Main_Phase = 1,   // 出牌阶段
    Send_Phase = 2,   // 传递阶段
    Fight_Phase = 3,   // 争夺阶段
    Receive_Phase = 4, // 接收阶段
}
