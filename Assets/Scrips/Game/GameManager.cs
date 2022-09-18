using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public static int SelfPlayerId = 0;
    public GameUI gameUI;
    public RoomUI roomUI;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public Dictionary<int, CardFS> cardsHand = new Dictionary<int, CardFS>(); //<id, card>

    public PhaseEnum curPhase { get; private set; }
    public SecretTaskEnum task { get; private set; }

    public int CurTurnPlayerId { get; private set; }
    public int CurMessagePlayerId { get; private set; }
    public int CurWaitingPlayerId { get; private set; }


    #region 特殊状态 有人求澄清 传情报时选择锁定目标 死亡给牌
    public int IsWaitSaving { get; private set; }
    public bool IsWaitLock { get; private set; }
    public bool IsWaitGiveCard { get; private set; }
    public bool IsUsingSkill { get; set; }
    public bool IsBingShiTan { get; private set; }

    public List<int> cardsToGive = new List<int>();
    #endregion

    public List<int> lockedPlayer = null;

    public SkillBase selectSkill;

    public CardFS messageReceived;
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
            if (IsUsingSkill)
            {
                selectSkill.OnCardSelect(value);
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

            gameUI.CheckTargetAvailable();
        }
    }

    internal void OnNetWorkErr()
    {
        gameUI.ShowInfo("连接错误");
        Reset();
        SceneManager.LoadScene("TheMessage");
    }

    internal void Reset()
    {
        SelectCardId = -1;
        SelectPlayerId = -1;
        CurTurnPlayerId = 0;
        CurMessagePlayerId = 0;
        CurWaitingPlayerId = 0;
        IsBingShiTan = false;
        IsUsingSkill = false;
        IsWaitGiveCard = false;
        IsWaitLock = false;
        IsWaitSaving = -1;
    }

    private int _SelectCardId = -1;

    public int SelectPlayerId
    {
        get { return _SelectPlayerId; }
        set
        {
            if (IsUsingSkill)
            {
                selectSkill.OnPlayerSelect(value);
                return;
            }

            gameUI.HidePlayerMessageInfo();
            if (gameUI.Players.ContainsKey(_SelectPlayerId)) gameUI.Players[_SelectPlayerId].OnSelect(false);
            _SelectPlayerId = -1;
            if (_SelectPlayerId == value)
            {
                _SelectPlayerId = -1;
                return;
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
                    gameUI.ShowInfo("不能选自己作为目标");
                }
                else if (gameUI.Players.ContainsKey(value))
                {
                    _SelectPlayerId = value;
                    gameUI.Players[_SelectPlayerId].OnSelect(true);
                }
            }
            // 判断出牌时选中玩家
            else if (curPhase == PhaseEnum.Main_Phase && cardsHand.ContainsKey(_SelectCardId))
            {
                switch (cardsHand[_SelectCardId].cardName)
                {
                    case CardNameEnum.WeiBi:
                    case CardNameEnum.PingHeng:
                    case CardNameEnum.ShiTan:
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
                    case CardNameEnum.LiYou:
                        if (gameUI.Players.ContainsKey(value))
                        {
                            _SelectPlayerId = value;
                            gameUI.Players[_SelectPlayerId].OnSelect(true);
                        }
                        break;
                    case CardNameEnum.ChengQing:
                        if (gameUI.Players.ContainsKey(value) && players[value].GetMessageCount(CardColorEnum.Black) > 0)
                        {
                            _SelectPlayerId = value;
                            gameUI.Players[_SelectPlayerId].OnSelect(true);
                        }
                        gameUI.ShowPlayerMessageInfo(_SelectPlayerId, true);
                        break;
                }
            }
            // 情报争夺阶段选择误导目标
            else if (curPhase == PhaseEnum.Fight_Phase && cardsHand.ContainsKey(_SelectCardId))
            {
                if (cardsHand[_SelectCardId].cardName == CardNameEnum.WuDao)
                {
                    if (value == GetPlayerAliveLeft(CurMessagePlayerId) || value == GetPlayerAliveRight(CurMessagePlayerId))
                    {
                        _SelectPlayerId = value;
                        gameUI.Players[_SelectPlayerId].OnSelect(true);
                    }
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
        GameObject roomGo = GameObject.Find("RoomUI");

        if (gameUI == null && windowGo != null)
        {
            gameUI = windowGo.GetComponent<GameUI>();
            if (gameUI == null)
            {
                gameUI = windowGo.AddComponent<GameUI>();
            }
        }

        if (roomUI == null && roomGo != null)
        {
            roomUI = roomGo.GetComponent<RoomUI>();
        }
        gameUI.gameObject.SetActive(false);
        roomUI.gameObject.SetActive(true);
    }

    private void InitDatas()
    {

    }
    public void InitPlayers(int num, List<RoleBase> roles, List<string> names)
    {
        players.Clear();
        for (int i = 0; i < num; i++)
        {
            Player player = new Player(i, roles[i], names[i]);
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
    public int GetPlayerAliveLeft(int PlayerId)
    {
        int id = (PlayerId + players.Count - 1) % players.Count;
        while (!players[id].alive)
        {
            id = (id + players.Count - 1) % players.Count;
            if (id == PlayerId)
            {
                return id;
            }
        }
        return id;
    }

    public int GetPlayerAliveRight(int PlayerId)
    {
        int id = (PlayerId + 1) % players.Count;
        while (!players[id].alive)
        {
            id = (id + 1) % players.Count;
            if (id == PlayerId)
            {
                return id;
            }
        }
        return id;
    }
    private void OnCardUse(int user, CardFS cardUsed, int target = -1)
    {
        if(cardUsed != null)
        {
            if (players.ContainsKey(user))
            {
                players[user].DisCard(1);
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
            targetInfo = target == -1 ? "" : "对" + GameManager.Singleton.players[target].name;
            gameUI.AddMsg(string.Format("{0}{1}使用了{2};", GameManager.Singleton.players[user].name, targetInfo, LanguageUtils.GetCardName(cardUsed.cardName)));
            SoundManager.PlaySound(cardUsed.cardName, players[user].role.isWoman);
        }
    }

    private void OnCardSend(int playerId, int cardId, int targetId, List<int> lockIds, DirectionEnum dir)
    {
        int user = playerId;
        if (players.ContainsKey(user))
        {
            players[user].DisCard(1);
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

        gameUI.AddMsg(string.Format("{0}向{1}传出一张情报，方向向{2}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name, dirStr));
        foreach (var id in lockIds)
        {
            gameUI.AddMsg("" + GameManager.Singleton.players[id].name + "被锁定了");
        }
    }
    private void OnWait(int playerId, int waitSeconds)
    {
        if (CurWaitingPlayerId == SelfPlayerId)
        {
            gameUI.OnWaiting(0);
        }
        else if (gameUI.Players.ContainsKey(CurWaitingPlayerId))
        {
            gameUI.Players[CurWaitingPlayerId].OnWaiting(0);
        }
        if (playerId == SelfPlayerId)
        {
            gameUI.OnWaiting(waitSeconds);
        }
        else if (gameUI.Players.ContainsKey(playerId))
        {
            gameUI.Players[playerId].OnWaiting(waitSeconds);
        }
        CurWaitingPlayerId = playerId;
        gameUI.RefreshIsCanCancel();

    }
    #region 服务器消息处理
    public void OnReceiveRoomInfo(List<string> names, int myPosition)
    {
        roomUI.OnRoomInfo(names, myPosition);
    }
    public void OnAddPlayer(string name, int index)
    {
        roomUI.OnAddPlayer(name, index);
    }
    public void OnPlayerLeave(int position)
    {
        roomUI.OnPlayerLeave(position);
    }

    public void OnTaskChange(SecretTaskEnum secretTask)
    {
        task = secretTask;
        gameUI.SetTask(task);
    }
    // 等待客户端选角色
    public void OnReceiveChooseRole(int playerCount, PlayerColorEnum playerColor, SecretTaskEnum secretTask, List<role> roles)
    {
        gameUI.gameObject.SetActive(true);
        roomUI.gameObject.SetActive(false);

        gameUI.InitPlayers(playerCount);
        gameUI.ShowChooseRole(playerColor, secretTask, roles);
    }
    public  void OnReceiveChooseRoleSuccess(role role)
    {
        gameUI.roleSelect.OnRoleSelect(role);
    }

    // 通知客户端：初始化游戏
    public void OnReceiveGameStart(int player_num, PlayerColorEnum playerColor, SecretTaskEnum secretTask, List<RoleBase> roles, List<string> names)
    {
        gameUI.gameObject.SetActive(true);
        roomUI.gameObject.SetActive(false); 
        gameUI.HideChooseRole();

        task = secretTask;

        InitPlayers(player_num, roles, names);
        players[SelfPlayerId].playerColor = new List<PlayerColorEnum>() { playerColor };
        gameUI.InitPlayers(player_num, true);

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
            players[playerId].DisCard(cards.Count);
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
            }
        }
        foreach (var card in cards)
        {
            cardInfo += LanguageUtils.GetCardName(card.cardName) + ",";
        }
        gameUI.DisCard(cards, playerId);

        gameUI.AddMsg(string.Format("{0}弃了{1}张牌; {2}", GameManager.Singleton.players[playerId].name, cards.Count, cardInfo));

    }
    //其他角色摸牌
    public void OnReceiveOtherDrawCards(int id, int num)
    {
        int total = players[id].DrawCard(num);
        if (gameUI.Players[id] != null)
        {
            gameUI.Players[id].OnDrawCard(total, num);
        }
        gameUI.AddMsg(string.Format("{0}摸了{1}张牌", GameManager.Singleton.players[id].name, num));
    }

    public void OnReceivePlayerUpDate(int playerId, role role)
    {
        players[playerId].UpdateRole(role);
        gameUI.Players[playerId].OnTurnBack(players[playerId].role.isBack);
    }

    // 通知客户端，到谁的哪个阶段了
    public void OnReceiveTurn(int playerId, int messagePlayerId, int waitingPlayerId, PhaseEnum phase, int waitSecond, DirectionEnum messageCardDir, CardFS message, uint seqId)
    {
        //Debug.Log(" playerId " + playerId + " messagePlayerId " + messagePlayerId + " waitingPlayerId " + waitingPlayerId + " phase " + phase.ToString());
        if(IsUsingSkill || selectSkill != null)
        {
            IsUsingSkill = false;
            selectSkill.Cancel();
        }
        int lastTurnPlayerId = CurTurnPlayerId;
        IsWaitGiveCard = false;
        IsWaitLock = false;
        IsWaitSaving = -1;
        messageReceived = null;
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

        gameUI.Players[SelfPlayerId].RefreshSkillState();
        gameUI.HideMessagingCard();
        gameUI.weiBiGiveCard.gameObject.SetActive(false);
        gameUI.Players[lastTurnPlayerId].HidePhase();
        gameUI.Players[playerId].SetPhase(phase);
        if (playerId != lastTurnPlayerId)
        {
            lockedPlayer = null;
            gameUI.ClearLock();
            gameUI.AddMsg(string.Format("{0}回合开始", GameManager.Singleton.players[playerId].name));
            if(lastTurnPlayerId == SelfPlayerId)
            {
                foreach (var skill in players[SelfPlayerId].role.skills)
                {
                    skill.OnTurnEnd();
                }
            }
        }
        if (phase == PhaseEnum.Send_Start_Phase)
        {
            gameUI.InitMessageSenderPos(playerId);
            gameUI.AddMsg(string.Format("{0}开始传递情报", GameManager.Singleton.players[playerId].name));
        }
        else if (phase == PhaseEnum.Send_Phase)
        {
            gameUI.ShowMessagingCard(message, messagePlayerId, true);
            if (messagePlayerId == SelfPlayerId && playerId != SelfPlayerId)
            {
                gameUI.ShowNextMessagePlayer(messageCardDir);
            }
            if (cardsHand.ContainsKey(message.id))
            {
                cardsHand.Remove(message.id);
                gameUI.RemoveCards(new List<CardFS>() { message });
            }
            //string dir;

            gameUI.AddMsg(string.Format("情报到达{0}，方向{1}", GameManager.Singleton.players[messagePlayerId].name, messageCardDir.ToString()));
        }
        else if (phase == PhaseEnum.Fight_Phase)
        {
            gameUI.ShowMessagingCard(message, messagePlayerId, true);
        }
        else if (phase == PhaseEnum.Receive_Phase)
        {
            if (waitSecond == 0)
            {
                players[messagePlayerId].AddMessage(message);
                gameUI.ShowAddMessage(messagePlayerId, message, true);
                gameUI.Players[messagePlayerId].RefreshMessage();
                gameUI.AddMsg(string.Format("{0}接收情报", GameManager.Singleton.players[messagePlayerId].name));
            }
            else if (waitingPlayerId == SelfPlayerId)
            {
                messageReceived = message;
                foreach (var skill in players[SelfPlayerId].role.skills)
                {
                    if (skill.CheckTriger()) break;
                }
            }
        }
    }

    public void OnReceiveUseWuDao(int user, int target, CardFS cardUsed)
    {
        OnCardUse(user, cardUsed, target);
    }

    public void OnReceiveUseJieHuo(int user, CardFS cardUsed)
    {
        OnCardUse(user, cardUsed);
    }

    // 通知所有人传情报
    public void OnReceiveMessageSend(int playerId, int cardId, int targetId, List<int> locakIds, DirectionEnum dir)
    {
        OnCardSend(playerId, cardId, targetId, locakIds, dir);
        string sLock = "";
        string sDir = "";
        if (locakIds.Count > 0)
        {
            sLock = " 并锁定玩家";
            foreach (var id in locakIds)
            {
                sLock += " " + id;
            }
        }
        switch (dir)
        {
            case DirectionEnum.Left:
                sDir = "向左";
                break;
            case DirectionEnum.Right:
                sDir = "向右";
                break;
            case DirectionEnum.Up:
                sDir = "向上";
                break;
        }
        string s = string.Format("{0}向{1}传出情报,方向{2}{3}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name, sDir, sLock);
        gameUI.ShowInfo(s);
        gameUI.AddMsg(s);
    }

    // 通知所有人选择要接收情报（只有选择要收时有这条协议）
    public void OnReceiveMessageAccept(int playerId)
    {
        gameUI.OnMessageAccept(playerId);
        string s = "" + GameManager.Singleton.players[playerId].name + "选择了接收情报，正在询问所有人是否响应";
        gameUI.ShowInfo(s);
        gameUI.AddMsg(s);
    }

    // 通知所有人使用调包
    public void OnReceiveUseDiaoBao(int user, int cardUsedId, CardFS messageCard)
    {
        CardFS cardUsed;
        if (user == SelfPlayerId)
        {
            cardUsed = cardsHand[cardUsedId];
        }
        else
        {
            cardUsed = new CardFS(null);
            cardUsed.cardName = CardNameEnum.DiaoBao;
            cardUsed.id = cardUsedId;
        }
        OnCardUse(user, cardUsed);

        gameUI.ShowMessagingDiaoBao(messageCard);
        string colorStr = "";
        foreach (var color in messageCard.color)
        {
            colorStr = colorStr + LanguageUtils.GetColorName(color);
        }

        gameUI.ShowInfo("情报" + colorStr + LanguageUtils.GetCardName(messageCard.cardName) + "被调包了");
        gameUI.AddMsg("情报" + colorStr + LanguageUtils.GetCardName(messageCard.cardName) + "被调包了");
    }

    public void OnErrorCode(error_code code)
    {
        //TODO 提示层级需要优化
        switch (code)
        {
            case error_code.ClientVersionNotMatch:
                gameUI.ShowInfo("客户端版本号不匹配");
                break;
            case error_code.NoMoreRoom:
                gameUI.ShowInfo("没有更多的房间了");
                break;
            default:
                gameUI.ShowInfo("ERROR " + (int)code + "  " + code.ToString());
                break;
        }
    }

    //通知所有人使用破译，并询问是否翻开并摸一张牌（只有黑情报才能翻开）
    public void OnReceiveUsePoYi(int user, CardFS cardUsed, CardFS messageCard, int waitingTime, uint seq)
    {
        seqId = seq;
        OnCardUse(user, cardUsed);
        OnWait(user, waitingTime);
        if (user == SelfPlayerId)
        {
            gameUI.ShowPoYiResult(messageCard);
            string colorStr = "";
            foreach (var color in messageCard.color)
            {
                colorStr = colorStr + LanguageUtils.GetColorName(color);
            }
            gameUI.AddMsg(string.Format("破译结果为，{0} {1}", colorStr, LanguageUtils.GetCardName(messageCard.cardName)));
        }
    }
    public void OnReceivePoYiShow(int user, bool show, CardFS messageCard)
    {
        if (show)
        {
            string colorStr = "";
            foreach (var color in messageCard.color)
            {
                colorStr = colorStr + LanguageUtils.GetColorName(color);
            }
            gameUI.AddMsg(string.Format("情报被翻开，{0} {1}", colorStr, LanguageUtils.GetCardName(messageCard.cardName)));
        }
        gameUI.poYiResult.gameObject.SetActive(false);
    }

    private List<PlayerColorEnum> shiTanColor;
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
            shiTanColor = card.shiTanColor;
            cardInfo = "试探摸牌颜色";
            foreach (var color in card.shiTanColor)
            {
                cardInfo = cardInfo + LanguageUtils.GetIdentityName(color);
            }
        }
        else
        {
            card = new CardFS(null);
            card.cardName = CardNameEnum.ShiTan;
        }
        //Debug.LogError("________________ OnRecerveUseShiTan," + cardId);
        gameUI.OnUseCard(user, targetUser, card);

        string s = string.Format("{0}对{1}使用了试探;{2}", GameManager.Singleton.players[user].name, GameManager.Singleton.players[targetUser].name, cardInfo);
        gameUI.AddMsg(s);
        gameUI.ShowInfo(s);
        SoundManager.PlaySound(CardNameEnum.ShiTan, players[user].role.isWoman);
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
            string cardInfo = "试探摸牌颜色";
            foreach (var color in card.shiTanColor)
            {
                cardInfo = cardInfo + LanguageUtils.GetIdentityName(color);
            }
            gameUI.AddMsg("你被试探了;" + cardInfo);
        }
    }

    // 被试探者执行试探
    public void OnReceiveExcuteShiTan(int playerId, bool isDrawCard)
    {
        if (playerId == SelfPlayerId)
        {
            gameUI.HideShiTanInfo();
        }

        if (shiTanColor != null)
        {
            List<PlayerColorEnum> colors = new List<PlayerColorEnum>();
            foreach (var color in players[playerId].playerColor)
            {
                if (isDrawCard == shiTanColor.Contains(color))
                {
                    colors.Add(color);
                }
            }
            players[playerId].playerColor = colors;

            gameUI.Players[playerId].playerColor.SetColor(players[playerId].playerColor);

            shiTanColor = null;
        }

        string s = isDrawCard ? "" + GameManager.Singleton.players[playerId].name + "被试探摸了一张牌" : "" + GameManager.Singleton.players[playerId].name + "被试探弃了一张牌";
        gameUI.ShowInfo(s);
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

                gameUI.AddMsg(string.Format("{0}将牌堆顶的{1}加入手牌", GameManager.Singleton.players[user].name, LanguageUtils.GetCardName(card.cardName)));
                gameManager.gameUI.Players[user].RefreshCardCount();
            }

            if (user == SelfPlayerId)
            {
                cardsHand[card.id] = (card);
                gameUI.DrawCards(new List<CardFS>() { card });
            }
            else
            {
                gameUI.ShowAddMessage(user, card, false);
            }
        }
        else
        {
            if (players.ContainsKey(target))
            {
                players[target].AddMessage(card);
                gameUI.ShowAddMessage(target, card, false);
                gameUI.Players[target].RefreshMessage();
                gameUI.AddMsg(string.Format("{0}将牌堆顶的{1}收为情报", GameManager.Singleton.players[target].name, LanguageUtils.GetCardName(card.cardName)));
            }
        }
    }
    // 通知客户端使用平衡的结果 //弃牌部分走 OnReceiveDiscards
    public void OnReceiveUsePingHeng(int user, int target, CardFS cardUsed)
    {
        OnCardUse(user, cardUsed, target);
    }
    // 通知所有人威逼的牌没有，展示所有手牌
    public void OnReceiveUseWeiBiShowHands(int user, int target, CardFS cardUsed, List<CardFS> cards, CardNameEnum cardWant)
    {
        OnCardUse(user, cardUsed, target);

        gameUI.AddMsg(string.Format("{0}号向{1}号索求一张{2}", user, target, LanguageUtils.GetCardName(cardWant)));

        if (user == SelfPlayerId)
        {
            string cardInfo = "";
            foreach (var card in cards)
            {
                cardsHand[card.id] = card;
                cardInfo += LanguageUtils.GetCardName(card.cardName) + ",";
            }
            gameUI.ShowHandCard(target, cards);
            gameUI.AddMsg(string.Format("{0}向你展示了手牌，{1}", GameManager.Singleton.players[target].name, cardInfo));
        }
        else
        {
            gameUI.AddMsg(string.Format("{0}没有{1}，向{2}展示了手牌", GameManager.Singleton.players[target].name, LanguageUtils.GetCardName(cardWant), GameManager.Singleton.players[user].name));
        }
        gameUI.ShowInfo(string.Format("{0}没有{1}，向{2}展示了手牌", GameManager.Singleton.players[target].name, LanguageUtils.GetCardName(cardWant), GameManager.Singleton.players[user].name));
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
        string s = string.Format("{0}号向{1}号索求一张{2}", user, target, LanguageUtils.GetCardName(cardWant));
        gameUI.AddMsg(s);
        gameUI.ShowInfo(s);
        //Debug.LogError(cardUsed.cardName);
    }

    // 通知所有人威逼给牌
    public void OnReceiveExcuteWeiBiGiveCard(int user, int target, CardFS cardGiven)
    {
        int total = players[user].DrawCard(1);
        players[target].cardCount = players[target].cardCount - 1;

        if (gameUI.Players[user] != null)
        {
            gameUI.Players[user].RefreshCardCount();
        }

        if (gameUI.Players.ContainsKey(target))
        {
            gameUI.Players[target].RefreshCardCount();
        }

        if (user == SelfPlayerId)
        {
            cardsHand[cardGiven.id] = cardGiven;
            gameUI.DrawCards(new List<CardFS>() { cardGiven });
        }
        if (SelfPlayerId == target)
        {
            cardsHand.Remove(cardGiven.id);
        }
        gameUI.GiveCards(target, user, new List<CardFS>() { cardGiven });

        gameUI.AddMsg(string.Format("{0}给了{1}一张牌", GameManager.Singleton.players[target].name, GameManager.Singleton.players[user].name));
    }

    // 通知所有人澄清
    public void OnReceiveUseChengQing(int user, int target, CardFS cardUsed, int targetCardId)
    {
        OnCardUse(user, cardUsed, target);

        string messageInfo = "";
        if (players.ContainsKey(target))
        {
            CardFS message = new CardFS(null);
            foreach (var card in players[target].messages)
            {
                if (card.id == targetCardId)
                {
                    messageInfo = LanguageUtils.GetColorsName(card.color) + LanguageUtils.GetCardName(card.cardName);
                    message = card;
                    break;
                }
            }
            players[target].RemoveMessage(targetCardId);
            gameUI.Players[target].RefreshMessage();
            gameUI.OnPlayerMessageRemove(target, new List<CardFS>() { message });
        }
        gameUI.HidePlayerMessageInfo();
        gameUI.AddMsg(string.Format("{0}的情报{1}被烧毁", GameManager.Singleton.players[target].name, messageInfo));

    }


    // 濒死求澄清
    public void OnReceiveWaitSaving(int playerId, int waitingPlayer, int waitingSecond, uint seq)
    {
        seqId = seq;
        OnWait(waitingPlayer, waitingSecond);

        if (waitingPlayer == SelfPlayerId)
        {
            IsWaitSaving = playerId;
            gameUI.Players[SelfPlayerId].RefreshSkillState();
        }
        else
        {
            IsWaitSaving = -1;
        }
        gameUI.AddMsg(string.Format("{0}濒死向{1}请求澄清", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[waitingPlayer].name));
        gameUI.ShowPhase();
    }
    // 通知客户端谁死亡了（通知客户端将其置灰，之后不能再成为目标了）
    public void OnReceivePlayerDying(int playerId, bool loseGame)
    {
        IsWaitSaving = -1;
        players[playerId].alive = false;
        gameUI.Players[playerId].OnDie(loseGame);

        if (loseGame)
        {
            gameUI.AddMsg(string.Format("{0}游戏失败", GameManager.Singleton.players[playerId].name));
        }
        else
        {
            gameUI.AddMsg(string.Format("{0}阵亡", GameManager.Singleton.players[playerId].name));
        }
    }
    // 通知客户端谁死亡了（通知客户端弃掉所有情报）
    public void OnReceivePlayerDied(int playerId)
    {
        IsWaitSaving = -1;
        List<CardFS> messages = new List<CardFS>();
        string cardsStr = "";
        foreach (var message in players[playerId].messages)
        {
            messages.Add(message);
        }
        players[playerId].messages.Clear();
        gameUI.OnPlayerMessageRemove(playerId, messages);
    }

    public void OnReceiveDieGiveingCard(int playerId, int waitingSecond, uint seq)
    {
        this.seqId = seq;
        if (playerId == SelfPlayerId)
        {
            SelectCardId = -1;
            IsWaitGiveCard = true;
        }
        else
        {
            gameUI.ShowInfo("" + playerId + "死亡，可以选择交给一名玩家至多三张牌");
        }
        IsWaitSaving = -1;
        gameUI.ShowPhase();
        OnWait(playerId, waitingSecond);
        gameUI.AddMsg(string.Format("等待{0}托付手牌", GameManager.Singleton.players[playerId].name));
    }
    public void OnReceiveDieGivenCard(int playerId, int targetPlayerId, int cardCount, List<CardFS> cards)
    {
        int count = SelfPlayerId == playerId || SelfPlayerId == targetPlayerId ? cards.Count : cardCount;
        int total = players[targetPlayerId].DrawCard(count);
        string cardsInfo = "";
        players[playerId].cardCount = players[playerId].cardCount - count;

        if (gameUI.Players[targetPlayerId] != null)
        {
            gameUI.Players[targetPlayerId].RefreshCardCount();
        }

        if (gameUI.Players.ContainsKey(playerId))
        {
            gameUI.Players[playerId].RefreshCardCount();
        }

        if (targetPlayerId == SelfPlayerId)
        {
            foreach (var cardGiven in cards)
            {
                cardsHand[cardGiven.id] = cardGiven;
                cardsInfo += LanguageUtils.GetCardName(cardGiven.cardName) + " ";
            }
            gameUI.DrawCards(cards);
        }
        if (SelfPlayerId == playerId)
        {
            foreach (var cardGiven in cards)
            {
                cardsHand.Remove(cardGiven.id);
                cardsInfo += LanguageUtils.GetCardName(cardGiven.cardName) + " ";
            }
        }
        gameUI.GiveCards(playerId, targetPlayerId, cards);

        gameUI.AddMsg(string.Format("{0}给了{1}{2}张牌 {3}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetPlayerId].name, count, cardsInfo));

    }

    // 通知谁获胜了
    public void OnReceiveWinner(List<int> playerId, List<int> winners, List<PlayerColorEnum> playerColers, List<SecretTaskEnum> playerTasks)
    {
        gameUI.Players[CurWaitingPlayerId].OnWaiting(0);
        gameUI.ShowWinInfo(playerId, winners, playerColers, playerTasks);
    }
    #endregion

    #region 通知询问是否使用技能
    public void OnReceiveWaitSkillRuGui(int playerId, int waitSeconds, uint seq)
    {
        seqId = seq;
        OnWait(playerId, waitSeconds);
        if (playerId == SelfPlayerId)
        {
            foreach (var skill in players[SelfPlayerId].role.skills)
            {
                if (skill is UserSkill_RuGui)
                {
                    var ruGui = skill as UserSkill_RuGui;
                    ruGui.PrepareUse();
                }
            }
        }
    }
    public void OnReceiveWaitSkillChengZhi(int userId, int diePlayerId, List<CardFS> cards, PlayerColorEnum playerColor, SecretTaskEnum secretTask, int waitingSeconds, uint seq)
    {
        seqId = seq;
        OnWait(userId, waitingSeconds);
        players[userId].cardCount += players[diePlayerId].cardCount;
        players[diePlayerId].cardCount = 0;
        gameUI.Players[userId].RefreshCardCount();
        gameUI.Players[diePlayerId].RefreshCardCount();
        if (userId == SelfPlayerId)
        {
            foreach (var card in cards)
            {
                cardsHand[card.id] = card;
            }
            gameUI.DrawCards(cards);

            foreach (var skill in players[SelfPlayerId].role.skills)
            {
                if (skill is UserSkill_ChengZhi)
                {
                    UserSkill_ChengZhi.dieColor = playerColor;
                    UserSkill_ChengZhi.dieTask = secretTask;
                    skill.PrepareUse();
                }
            }
        }

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
                    case CardNameEnum.ShiTan:
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
                    case CardNameEnum.WeiBi:
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
                    case CardNameEnum.LiYou:
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
                    case CardNameEnum.PingHeng:
                        if (SelectPlayerId != -1)
                        {
                            ProtoHelper.SendUseCardMessage_PingHeng(SelectCardId, SelectPlayerId, this.seqId);
                        }
                        else
                        {
                            Debug.LogError("请选择正确的平衡目标");
                        }
                        break;
                    case CardNameEnum.ChengQing:
                        gameUI.playerMessagInfo.OnClickChengQing();
                        break;
                }
            }
            else if (curPhase == PhaseEnum.Fight_Phase)
            {
                CardNameEnum card = cardsHand[SelectCardId].cardName;
                switch (card)
                {
                    case CardNameEnum.DiaoBao:
                        ProtoHelper.SendUseDiaoBao(SelectCardId, seqId);
                        break;
                    case CardNameEnum.JieHuo:
                        ProtoHelper.SendUseCardMessage_JieHuo(SelectCardId, seqId);
                        break;
                    case CardNameEnum.WuDao:
                        if (SelectPlayerId == GetPlayerAliveRight(CurMessagePlayerId) || SelectPlayerId == GetPlayerAliveLeft(CurMessagePlayerId))
                        {
                            ProtoHelper.SendUseCardMessage_WuDao(SelectCardId, SelectPlayerId, seqId);
                        }
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

                DirectionEnum direction = cardsHand[SelectCardId].direction;
                if(players[SelfPlayerId].role is Role_LaoBie)
                {
                    var role = players[SelfPlayerId].role as Role_LaoBie;
                    direction = role.direction;
                }
                ProtoHelper.SendMessageCard(SelectCardId, SelectPlayerId, new List<int>(), direction, seqId);
                SelectCardId = -1;
            }
            else if (!IsWaitLock)
            {
                if (SelectPlayerId < 1)
                {
                    SelectCardId = -1;
                    SelectPlayerId = -1;
                    return;
                }

                IsWaitLock = true;
                messageTarget = SelectPlayerId;
                SelectPlayerId = -1;
                gameUI.ShowPhase();
                gameUI.CheckTargetAvailable();
            }
            else
            {
                IsWaitLock = false;
                int lockId = SelectPlayerId > 0 ? SelectPlayerId : 0;
                DirectionEnum direction = cardsHand[SelectCardId].direction;
                if (players[SelfPlayerId].role is Role_LaoBie)
                {
                    var role = players[SelfPlayerId].role as Role_LaoBie;
                    direction = role.direction;
                }
                ProtoHelper.SendMessageCard(SelectCardId, messageTarget, new List<int>() { lockId }, direction, seqId);
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

    public void OnServerConnect()
    {
        ProtoHelper.SendAddRoom();
    }
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
