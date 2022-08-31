using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Text textPhase;
    public GameObject goTask;
    public Text textTask;
    public PoYiResult poYiResult;
    public ShiTanInfo shiTanInfo;
    public GameObject goWeiBiSelect;
    public WeiBiGiveCard weiBiGiveCard;
    public PlayerMessagInfo playerMessagInfo;
    public WinInfo winInfo;
    public UIPlayer itemPlayerUI;
    public UICard itemCardUI;
    public UICard messageCard;
    public Text textInfo;
    public Text textMidInfo;
    public Text textDeckCount;

    public GameObject goDirect;
    public Transform transDir;
    public RectTransform transCards;
    public Transform transPlayerSelf;
    public GridLayoutGroup gridCards;
    public GridLayoutGroup topPlayerGrid;
    public GridLayoutGroup leftPlayerGrid;
    public GridLayoutGroup rightPlayerGrid;
    public Button butCancel;
    public Button butSure;
    public Transform selfMessagePos;
    public Transform transCardsUsed;
    public Transform transCardsDised;
    public Slider slider;

    public Dictionary<int, UICard> Cards = new Dictionary<int, UICard>();
    public Dictionary<int, UIPlayer> Players = new Dictionary<int, UIPlayer>();

    private int leftNum;
    private int topNum;

    private void Awake()
    {
        goDirect.SetActive(false);
        slider.gameObject.SetActive(false);
        itemCardUI.gameObject.SetActive(false);
        itemPlayerUI.gameObject.SetActive(false);
        messageCard.gameObject.SetActive(false);
        winInfo.gameObject.SetActive(false);
        goTask.gameObject.SetActive(false);
        poYiResult.gameObject.SetActive(false);
        shiTanInfo.gameObject.SetActive(false);
        goWeiBiSelect.gameObject.SetActive(false);
        weiBiGiveCard.gameObject.SetActive(false);
        playerMessagInfo.gameObject.SetActive(false);
    }
    public void InitPlayers(int num)
    {
        if (Players.Count > 0)
        {
            foreach (var playerUI in Players)
            {
                GameObject.Destroy(playerUI.Value.gameObject);
            }
            Players.Clear();
        }

        leftNum = (num - 1) / 3;
        topNum = num - 1 - 2 * leftNum;

        var self = GameObject.Instantiate(itemPlayerUI, transPlayerSelf);
        self.Init(0);
        Players[0] = self;
        for (int i = 1; i < leftNum + 1; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, rightPlayerGrid.transform);
            player.Init(i);
            Players[i] = player;
        }
        for (int i = 1 + leftNum; i < 1 + leftNum + topNum; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, topPlayerGrid.transform);
            player.Init(i);
            Players[i] = player;
        }
        for (int i = 1 + leftNum + topNum; i < 1 + leftNum + topNum + leftNum; i++)
        {
            var player = GameObject.Instantiate(itemPlayerUI, leftPlayerGrid.transform);
            player.Init(i);
            Players[i] = player;
        }
        leftPlayerGrid.spacing = new Vector2(0, (leftPlayerGrid.GetComponent<RectTransform>().sizeDelta.y - leftPlayerGrid.cellSize.y * leftNum) / leftNum);
        rightPlayerGrid.spacing = new Vector2(0, (rightPlayerGrid.GetComponent<RectTransform>().sizeDelta.y - rightPlayerGrid.cellSize.y * leftNum) / leftNum);
        topPlayerGrid.spacing = new Vector2((topPlayerGrid.GetComponent<RectTransform>().sizeDelta.x - rightPlayerGrid.cellSize.x * topNum) / topNum, 0);

    }

    private Vector3 GetMessagePos(int playerId)
    {
        if(playerId == 0)
        {
            return selfMessagePos.position;
        }
        else if(playerId > 0 && playerId < leftNum + 1)
        {
            return Players[playerId].transform.position + new Vector3(-100, 0);
        }
        else if (playerId > leftNum && playerId < 1 + leftNum + topNum)
        {
            return Players[playerId].transform.position + new Vector3(0, -150);
        }
        else if (playerId > leftNum + topNum && playerId < 1 + leftNum + topNum + leftNum)
        {
            return Players[playerId].transform.position + new Vector3(100, 0);
        }
        return Vector3.zero;
    }

    public void InitCards(int count)
    {
        if (count <= 4)
        {
            this.ClearCards();
            for (int i = count; i > 0; i--)
            {
                UICard card = GameObject.Instantiate(itemCardUI, transCards);
                card.Init(i);
                Cards[i] = (card);
            }
        }
        else
        {
        }
        CardsSizeFitter();
    }

    public void DrawCards(List<CardFS> cards)
    {
        int count = cards.Count;
        for (int i = count; i > 0; i--)
        {
            UICard card = GameObject.Instantiate(itemCardUI, transCards);
            card.Init(i, cards[count - i]);
            Cards[cards[count - i].id] = card;
        }
        CardsSizeFitter();
    }

    public void DisCard(List<CardFS> cards, int playerId)
    {
        var user = playerId;
        int count = 0;
        foreach (var card in cards)
        {
            count++;
            if (user == GameManager.SelfPlayerId && card != null)
            {
                int cardId = card.id;
                if (Cards.ContainsKey(cardId))
                {
                    var uiCard = Cards[cardId];
                    Cards.Remove(cardId);
                    uiCard.transform.SetParent(transCardsDised);
                    uiCard.transform.localScale = new Vector3(0.5f, 0.5f);
                    CardsSizeFitter();
                    Vector3 to = transCardsDised.position;
                    StartCoroutine(DoMove(uiCard.transform, uiCard.transform.position, to, 0.1f, () =>
                    {
                        Destroy(uiCard.gameObject);
                    }));
                }
            }
            else if (user != GameManager.SelfPlayerId && card != null)
            {
                var uiCard = GameObject.Instantiate(itemCardUI, transCardsDised);
                uiCard.transform.position = Players[user].transform.position + (count - 1) * new Vector3(10, 0); ;
                uiCard.transform.localScale = new Vector3(0.5f, 0.5f);
                uiCard.SetInfo(card);
                uiCard.gameObject.SetActive(true);
                Vector3 to = transCardsDised.position;
                StartCoroutine(DoMove(uiCard.transform, Players[user].transform.position, to, 0.1f, () =>
                {
                    Destroy(uiCard.gameObject);
                }));
            }
        }

    }

    public void GiveCards(int from, int to, List<CardFS> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            UICard uICard;
            Vector3 fromPos;
            Vector3 toPos = Players[to].transform.position;
            if (from == GameManager.SelfPlayerId)
            {
                int cardId = cards[i].id;

                uICard = Cards[cardId];
                Cards.Remove(cardId);
                uICard.transform.SetParent(transCardsUsed);
                uICard.transform.localScale = new Vector3(0.5f, 0.5f);
                fromPos = uICard.transform.position;
                CardsSizeFitter();
            }
            else
            {
                uICard = GameObject.Instantiate(itemCardUI, transCardsUsed);
                uICard.transform.localScale = new Vector3(0.5f, 0.5f);
                fromPos = Players[from].transform.position + new Vector3(i * 20, 0);
            }
            uICard.gameObject.SetActive(true);
            StartCoroutine(DoMove(uICard.transform, fromPos, toPos, 0.1f, () =>
            {
                Destroy(uICard.gameObject);
            }));

        }

    }
    public void DisCards(List<CardFS> cards)
    {
        foreach (var card in cards)
        {
            int cardId = card.id;

            if (Cards.ContainsKey(cardId))
            {
                Cards[cardId].OnDiscard();
                Cards.Remove(cardId);
                CardsSizeFitter();
            }
        }
    }

    public void ClearCards()
    {
        if (Cards.Count > 0)
        {
            foreach (var kv in Cards)
            {
                GameObject.Destroy(kv.Value.gameObject);
            }
            Cards.Clear();
        }
    }

    private void CardsSizeFitter()
    {
        if (Cards.Count * gridCards.cellSize.x <= transCards.sizeDelta.x)
        {
            gridCards.spacing = Vector2.zero;
        }
        else
        {
            float spacingX = -(Cards.Count * gridCards.cellSize.x - transCards.sizeDelta.x) / (Cards.Count - 1);
            gridCards.spacing = new Vector2(spacingX, 0);
        }
    }
    public void SetDeckNum(int num)
    {
        textDeckCount.text = "" + num;
    }

    public void AddMsg(string v)
    {
        textInfo.text = textInfo.text + "\n" + v;
        //Debug.Log(v);
        //throw new NotImplementedException();
    }

    public void OnclickUserCard()
    {
        if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
        {
            Debug.LogError("不在自己的相应时间");
            return;
        }

        if (GameManager.Singleton.IsWaitGiveCard)
        {
            GameManager.Singleton.SendDieGiveCards();
            return;
        }

        //有人濒死求澄清
        if (GameManager.Singleton.IsWaitSaving != -1)
        {
            if (GameManager.Singleton.GetCardSelect() != null && GameManager.Singleton.GetCardSelect().cardName == CardNameEnum.ChengQing)
            {
                ShowPlayerMessageInfo(GameManager.Singleton.IsWaitSaving, true);
            }
        }
        //自己出牌阶段
        else if (GameManager.Singleton.curPhase == PhaseEnum.Main_Phase && GameManager.Singleton.CurTurnPlayerId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.SendUseCard();
        }
        //自己开始传情报阶段
        else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Start_Phase)
        {
            GameManager.Singleton.SendMessage();
        }
        //情报传递阶段，情报到自己面前时
        else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Phase)
        {
            bool usePoYi = false;
            if (GameManager.Singleton.SelectCardId != -1)
            {
                usePoYi = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId].cardName == CardNameEnum.PoYi;
            }
            if (usePoYi)
            {
                if(messageCard.IsUnknown())
                {
                    GameManager.Singleton.SendUserPoYi();
                }
                else
                {
                    ShowInfo("情报已经被翻开，不需要破译");
                }

            }
            else
            {
                GameManager.Singleton.SendWhetherReceive(true);
            }
        }
        else if (GameManager.Singleton.curPhase == PhaseEnum.Fight_Phase)
        {
            GameManager.Singleton.SendUseCard();
        }

    }
    public void OnclickEnd()
    {
        if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
        {
            Debug.LogError("不在自己的相应时间");
            return;
        }
        if (GameManager.Singleton.IsWaitSaving != -1)
        {
            GameManager.Singleton.SendWhetherSave(false);
            return;
        }
        if (GameManager.Singleton.IsWaitGiveCard)
        {
            ProtoHelper.SendDieGiveCard(GameManager.Singleton.seqId);
            return;
        }
        if (GameManager.Singleton.IsWaitLock)
        {
            GameManager.Singleton.SendMessage();
            return;
        }

        //自己出牌阶段
        if (GameManager.Singleton.curPhase == PhaseEnum.Main_Phase && GameManager.Singleton.CurTurnPlayerId == GameManager.SelfPlayerId)
        {
            GameManager.Singleton.SendEndWaiting();
        }
        else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Phase)
        {
            GameManager.Singleton.SendWhetherReceive(false);
        }
        else if (GameManager.Singleton.curPhase == PhaseEnum.Fight_Phase)
        {
            GameManager.Singleton.SendEndFightPhase();
        }
    }

    public void SetLock(List<int> lockIds)
    {
        foreach (var id in lockIds)
        {
            Players[id].SetLock(true);
        }
    }

    public void ClearLock()
    {
        foreach (var id_player in Players)
        {
            id_player.Value.SetLock(false);
        }
    }

    public void ShowShiTanInfo(CardFS card, int waitingTime)
    {
        shiTanInfo.Show(card, waitingTime);
    }

    public void HideShiTanInfo()
    {
        shiTanInfo.gameObject.SetActive(false);
    }
    public void OnUseCard(int user, int targetUser, CardFS card = null)
    {
        if (Players.ContainsKey(user))
        {
            Players[user].UseCard(card);
        }

        if (user == GameManager.SelfPlayerId && card != null)
        {
            int cardId = card.id;
            if (Cards.ContainsKey(cardId))
            {
                var uiCard = Cards[cardId];
                Cards.Remove(cardId);
                uiCard.transform.SetParent(transCardsUsed);
                uiCard.transform.localScale = new Vector3(0.5f, 0.5f);
                CardsSizeFitter();
                Vector3 to = card.cardName == CardNameEnum.DiaoBao ? messageCard.transform.position : transCardsUsed.position;
                StartCoroutine(DoMove(uiCard.transform, uiCard.transform.position, to, 0.1f, () =>
                {
                    if (card.cardName == CardNameEnum.DiaoBao)
                    {
                        Destroy(uiCard.gameObject);
                    }
                    else
                    {
                        Destroy(uiCard.gameObject, 2);
                    }
                }));
            }
        }
        else if (user != GameManager.SelfPlayerId && card != null)
        {
            var uiCard = GameObject.Instantiate(itemCardUI, transCardsUsed);
            uiCard.transform.position = Players[user].transform.position;
            uiCard.transform.localScale = new Vector3(0.5f, 0.5f);
            uiCard.SetInfo(card);
            uiCard.gameObject.SetActive(true);
            Vector3 to = card.cardName == CardNameEnum.DiaoBao ? messageCard.transform.position : transCardsUsed.position;
            StartCoroutine(DoMove(uiCard.transform, Players[user].transform.position, to, 0.1f, () =>
            {
                if (card.cardName == CardNameEnum.DiaoBao)
                {
                    Destroy(uiCard.gameObject);
                }
                else
                {
                    Destroy(uiCard.gameObject, 2);
                }
            }));
        }
    }
    public void OnCardSend(int user, int cardId)
    {
        if (Players.ContainsKey(user))
        {
            Players[user].SendCard();
        }

        if (user == GameManager.SelfPlayerId && cardId != 0)
        {
            if (Cards.ContainsKey(cardId))
            {
                Cards[cardId].OnSend();
                Cards.Remove(cardId);
                CardsSizeFitter();
            }
        }
    }
    public void ShowMessagingDiaoBao(CardFS cardUsed)
    {
        //messageCard.TurnOn(cardUsed);

        var uiCard = GameObject.Instantiate(messageCard, transCardsDised);
        uiCard.transform.position = messageCard.transform.position;
        Vector3 to = transCardsDised.position;
        StartCoroutine(DoMove(uiCard.transform, messageCard.transform.position, to, 0.1f, () =>
        {
            if (uiCard.IsUnknown())
            {
                uiCard.TurnOn(cardUsed);
            }

            Destroy(uiCard.gameObject, 2);
        }, 0.3f));
    }

    public void OnPlayerMessageRemove(int playerId, List<CardFS> messages)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            UICard uICard = GameObject.Instantiate(itemCardUI, transCardsDised);
            uICard.SetInfo(messages[i]);
            Vector3 fromPos = Players[playerId].transform.position + new Vector3(i * 20, 0);
            Vector3 toPos = transCardsDised.position;
            uICard.transform.localScale = new Vector3(0.5f, 0.5f);
            uICard.gameObject.SetActive(true);

            StartCoroutine(DoMove(uICard.transform, fromPos, toPos, 0.1f, () =>
            {
                Destroy(uICard.gameObject);
            }));
        }
    }
    public void OnCardsGive(int from, int to, List<CardFS> cards)
    {

    }

    public void ShowTopCard(CardFS card)
    {
        Debug.LogError("展示了牌堆顶的牌，" + card.cardName);
    }

    public void ShowWeiBiSelect(bool show)
    {
        goWeiBiSelect.SetActive(show);
    }

    public void ShowWeiBiGiveCard(CardNameEnum cardWant, int user, int waitTime)
    {
        weiBiGiveCard.Show(cardWant, user, waitTime);
    }

    public void ShowPlayerMessageInfo(int playerId, bool showChengQing = false)
    {
        playerMessagInfo.Show(playerId, showChengQing);
    }

    public void HidePlayerMessageInfo()
    {
        playerMessagInfo.gameObject.SetActive(false);
    }

    public void InitMessageSenderPos(int messagePlayerId)
    {
        messageCard.transform.position = GetMessagePos(messagePlayerId);
    }
    public void HideMessagingCard()
    {
        messageCard.gameObject.SetActive(false);
        goDirect.SetActive(false);
    }
    public void ShowNextMessagePlayer(DirectionEnum messageCardDir)
    {
        goDirect.SetActive(true);
        if (messageCardDir == DirectionEnum.Up)
        {
            transDir.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (messageCardDir == DirectionEnum.Left)
        {
            transDir.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (messageCardDir == DirectionEnum.Right)
        {
            transDir.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
    public void ShowMessagingCard(CardFS message, int messagePlayerId = -1, bool move = false)
    {
        //Debug.LogError("情报id，" + message.id);
        messageCard.gameObject.SetActive(true);
        messageCard.SetInfo(message);
        if (messagePlayerId != -1)
        {
            if (move)
            {
                Vector3 from = messageCard.transform.position;
                Vector3 to = GetMessagePos(messagePlayerId);
                StartCoroutine(DoMove(messageCard.transform, from, to, 0.1f));
            }
            else
            {
                messageCard.transform.position = GetMessagePos(messagePlayerId);
            }
        }
    }

    private IEnumerator DoMove(Transform trans, Vector3 from, Vector3 to, float t, UnityAction callBack = null, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        float pt = 0;
        int count = 10;
        for (int i = 0; i <= count; i++)
        {
            pt = (float)i / count;
            yield return new WaitForSeconds(pt * t);
            trans.position = from + pt * (to - from);
        }
        if (callBack != null)
        {
            callBack.Invoke();
        }
    }

    private string infoStr = null;
    private Coroutine hideInfoCorout;
    public void ShowInfo(string info)
    {
        textMidInfo.text = infoStr == null ? info : infoStr + "\n" + info;
        textMidInfo.gameObject.SetActive(true);
        infoStr = info;
        hideInfoCorout = StartCoroutine(HideInfo());
    }

    private IEnumerator HideInfo()
    {
        string s = infoStr;
        yield return new WaitForSeconds(2);
        infoStr = null;
        textMidInfo.text = textMidInfo.text.Replace(s, "");
        //textMidInfo.gameObject.SetActive(false);
    }

    public void ShowPhase(string info = null)
    {
        if (info != null)
        {
            textPhase.text = info;
        }
        else if (GameManager.Singleton.IsWaitLock)
        {
            textPhase.text = "请选择锁定目标";
        }
        else if (GameManager.Singleton.IsWaitSaving != -1)
        {
            textPhase.text = "" + GameManager.Singleton.IsWaitSaving + "号玩家濒死，向" + GameManager.Singleton.CurWaitingPlayerId + "号请求澄清";
        }
        else if (GameManager.Singleton.IsWaitGiveCard)
        {
            textPhase.text = "你阵亡了，可以选择至多三张牌交给一名玩家";
        }
        else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Start_Phase)
        {
            if (GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId)
            {
                textPhase.text = "进入情报传递阶段，请传递一张情报";
            }
            else
            {
                textPhase.text = "";
                ShowInfo("" + GameManager.Singleton.CurWaitingPlayerId + "号玩家进入情报传递阶段，正在准备传递的情报");
            }
        }
        else
        {
            textPhase.text = "";
            //textPhase.text = LanguageUtils.GetPhaseName(GameManager.Singleton.curPhase);
        }
    }

    public void ShowWinInfo(List<int> playerId, List<int> winners, List<PlayerColorEnum> playerColers, List<SecretTaskEnum> playerTasks)
    {
        winInfo.Show(playerId, winners, playerColers, playerTasks);
    }

    public void ShowPoYiResult(CardFS messageCard)
    {
        poYiResult.Show(messageCard);
    }

    public void SetTask(SecretTaskEnum secretTask)
    {
        goTask.SetActive(GameManager.Singleton.GetPlayerColor() == PlayerColorEnum.Green);
        textTask.text = LanguageUtils.GetTaskName(secretTask);
    }

    public void OnMessageAccept(int playerId)
    {
        StartCoroutine(DoMessageScale());
    }

    private IEnumerator DoMessageScale()
    {
        yield return null;
        float pt = 0;
        float t = 0.1f;
        int count = 10;
        float origenScal = 0.5f;
        float scal = 0;
        for (int i = 0; i <= count; i++)
        {
            pt = (float)i / count;
            yield return new WaitForSeconds(pt * t);
            scal = origenScal * (2 * pt * (1 - pt) + 1);
            messageCard.transform.localScale = new Vector3(scal, scal, 1);
        }
    }

    public void RefreshIsCanCancel()
    {
        bool canCancel = true;
        bool canSure = true;
        if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
        {
            canCancel = false;
            canSure = false;
        }
        else if (GameManager.Singleton.IsWaitSaving != -1)
        {
            canCancel = true;
            canSure = true;
        }
        else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Phase)
        {
            bool isSend = GameManager.Singleton.CurTurnPlayerId == GameManager.SelfPlayerId;
            bool isLocked = GameManager.Singleton.lockedPlayer != null && GameManager.Singleton.lockedPlayer.Contains(GameManager.SelfPlayerId);
            if (isSend || isLocked)
            {
                canCancel = false;
            }
        }
        butCancel.interactable = canCancel;
        butSure.interactable = canSure;
    }

    public void ShowAddMessage(int messagePlayerId, CardFS message, bool isSend)
    {
        if (isSend)
        {
            var card = GameObject.Instantiate(messageCard, transform);
            if (card.IsUnknown())
            {
                card.TurnOn(message);
                StartCoroutine(DoMove(card.transform, card.transform.position, Players[messagePlayerId].transform.position, 0.05f, () => { Destroy(card.gameObject); }, 1f));
            }
            else
            {
                card.gameObject.SetActive(true);
                StartCoroutine(DoMove(card.transform, card.transform.position, Players[messagePlayerId].transform.position, 0.05f, () => { Destroy(card.gameObject); }, 1f));
            }
        }
        else
        {
            var card = GameObject.Instantiate(itemCardUI, transform);
            card.transform.position = textDeckCount.transform.position;
            card.transform.localScale = new Vector3(0.5f, 0.5f);
            card.SetInfo(message);
            card.gameObject.SetActive(true);
            StartCoroutine(DoMove(card.transform, card.transform.position, Players[messagePlayerId].transform.position, 0.05f, () => { Destroy(card.gameObject); }, 1f));
        }
    }

    private Coroutine sliderCorout;
    public void OnWaiting(int seconds)
    {
        if (seconds <= 0)
        {
            if (sliderCorout != null)
            {
                StopCoroutine(sliderCorout);
            }
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
            sliderCorout = StartCoroutine(ShowSlider(seconds));
        }
    }

    private IEnumerator ShowSlider(int seconds)
    {
        float total = seconds;
        float secondsF = seconds;
        slider.value = secondsF / total;
        while (secondsF > 0)
        {
            secondsF = secondsF - 0.1f;
            yield return new WaitForSeconds(0.1f);
            slider.value = secondsF / total;
        }
        yield return new WaitForEndOfFrame();
        slider.gameObject.SetActive(false);
    }

    public void ShowHandCard(int target, List<CardFS> cards)
    {
        playerMessagInfo.ShowHandCard(target, cards);
    }

    public void CheckTargetAvailable()
    {

        foreach (var kv in Players)
        {
            kv.Value.SetBanClick(false);
        }

        if (GameManager.Singleton.SelectCardId != -1)
        {
            var card = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId];
            if (GameManager.Singleton.IsWaitLock)
            {
                //传情报锁定时，不需要限定目标
            }
            else if (GameManager.Singleton.curPhase == PhaseEnum.Fight_Phase && card.cardName == CardNameEnum.WuDao)
            {
                bool banClick = false;
                foreach (var kv in Players)
                {
                    banClick = kv.Key != GameManager.Singleton.GetPlayerAliveLeft(GameManager.Singleton.CurMessagePlayerId)
                        && kv.Key != GameManager.Singleton.GetPlayerAliveRight(GameManager.Singleton.CurMessagePlayerId);
                    kv.Value.SetBanClick(banClick);
                }
            }
            else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Start_Phase && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && card.direction != DirectionEnum.Up)
            {
                bool banClick = false;
                foreach (var kv in Players)
                {
                    if (card.direction == DirectionEnum.Left)
                    {
                        banClick = kv.Key != GameManager.Singleton.GetPlayerAliveLeft(GameManager.SelfPlayerId);
                    }
                    else
                    {
                        banClick = kv.Key != GameManager.Singleton.GetPlayerAliveRight(GameManager.SelfPlayerId);
                    }
                    kv.Value.SetBanClick(banClick);
                }

            }
        }
    }
}
