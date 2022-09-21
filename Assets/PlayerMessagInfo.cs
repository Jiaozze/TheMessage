using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMessagInfo : MonoBehaviour
{
    public Text textTittle;
    public Button butChengQing;
    public GridLayoutGroup grid;
    public UICard itemCardUI;

    private Dictionary<int, UICard> items = new Dictionary<int, UICard>();

    private int cardId = 0;
    private bool isShowHand = false;
    private void Awake()
    {
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        cardId = 0;
        if (GameManager.Singleton.IsUsingSkill && GameManager.Singleton.selectSkill != null)
        {
            GameManager.Singleton.selectSkill.OnMessageInfoClose();
        }
        isShowHand = false;
    }

    public void Show(int playerId, bool showChengQing = false)
    {
        if (isShowHand)
        {
            //展示其他角色手牌和情报用的一个ui，展示手牌的时候就不让展示情报了
            GameManager.Singleton.gameUI.ShowInfo("正在展示手牌无法查看情报");
            return;
        }

        gameObject.SetActive(true);
        Debug.LogError(playerId);
        textTittle.text = "" + GameManager.Singleton.players[playerId].name + "的情报";

        foreach (var kv in items)
        {
            GameObject.Destroy(kv.Value.gameObject);
        }
        items.Clear();

        int i = GameManager.Singleton.players[playerId].messages.Count;
        foreach (var msg in GameManager.Singleton.players[playerId].messages)
        {
            UICard card = GameObject.Instantiate(itemCardUI, grid.transform);
            card.Init(i, msg);
            card.SetClickAction(() =>
            {
                if (items.ContainsKey(cardId))
                {
                    items[cardId].OnSelect(false);
                }
                if (cardId != msg.id)
                {
                    cardId = msg.id;
                    if (items.ContainsKey(cardId))
                    {
                        items[cardId].OnSelect(true);
                    }
                    butChengQing.interactable = true;
                }
                else
                {
                    butChengQing.interactable = false;
                    cardId = 0;
                }
                if (GameManager.Singleton.IsUsingSkill && GameManager.Singleton.selectSkill != null)
                {
                    GameManager.Singleton.selectSkill.OnMessageSelect(playerId, cardId);
                }
            });
            items[msg.id] = (card);
        }

        butChengQing.gameObject.SetActive(showChengQing);
        if (GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId
    && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase
    //&& GameManager.Singleton.SelectPlayerId != playerId
    && GameManager.Singleton.SelectCardId != -1
    && GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId].cardName == CardNameEnum.ChengQing)
        {
            if(GameManager.Singleton.SelectPlayerId != playerId)
            {
                GameManager.Singleton.SelectPlayerId = playerId;
            }
            butChengQing.gameObject.SetActive(true);
        }
    }

    public void ShowHandCard(int playerId, List<CardFS> cards)
    {
        isShowHand = true;
        gameObject.SetActive(true);
        textTittle.text = "" + GameManager.Singleton.players[playerId].name + "的手牌";

        foreach (var kv in items)
        {
            GameObject.Destroy(kv.Value.gameObject);
        }
        items.Clear();

        UserSkill_JingMeng skill_JingMeng = null;
        if (GameManager.Singleton.selectSkill is UserSkill_JingMeng)
        {
            skill_JingMeng = GameManager.Singleton.selectSkill as UserSkill_JingMeng;
        }

        int i = cards.Count;
        foreach (var cardFS in cards)
        {
            UICard card = GameObject.Instantiate(itemCardUI, grid.transform);
            card.Init(i, cardFS);
            items[cardFS.id] = (card);
            if(skill_JingMeng != null)
            {
                card.SetClickAction(() =>
                {
                    if (items.ContainsKey(cardId))
                    {
                        items[cardId].OnSelect(false);
                    }
                    if (cardId != cardFS.id)
                    {
                        cardId = cardFS.id;
                        if (items.ContainsKey(cardId))
                        {
                            items[cardId].OnSelect(true);
                        }
                        butChengQing.interactable = true;
                    }
                    else
                    {
                        butChengQing.interactable = false;
                        cardId = 0;
                    }

                    skill_JingMeng.OnClickOthersCard(playerId, cardId);
                });
            }
        }

        butChengQing.gameObject.SetActive(false);

    }

    public void Hide()
    {
        if(isShowHand && GameManager.Singleton.IsUsingSkill && GameManager.Singleton.selectSkill is UserSkill_JingMeng)
        {
            UserSkill_JingMeng skill_JingMeng = GameManager.Singleton.selectSkill as UserSkill_JingMeng;
            if(skill_JingMeng.cards != null)
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择一张牌弃置");
                return;
            }
        }
        gameObject.SetActive(false);
    }

    public void OnClickChengQing()
    {
        var curCard = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId];

        if(GameManager.Singleton.IsWaitSaving != -1 && cardId != 0 && curCard.cardName == CardNameEnum.ChengQing)
        {
            foreach (var message in GameManager.Singleton.players[GameManager.Singleton.IsWaitSaving].messages)
            {
                if (message.id == cardId)
                {
                    if (!message.color.Contains(CardColorEnum.Black))
                    {
                        GameManager.Singleton.gameUI.ShowInfo("需要选择黑色情报澄清");
                        return;
                    }
                    break;
                }
            }

            GameManager.Singleton.SendWhetherSave(true, cardId);
        }
        else if (cardId != 0 && curCard.cardName == CardNameEnum.ChengQing && GameManager.Singleton.SelectPlayerId != -1)
        {
            foreach(var message in GameManager.Singleton.players[GameManager.Singleton.SelectPlayerId].messages)
            {
                if(message.id == cardId)
                {
                    if(!message.color.Contains(CardColorEnum.Black))
                    {
                        GameManager.Singleton.gameUI.ShowInfo("需要选择黑色情报澄清");
                        return;
                    }
                    break;
                }
            }

            ProtoHelper.SendUseCardMessage_ChengQing(GameManager.Singleton.SelectCardId, GameManager.Singleton.SelectPlayerId, cardId, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("选择正确的澄清角色/卡牌");
            Debug.LogError("" + GameManager.Singleton.SelectPlayerId + cardId + GameManager.Singleton.IsWaitSaving + curCard.cardName);
        }
    }
}
