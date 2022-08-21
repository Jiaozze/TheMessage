using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public static int SelfPlayerId = 0;
    public GameUI gameUI;

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public Dictionary<int, CardFS> cardsHand = new Dictionary<int, CardFS>(); //<id, card>

    public PhaseEnum curPhase { get; private set; }
    public SecretTaskEnum task { get; private set; }

    public int CurTurnPlayerId { get; private set; }
    public int CurMessagePlayerId { get; private set; }
    public int CurWaitingPlayerId { get; private set; }

    public bool IsBingShiTan { get; private set; }

    #region 特殊状态 有人求澄清 传情报时选择锁定目标 死亡给牌
    public int IsWaitSaving { get; private set; }
    public bool IsWaitLock { get; private set; }
    public bool IsWaitGiveCard { get; private set; }

    public List<int> cardsToGive = new List<int>();
    #endregion

    public List<int> lockedPlayer = null;
    public int SelectCardId
    {
        get { return _SelectCardId; }
        set
        {
            if (IsWaitLock)
            {
                Debug.LogError("发情报选择是否锁定时，点牌无效");
                return;
            }
            if (IsWaitGiveCard)
            {
                if (cardsToGive.Contains(value))
                {
                    cardsToGive.Remove(value);
                    gameUI.Cards[value].OnSelect(false);
                }
                else if (cardsToGive.Count >= 3)
                {
                    gameUI.Cards[0].OnSelect(false);
                    cardsToGive.RemoveAt(0);
                    cardsToGive.Add(value);
                    gameUI.Cards[value].OnSelect(true);
                }
                else
                {
                    cardsToGive.Add(value);
                    gameUI.Cards[value].OnSelect(true);
                }
                return;
            }

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
            gameUI.HidePlayerMessageInfo();
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
            // 开始传情报时，选择传递目标和锁定目标 // 等待死亡时给三张牌
            else if (curPhase == PhaseEnum.Send_Start_Phase || IsWaitGiveCard)
            {
                if (value == SelfPlayerId)
                {
                    Debug.LogError("不能选自己作为目标");
                }
                else if (gameUI.Players.ContainsKey(value))
                {
                    _SelectPlayerId = value;
                    gameUI.Players[_SelectPlayerId].OnSelect(true);
                }
            }
            // 判断出牌时选中玩家
            else if (cardsHand.ContainsKey(_SelectCardId))
            {
                switch (cardsHand[_SelectCardId].cardName)
                {
                    case CardNameEnum.Wei_Bi:
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
                    case CardNameEnum.Cheng_Qing:
                        if (gameUI.Players.ContainsKey(value) && players[value].GetMessageCount(CardColorEnum.Black) > 0)
                        {
                            _SelectPlayerId = value;
                            gameUI.Players[_SelectPlayerId].OnSelect(true);
                        }
                        gameUI.ShowPlayerMessageInfo(_SelectPlayerId, true);
                        break;
                }
            }
            Debug.Log("_SelectPlayerId" + _SelectPlayerId);
        }
    }

    private int _SelectPlayerId = -1;

    public uint seqId;
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
            this.cardsHand.Add(card.id, card);
        }
    }

    public CardFS GetCardSelect()
    {
        if (cardsHand.ContainsKey(SelectCardId))
        {
            return cardsHand[SelectCardId];
        }
        else
        {
            return null;
        }
    }

    public PlayerColorEnum GetPlayerColor()
    {
        return players[SelfPlayerId].playerColor[0];
    }

    private void OnCardUse(int user, CardFS cardUsed, int target = -1)
    {
        if (players.ContainsKey(user))
        {
            players[user].cardCount = players[user].cardCount - 1;
        }
        if (user == SelfPlayerId && cardsHand.ContainsKey(cardUsed.id))
        {
            cardsHand.Remove(cardUsed.id);
            SelectCardId = -1;
        }
        else if (user == SelfPlayerId && !cardsHand.ContainsKey(cardUsed.id))
        {
            Debug.LogError("no card in hand," + cardUsed.id);
        }
        gameUI.OnUseCard(user, target, cardUsed);
        string targetInfo;
        targetInfo = target == -1 ? "" : "对" + target + "号玩家";
        gameUI.AddMsg(string.Format("{0}号玩家{1}使用了{2};", user, targetInfo, LanguageUtils.GetCardName(cardUsed.cardName)));
    }

    private void OnCardSend(int playerId, int cardId, int targetId, List<int> lockIds, DirectionEnum dir)
    {
        int user = playerId;
        if (players.ContainsKey(user))
        {
            players[user].cardCount = players[user].cardCount - 1;
        }
        if (user == SelfPlayerId && cardsHand.ContainsKey(cardId))
        {
            cardsHand.Remove(cardId);
            SelectCardId = -1;
        }
        else if (user == SelfPlayerId && !cardsHand.ContainsKey(cardId))
        {
            Debug.LogError("no card in hand," + cardId);
        }
        lockedPlayer = lockIds;
        gameUI.OnCardSend(playerId, cardId);
        gameUI.SetLock(lockIds);
        string dirStr = "";
        if (dir == DirectionEnum.Up) dirStr = "上";
        else if (dir == DirectionEnum.Left) dirStr = "左";
        else if (dir == DirectionEnum.Right) dirStr = "右";

        gameUI.AddMsg(string.Format("{0}号玩家向{1}号玩家传出一张情报，方向向{2}", playerId, targetId, dirStr));
        foreach(var id in lockIds)
        {
            gameUI.AddMsg("" + id + "号玩家被锁定了");
        }
    }
    private void OnWait(int playerId, int waitSeconds)
    {
        if (gameUI.Players.ContainsKey(CurWaitingPlayerId))
        {
            gameUI.Players[CurWaitingPlayerId].OnWaiting(0);
        }
        if (gameUI.Players.ContainsKey(playerId))
        {
            gameUI.Players[playerId].OnWaiting(waitSeconds);
        }
        CurWaitingPlayerId = playerId;

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
        gameUI.SetTask(task);
        //gameUI.AddMsg(string.Format("你摸了{0}张牌, {1}", cards.Count, GetCardsInfo(cards)));
    }
    // 通知客户端，牌堆的剩余数量
    public void OnReceiveDeckNum(int num, bool shuffled)
    {
        DeckNum = num;
        gameUI.SetDeckNum(num);
    }
    // 自己摸牌
    public void OnReceivePlayerDrawCards(List<CardFS> cards)
    {
        string cardInfo = "";
        foreach (var card in cards)
        {
            cardsHand[card.id] = card;
            cardInfo += LanguageUtils.GetCardName(card.cardName) + ",";
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
        string cardInfo = "";
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

                cardInfo += LanguageUtils.GetCardName(card.cardName) + ",";
            }
            gameUI.DisCards(cards);
        }
        gameUI.AddMsg(string.Format("{0}号玩家弃了{1}张牌; {2}", playerId, cards.Count, cardInfo));

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
    public void OnReceiveTurn(int playerId, int messagePlayerId, int waitingPlayerId, PhaseEnum phase, int waitSecond, DirectionEnum messageCardDir, CardFS message, uint seqId)
    {
        IsWaitLock = false;
        IsWaitSaving = -1;
        gameUI.HideMessagingCard();
        gameUI.weiBiGiveCard.gameObject.SetActive(false);
        if (playerId != CurTurnPlayerId)
        {
            lockedPlayer = null;
            gameUI.ClearLock();
            gameUI.AddMsg(string.Format("{0}号玩家回合开始", playerId));
        }
        if (phase == PhaseEnum.Send_Start_Phase)
        {
            gameUI.InitMessageSenderPos(playerId);
            gameUI.AddMsg(string.Format("{0}号玩家开始传递情报", playerId));
        }
        else if (phase == PhaseEnum.Send_Phase)
        {
            gameUI.ShowMessagingCard(message, messagePlayerId, true);
            if (cardsHand.ContainsKey(message.id))
            {
                cardsHand.Remove(message.id);
                gameUI.DisCards(new List<CardFS>() { message });
            }
            //string dir;

            gameUI.AddMsg(string.Format("情报到达{0}号玩家，方向{1}", messagePlayerId, messageCardDir.ToString()));
        }
        else if (phase == PhaseEnum.Fight_Phase)
        {
            gameUI.ShowMessagingCard(message, messagePlayerId);
        }
        else if (phase == PhaseEnum.Receive_Phase)
        {
            players[messagePlayerId].AddMessage(message);
            gameUI.Players[messagePlayerId].RefreshMessage();
            gameUI.AddMsg(string.Format("{0}号玩家接收情报", messagePlayerId));
        }

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
        if (CurTurnPlayerId != SelfPlayerId)
        {
            gameUI.ShowWeiBiSelect(false);
        }

        if (CurMessagePlayerId != messagePlayerId)
        {
            gameUI.Players[CurMessagePlayerId].OnMessageTurnTo(false);
        }
        if (gameUI.Players[CurTurnPlayerId] != null)
        {
            gameUI.Players[messagePlayerId]?.OnMessageTurnTo(true);
        }
        CurMessagePlayerId = messagePlayerId;

        OnWait(waitingPlayerId, waitSecond);
        gameUI.ShowPhase();
        gameUI.RefreshIsCanCancel();
    }

    public void OnReceiveUseJieHuo(int user, CardFS cardUsed)
    {
        OnCardUse(user, cardUsed);
    }

    // 通知所有人传情报
    public void OnReceiveMessageSend(int playerId, int cardId, int targetId, List<int> locakIds, DirectionEnum dir)
    {
        OnCardSend(playerId, cardId, targetId, locakIds, dir);

    }

    // 通知所有人选择要接收情报（只有选择要收时有这条协议）
    public void OnReceiveMessageAccept(int playerId)
    {
        gameUI.OnMessageAccept(playerId);
    }
    // 通知所有人使用调包
    public void OnReceiveUseDiaoBao(int user, int cardUsedId, CardFS messageCard)
    {
        CardFS cardUsed;
        if(user == SelfPlayerId)
        {
            cardUsed = cardsHand[cardUsedId];
        }
        else
        {
            cardUsed = new CardFS(null);
            cardUsed.cardName = CardNameEnum.Diao_Bao;
            cardUsed.id = cardUsedId;
        }
        OnCardUse(user, cardUsed);

        gameUI.ShowMessagingCard(cardUsed);
        string colorStr = "";
        foreach (var color in messageCard.color)
        {
            colorStr = colorStr + LanguageUtils.GetColorName(color);
        }

        gameUI.AddMsg("情报" + colorStr + LanguageUtils.GetCardName(messageCard.cardName) + "被调包了");
    }

    //通知所有人使用破译，并询问是否翻开并摸一张牌（只有黑情报才能翻开）
    public void OnReceiveUsePoYi(int user, CardFS cardUsed, CardFS messageCard, int waitingTime, uint seq)
    {
        seqId = seq;
        OnCardUse(user, cardUsed);
        OnWait(user, waitingTime);
        if(user == SelfPlayerId)
        {
            gameUI.ShowPoYiResult(messageCard);
            string colorStr = "";
            foreach(var color in messageCard.color)
            {
                colorStr = colorStr + LanguageUtils.GetColorName(color);
            }
            gameUI.AddMsg(string.Format("破译结果为，{0} {1}", colorStr, LanguageUtils.GetCardName(messageCard.cardName)));
        }
    }
    public void OnReceivePoYiShow(int user, bool show, CardFS messageCard)
    {
        if(show)
        {
            string colorStr = "";
            foreach (var color in messageCard.color)
            {
                colorStr = colorStr + LanguageUtils.GetColorName(color);
            }
            gameUI.AddMsg(string.Format("情报被翻开，{0} {1}", colorStr, LanguageUtils.GetCardName(messageCard.cardName)));
        }
    }

    // 通知客户端，谁对谁使用了试探
    public void OnRecerveUseShiTan(int user, int targetUser, int cardId = 0)
    {
        CardFS card = null;
        string cardInfo = "";
        if (players.ContainsKey(user))
        {
            players[user].cardCount = players[user].cardCount - 1;
        }
        if (user == SelfPlayerId && cardsHand.ContainsKey(cardId))
        {
            card = cardsHand[cardId];
            cardsHand.Remove(cardId);
            cardInfo = "试探摸牌颜色";
            foreach (var color in card.shiTanColor)
            {
                cardInfo = cardInfo + LanguageUtils.GetIdentityName(color);
            }
        }
        //Debug.LogError("________________ OnRecerveUseShiTan," + cardId);
        gameUI.OnUseCard(user, targetUser, card);

        gameUI.AddMsg(string.Format("{0}号玩家对{1}号玩家使用了试探;{2}", user, targetUser, cardInfo));
    }
    // 向被试探者展示试探，并等待回应
    public void OnReceiveShowShiTan(int user, int targetUser, CardFS card, int waitingTime, uint seqId)
    {
        this.seqId = seqId;
        OnWait(targetUser, waitingTime);
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
        OnCardUse(user, cardUsed, target);

        gameUI.ShowTopCard(card);
        if (isJoinHand)
        {
            if (players.ContainsKey(user))
            {
                players[user].cardCount += 1;

                gameUI.AddMsg(string.Format("{0}号玩家将牌堆顶的{1}加入手牌", user, LanguageUtils.GetCardName(card.cardName)));
            }

            if (user == SelfPlayerId)
            {
                gameUI.DrawCards(new List<CardFS>() { card });
            }
        }
        else
        {
            if (players.ContainsKey(target))
            {
                players[target].AddMessage(card);
                gameUI.Players[target].RefreshMessage();
                gameUI.AddMsg(string.Format("{0}号玩家将牌堆顶的{1}收为情报", target, LanguageUtils.GetCardName(card.cardName)));
            }
        }
    }
    // 通知客户端使用平衡的结果 //弃牌部分走 OnReceiveDiscards
    public void OnReceiveUsePingHeng(int user, int target, CardFS cardUsed)
    {
        OnCardUse(user, cardUsed, target);
    }
    // 通知所有人威逼的牌没有，展示所有手牌
    public void OnReceiveUseWeiBiShowHands(int user, int target, CardFS cardUsed, List<CardFS> cards)
    {
        OnCardUse(user, cardUsed, target);
        if (user == SelfPlayerId)
        {
            string cardInfo = "";
            foreach (var card in cards)
            {
                cardsHand[card.id] = card;
                cardInfo += LanguageUtils.GetCardName(card.cardName) + ",";
            }
            gameUI.AddMsg(string.Format("{0}号玩家向你展示了手牌，{1}", target, cardInfo));
        }
        else
        {
            gameUI.AddMsg(string.Format("{0}号玩家向{1}号玩家展示了手牌", target, user));
        }
    }
    // 通知所有人威逼等待给牌
    public void OnReceiveUseWeiBiGiveCard(int user, int target, CardFS cardUsed, CardNameEnum cardWant, int waitTime, uint seq)
    {
        OnCardUse(user, cardUsed, target);

        this.seqId = seq;
        OnWait(target, waitTime);
        if (target == SelfPlayerId)
        {
            gameUI.ShowWeiBiGiveCard(cardWant, user, waitTime);
        }
        //Debug.LogError(cardUsed.cardName);
    }

    // 通知所有人威逼给牌
    public void OnReceiveExcuteWeiBiGiveCard(int user, int target, CardFS cardGiven)
    {
        int total = players[user].DrawCard(1);
        players[target].cardCount = players[target].cardCount - 1;

        if (gameUI.Players[user] != null)
        {
            gameUI.Players[user].OnDrawCard(total, 1);
        }

        if (gameUI.Players.ContainsKey(target))
        {
            gameUI.Players[target].Discard(new List<CardFS>() { cardGiven });
        }

        if (user == SelfPlayerId)
        {
            cardsHand[cardGiven.id] = cardGiven;
            gameUI.DrawCards(new List<CardFS>() { cardGiven });
        }
        if (SelfPlayerId == target)
        {
            cardsHand.Remove(cardGiven.id);
            gameUI.DisCards(new List<CardFS>() { cardGiven });
        }

        gameUI.AddMsg(string.Format("{0}号玩家给了{1}号玩家一张牌{2}", target, user, LanguageUtils.GetCardName(cardGiven.cardName)));
    }

    // 通知所有人澄清
    public void OnReceiveUseChengQing(int user, int target, CardFS cardUsed, int targetCardId)
    {
        OnCardUse(user, cardUsed, target);

        if (players.ContainsKey(target))
        {
            players[target].RemoveMessage(targetCardId);
            gameUI.Players[target].RefreshMessage();
        }
        gameUI.HidePlayerMessageInfo();
        gameUI.AddMsg(string.Format("{0}号玩家的情报被烧毁", target));

    }

    // 濒死求澄清
    public void OnReceiveWaitSaving(int playerId, int waitingPlayer, int waitingSecond)
    {
        gameUI.Players[CurWaitingPlayerId].OnWaiting(0);
        gameUI.Players[waitingPlayer].OnWaiting(waitingSecond);

        if (waitingPlayer == SelfPlayerId)
        {
            IsWaitSaving = playerId;
        }
        else
        {
            IsWaitSaving = -1;
        }
        gameUI.AddMsg(string.Format("{0}号玩家濒死向{1}号玩家请求澄清", playerId, waitingPlayer));

    }

    // 通知客户端谁死亡了
    public void OnReceivePlayerDied(int playerId, bool loseGame)
    {
        gameUI.Players[playerId].OnDie(loseGame);
        if(loseGame)
        {
            gameUI.AddMsg(string.Format("{0}号玩家游戏失败", playerId));
        }
        else
        {
            gameUI.AddMsg(string.Format("{0}号玩家阵亡", playerId));
        }
    }

    public void OnReceiveDieGiveingCard(int playerId, int waitingSecond, uint seq)
    {
        this.seqId = seq;
        OnWait(playerId, waitingSecond);
        if (playerId == SelfPlayerId)
        {
            SelectCardId = -1;
            IsWaitGiveCard = true;
            gameUI.ShowPhase();
        }
        gameUI.AddMsg(string.Format("等待{0}号玩家托付手牌", playerId));
    }

    // 通知谁获胜了
    public void OnReceiveWinner(int playerId, List<int> winners, List<PlayerColorEnum> playerColers, List<SecretTaskEnum> playerTasks)
    {
        gameUI.ShowWinInfo(playerId, winners, playerColers, playerTasks);
    }
    #endregion


    #region 向服务器发送请求
    public void SendEndWaiting()
    {
        ProtoHelper.SendEndWaiting(seqId);
    }
    public void SendEndFightPhase()
    {
        ProtoHelper.SendEndFight(seqId);
    }
    public void SendUseCard()
    {
        if (SelectCardId != -1 && cardsHand.ContainsKey(SelectCardId))
        {
            if (curPhase == PhaseEnum.Main_Phase)
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
                    //使用威逼, 只打开选择界面， 不发送请求
                    case CardNameEnum.Wei_Bi:
                        if (SelectPlayerId != -1 && SelectPlayerId != 0)
                        {
                            gameUI.ShowWeiBiSelect(true);
                            return;
                        }
                        else
                        {
                            Debug.LogError("请选择正确的威逼目标");
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
            else if (curPhase == PhaseEnum.Fight_Phase)
            {
                CardNameEnum card = cardsHand[SelectCardId].cardName;
                switch (card)
                {
                    case CardNameEnum.Diao_Bao:
                        ProtoHelper.SendUseDiaoBao(SelectCardId, seqId);
                        break;
                    case CardNameEnum.Jie_Huo:
                        ProtoHelper.SendUseCardMessage_JieHuo(SelectCardId, seqId);
                        break;

                }
            }

        }

        SelectCardId = -1;
    }

    public void SendUserPoYi()
    {
        ProtoHelper.SendUseCardMessage_PoYi(SelectCardId, seqId);
    }

    private int messageTarget = 0;
    public void SendMessage()
    {
        if (curPhase == PhaseEnum.Send_Start_Phase && CurWaitingPlayerId == SelfPlayerId)
        {
            if (!cardsHand[SelectCardId].canLock)
            {
                if (SelectPlayerId < 1)
                {
                    SelectCardId = -1;
                    SelectPlayerId = -1;
                    return;
                }

                ProtoHelper.SendMessageCard(SelectCardId, SelectPlayerId, new List<int>(), cardsHand[SelectCardId].direction, seqId);
                SelectCardId = -1;
            }
            else if (!IsWaitLock)
            {
                if(SelectPlayerId < 1)
                {
                    SelectCardId = -1;
                    SelectPlayerId = -1;
                    return;
                }

                IsWaitLock = true;
                messageTarget = SelectPlayerId;
                SelectPlayerId = -1;
                gameUI.ShowPhase();
            }
            else
            {
                IsWaitLock = false;
                int lockId = SelectPlayerId > 0 ? SelectPlayerId : 0;
                ProtoHelper.SendMessageCard(SelectCardId, messageTarget, new List<int>() { lockId }, cardsHand[SelectCardId].direction, seqId);
                SelectCardId = -1;
            }
        }
    }

    public void SendWhetherReceive(bool receive)
    {
        ProtoHelper.SendWhetherReceive(receive, seqId);
    }
    public void SendDoShiTan(int cardId)
    {
        ProtoHelper.SendDoShiTan(cardId, seqId);
        SelectCardId = -1;
    }

    public void SendDoWeiBi(int cardId)
    {
        Debug.LogError("威逼给牌" + cardId);
        ProtoHelper.SendDoWeiBi(cardId, seqId);
        SelectCardId = -1;
    }

    public void SendWhetherSave(bool save, int cardId = 0)
    {
        ProtoHelper.SendChengQingSaveDying(save, SelectCardId, cardId, seqId);
    }

    public void SendDieGiveCards()
    {
        if (SelectPlayerId != -1 && SelectPlayerId != 0 && cardsToGive.Count > 0)
        {
            ProtoHelper.SendDieGiveCard(seqId, cardsToGive, SelectPlayerId);
        }
        else
        {
            Debug.LogError("请选择手牌和目标角色");
        }
    }

    public void SendPoYiShow(bool show)
    {
        ProtoHelper.SendPoYiShow(show, seqId);
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
    Send_Start_Phase = 2, //情报传递阶段开始时
    Send_Phase = 3,   // 传递阶段
    Fight_Phase = 4,   // 争夺阶段
    Receive_Phase = 5, // 接收阶段
}
