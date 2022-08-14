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
            else if (value == -1)
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
            if (gameUI.Players.ContainsKey(_SelectPlayerId)) gameUI.Players[_SelectPlayerId].OnSelect(false);
            if (_SelectPlayerId == value)
            {
                _SelectPlayerId = -1;
            }

            // 取消选中玩家
            if (value == -1)
            {
                _SelectPlayerId = value;
            }
            // 判断出牌时选中玩家
            else if (cardsHand.ContainsKey(_SelectCardId))
            {
                switch (cardsHand[_SelectCardId].cardName)
                {
                    case CardNameEnum.Ping_Heng:
                    case CardNameEnum.Shi_Tan:
                        if (value == SelfPlayerId)
                        {
                            string name = LanguageUtils.GetCardName(cardsHand[_SelectCardId].cardName);
                            Debug.LogError("不能选自己作为" + name + "的目标");
                        }
                        else if (gameUI.Players.ContainsKey(value))
                        {
                            _SelectPlayerId = value;
                            gameUI.Players[_SelectPlayerId].OnSelect(true);
                        }
                        break;
                    case CardNameEnum.Li_You:
                        if (gameUI.Players.ContainsKey(value))
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
            Player player = new Player(i);
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
        return players[SelfPlayerId].playerColor[0];
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

    private void OnCardUse(int user, int target, CardFS cardUsed)
    {
        if (players.ContainsKey(user))
        {
            players[user].cardCount = players[user].cardCount - 1;
        }
        if (user == SelfPlayerId && cardsHand.ContainsKey(cardUsed.id))
        {
            cardsHand.Remove(cardUsed.id);
        }
        gameUI.OnUseCard(user, target, cardUsed);
    }

    #region 服务器消息处理

    // 通知客户端：初始化游戏
    public void OnReceiveGameStart(int player_num, PlayerColorEnum playerColor, SecretTaskEnum secretTask)
    {
        task = secretTask;

        InitPlayers(player_num);
        players[SelfPlayerId].playerColor = new List<PlayerColorEnum>() { playerColor };
        gameUI.InitPlayers(player_num);

        InitCards(new List<CardFS>());
        gameUI.InitCards(0);
        //gameUI.AddMsg(string.Format("你摸了{0}张牌, {1}", cards.Count, GetCardsInfo(cards)));
    }
    // 自己摸牌
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
        gameUI.AddMsg(string.Format("你摸了{0}张牌; {1}", cards.Count, cardInfo));

    }
    //玩家弃牌
    public void OnReceiveDiscards(int playerId, List<CardFS> cards)
    {
        //Debug.LogError("" + playerId + "号玩家弃牌 " +  cards.Count);
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
    //其他角色摸牌
    public void OnReceiveOtherDrawCards(int id, int num)
    {
        int total = players[id].DrawCard(num);
        if (gameUI.Players[id] != null)
        {
            gameUI.Players[id].OnDrawCard(total, num);
        }
        gameUI.AddMsg(string.Format("{0}号玩家摸了{1}张牌", id, num));
    }
    // 通知客户端，到谁的哪个阶段了
    public void OnReceiveTurn(int playerId, int messagePlayerId, int waitingPlayerId, PhaseEnum phase, int waitSecond, uint seqId)
    {
        //Debug.Log("____________________OnTurn:" + playerId + "," + messagePlayerId + "," + waitingPlayerId);
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
        //gameUI.AddMsg(string.Format("{0}号玩家回合开始", id));
    }

    // 通知客户端，谁对谁使用了试探
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
    // 向被试探者展示试探，并等待回应
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
        //自己是被使用者，展示
        if (targetUser == SelfPlayerId)
        {
            IsBingShiTan = true;
            gameUI.ShowShiTanInfo(card, waitingTime);
        }
    }
    // 被试探者执行试探
    public void OnReceiveExcuteShiTan(int playerId, bool isDrawCard)
    {
        if (playerId == SelfPlayerId)
        {
            gameUI.HideShiTanInfo();
        }
    }
    // 通知客户端使用利诱的结果
    public void OnRecerveUseLiYou(int user, int target, CardFS cardUsed, CardFS card, bool isJoinHand)
    {
        OnCardUse(user, target, cardUsed);

        gameUI.ShowTopCard(card);
        if (isJoinHand)
        {
            if (players.ContainsKey(user))
            {
                players[user].cardCount += 1;
            }
        }
        else
        {
            if (players.ContainsKey(user))
            {
                players[user].AddMessage(card);
            }
        }
    }
    // 通知客户端使用平衡的结果 //弃牌部分走 OnReceiveDiscards
    public void OnReceiveUsePingHeng(int user, int target, CardFS cardUsed)
    {
        OnCardUse(user, target, cardUsed);
    }
    #endregion


    #region 向服务器发送请求
    public void SendEndWaiting()
    {
        ProtoHelper.SendEndWaiting(seqId);
    }

    public void SendUseCard()
    {
        if (SelectCardId != -1 && cardsHand.ContainsKey(SelectCardId))
        {
            CardNameEnum card = cardsHand[SelectCardId].cardName;
            switch (card)
            {
                //使用试探
                case CardNameEnum.Shi_Tan:
                    if (SelectPlayerId != -1 && SelectPlayerId != 0)
                    {
                        ProtoHelper.SendUseCardMessage_ShiTan(SelectCardId, SelectPlayerId, this.seqId);
                    }
                    else
                    {
                        Debug.LogError("请选择正确的试探目标");
                    }
                    break;
                //使用利诱
                case CardNameEnum.Li_You:
                    if (SelectPlayerId != -1)
                    {
                        ProtoHelper.SendUseCardMessage_LiYou(SelectCardId, SelectPlayerId, this.seqId);
                    }
                    else
                    {
                        Debug.LogError("请选择正确的利诱目标");
                    }
                    break;
                //使用平衡
                case CardNameEnum.Ping_Heng:
                    if (SelectPlayerId != -1)
                    {
                        ProtoHelper.SendUseCardMessage_PingHeng(SelectCardId, SelectPlayerId, this.seqId);
                    }
                    else
                    {
                        Debug.LogError("请选择正确的平衡目标");
                    }
                    break;
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
