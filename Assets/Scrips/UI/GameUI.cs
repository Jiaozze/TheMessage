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
    public Text textDeckCount;
    public RectTransform transCards;
    public Transform transPlayerSelf;
    public GridLayoutGroup gridCards;
    public GridLayoutGroup topPlayerGrid;
    public GridLayoutGroup leftPlayerGrid;
    public GridLayoutGroup rightPlayerGrid;
    public Button butCancel;
    public Button butSure;
    public Transform selfMessagePos;

    public Dictionary<int, UICard> Cards = new Dictionary<int, UICard>();
    public Dictionary<int, UIPlayer> Players = new Dictionary<int, UIPlayer>();
    private Dictionary<int, Vector3> messagePoses = new Dictionary<int, Vector3>();
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

        int leftNum = (num - 1) / 3;
        int topNum = num - 1 - 2 * leftNum;

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

        StartCoroutine(InitMssagePoses(leftNum, topNum));
    }

    private IEnumerator InitMssagePoses(int leftNum, int topNum)
    {
        yield return new WaitForEndOfFrame();
        if (messagePoses.Count > 0)
        {
            messagePoses.Clear();
        }
        messagePoses[0] = selfMessagePos.position;
        for (int i = 1; i < leftNum + 1; i++)
        {
            messagePoses[i] = Players[i].transform.position + new Vector3(-100, 0);
        }
        for (int i = 1 + leftNum; i < 1 + leftNum + topNum; i++)
        {
            messagePoses[i] = Players[i].transform.position + new Vector3(0, -150);
        }
        for (int i = 1 + leftNum + topNum; i < 1 + leftNum + topNum + leftNum; i++)
        {
            messagePoses[i] = Players[i].transform.position + new Vector3(100, 0);
        }
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

    public void DisCards(List<CardFS> cards)
    {
        foreach (var card in cards)
        {
            int cardId = card.id;

            if (Cards.ContainsKey(cardId))
            {
                Cards[cardId].OnDiscard();
                Cards.Remove(cardId);
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
            Debug.LogError("�����Լ�����Ӧʱ��");
            return;
        }

        if (GameManager.Singleton.IsWaitGiveCard)
        {
            GameManager.Singleton.SendDieGiveCards();
            return;
        }

        //���˱��������
        if (GameManager.Singleton.IsWaitSaving != -1)
        {
            if (GameManager.Singleton.GetCardSelect() != null && GameManager.Singleton.GetCardSelect().cardName == CardNameEnum.Cheng_Qing)
            {
                ShowPlayerMessageInfo(GameManager.Singleton.IsWaitSaving, true);
            }
        }
        //�Լ����ƽ׶�
        else if (GameManager.Singleton.curPhase == PhaseEnum.Main_Phase)
        {
            GameManager.Singleton.SendUseCard();
        }
        //�Լ���ʼ���鱨�׶�
        else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Start_Phase)
        {
            GameManager.Singleton.SendMessage();
        }
        //�鱨���ݽ׶Σ��鱨���Լ���ǰʱ
        else if (GameManager.Singleton.curPhase == PhaseEnum.Send_Phase)
        {
            bool usePoYi = false;
            if (GameManager.Singleton.SelectCardId != -1)
            {
                usePoYi = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId].cardName == CardNameEnum.Po_Yi;
            }
            if (usePoYi)
            {
                GameManager.Singleton.SendUserPoYi();
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
            Debug.LogError("�����Լ�����Ӧʱ��");
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

        //�Լ����ƽ׶�
        if (GameManager.Singleton.curPhase == PhaseEnum.Main_Phase)
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
                Cards[cardId].OnUse();
                Cards.Remove(cardId);
            }
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
                Cards[cardId].OnUse();
                Cards.Remove(cardId);
            }
        }
    }
    public void ShowTopCard(CardFS card)
    {
        Debug.LogError("չʾ���ƶѶ����ƣ�" + card.cardName);
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
        if (messagePoses.ContainsKey(messagePlayerId))
        {
            messageCard.transform.position = messagePoses[messagePlayerId];
        }
    }

    public void ShowMessagingCard(CardFS message, int messagePlayerId = -1, bool move = false)
    {
        //Debug.LogError("�鱨id��" + message.id);
        messageCard.gameObject.SetActive(true);
        messageCard.SetInfo(message);
        if (messagePlayerId != -1)
        {
            if (move)
            {
                Vector3 from = messageCard.transform.position;
                Vector3 to = messagePoses[messagePlayerId];
                StartCoroutine(DoMove(messageCard.transform, from, to, 0.1f));
            }
            else
            {
                messageCard.transform.position = messagePoses[messagePlayerId];
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
    public void HideMessagingCard()
    {
        messageCard.gameObject.SetActive(false);
    }

    public void ShowPhase()
    {
        if (GameManager.Singleton.IsWaitLock)
        {
            textPhase.text = "��ѡ������Ŀ��";
        }
        else if (GameManager.Singleton.IsWaitGiveCard)
        {
            textPhase.text = "�������ˣ�����ѡ�����������ƽ���һ�����";
        }
        else
        {
            textPhase.text = LanguageUtils.GetPhaseName(GameManager.Singleton.curPhase);
        }
    }

    public void ShowWinInfo(int playerId, List<int> winners, List<PlayerColorEnum> playerColers, List<SecretTaskEnum> playerTasks)
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
            card.transform.localScale = new Vector3(0.5f, 0.5f);
            card.SetInfo(message);
            card.gameObject.SetActive(true);
            StartCoroutine(DoMove(card.transform, card.transform.position, Players[messagePlayerId].transform.position, 0.05f, () => { Destroy(card.gameObject); }, 1f));
        }
    }
}
