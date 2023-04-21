using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

public class FengYunBianHuanRP : MonoBehaviour
{
    public Text LogText;
    public Button TakeHandCardsButton;
    public Button TakeMessageButton;
    public GridLayoutGroup CardsBox;
    public UICard boxCard;
    public uint seq;

    private FengyunbianhuanModel model;
    private int chooseCardId;
    
    

    private void Start()
    {
        //监听boxcards数组减少时，移除UI中的相应卡牌
        model.boxCards
            .ObserveRemove()
            .Subscribe(_ =>
            {
                foreach (var card in CardsBox.GetComponentsInChildren<UICard>())
                {
                    if(card.cardId == _.Value.id)
                    {
                        Destroy(card.gameObject);
                    }
                }
            })
            .AddTo(this);

        //轮到自己选牌时，待选卡牌添加监听器获取点选卡片id和颜色
        model.isTarget
            .Select(_ => _ == true)
            .Subscribe(_ =>
            {
                foreach (var card in CardsBox.GetComponentsInChildren<UICard>())
                {
                    List<CardColorEnum> cardColor = new List<CardColorEnum>();
                    foreach (var cardfs in model.boxCards)
                    {
                        if(card.cardId == cardfs.id)
                        {
                            cardColor = cardfs.color;
                        }                       
                    }
                    card.SetClickAction(() =>
                    {
                        foreach(var acard in CardsBox.GetComponentsInChildren<UICard>())
                        {
                            acard.OnSelect(false);
                        }
                        card.OnSelect(true);
                        model.chooseCardInfo.Add(cardColor);
                        chooseCardId = card.cardId;
                    });
                }
                
            }).AddTo(this);

        //监听choosecard增加时，变更两个提交按钮状态
        model.chooseCardInfo
            .ObserveAdd()           
            .Subscribe(_ =>
            {
                if(model.isTarget.Value == true)
                {
                    TakeHandCardsButton.interactable = true;
                    if((_.Value.Contains(CardColorEnum.Black) && GameManager.Singleton.players[GameManager.SelfPlayerId].GetMessageCount(CardColorEnum.Black) == 0)
                    && ((_.Value.Contains(CardColorEnum.Red) && GameManager.Singleton.players[GameManager.SelfPlayerId].GetMessageCount(CardColorEnum.Red) == 0)
                    || (_.Value.Contains(CardColorEnum.Blue) && GameManager.Singleton.players[GameManager.SelfPlayerId].GetMessageCount(CardColorEnum.Blue) == 0)))
                    {
                        TakeMessageButton.interactable = true;
                    }
                    else if(_.Value.Count == 1 &&((_.Value.Contains(CardColorEnum.Black) && GameManager.Singleton.players[GameManager.SelfPlayerId].GetMessageCount(CardColorEnum.Black) == 0)
                    || (_.Value.Contains(CardColorEnum.Red) && GameManager.Singleton.players[GameManager.SelfPlayerId].GetMessageCount(CardColorEnum.Red) == 0)
                    || (_.Value.Contains(CardColorEnum.Blue) && GameManager.Singleton.players[GameManager.SelfPlayerId].GetMessageCount(CardColorEnum.Blue) == 0)))
                    {
                        TakeMessageButton.interactable = true;
                    }
                    else
                    {
                        TakeMessageButton.interactable = false;
                    }
                }
            }).AddTo(this);
    }

    //初始化UI和数据模型
    public void InitUI(List<CardFS> cards)
    {
        //实例化UI模型
        model = new FengyunbianhuanModel();
        //监听boxcards数组增加时，向UI中添加卡牌
        model.boxCards
            .ObserveAdd()
            .Subscribe(_ =>
            {
                UICard card = Instantiate(boxCard, CardsBox.transform);
                //card.transform.SetParent(CardsBox.transform, false);
                card.Init(1, _.Value);
            })
            .AddTo(this);
        //将card信息填入模型中
        foreach (CardFS card in cards)
        {
            model.boxCards.Add(card);
        }
    }

    public void SetTarget(bool isTarget)
    {
        model.isTarget.Value = isTarget;
    }

    public CardFS PopCardFromBox(int cardId)
    {
        for (int i = 0; i < model.boxCards.Count; i++)
        {
            var card = model.boxCards[i];
            if (card.id == cardId)
            {
                model.boxCards.RemoveAt(i);
                return card;
            }
        }
        // If card with the specified id is not found, return null or throw an exception
        return null; // or throw new Exception("Card not found in the box");
    }

    public void TakeHandCard()
    {
        int selfId = GameManager.SelfPlayerId;
        int cardId = chooseCardId;
        GameManager.Singleton.SendFengYunBianHuanChooseCardToHandCard(selfId, cardId, seq);
        TakeMessageButton.interactable = false;
        TakeHandCardsButton.interactable = false;
    }

    public void TakeMessage()
    {
        int selfId = GameManager.SelfPlayerId;
        int cardId = chooseCardId;
        GameManager.Singleton.SengFengyunBianHuanChooseCardToMessage(selfId, cardId, seq);
        TakeMessageButton.interactable = false;
        TakeHandCardsButton.interactable = false;
    }

    public void DestorySelf()
    {
        if(model.boxCards.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
