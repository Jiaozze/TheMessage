using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SouJiSelect : MonoBehaviour
{
    public Text textTittleHand;
    public GridLayoutGroup gridHand;
    public UICard cardMessage;
    public UICard itemCardUI;

    private Dictionary<int, UICard> items = new Dictionary<int, UICard>();

    // Start is called before the first frame update
    void Start()
    {
        cardMessage.SetClickAction(() =>
        {
            cardMessage.OnSelect(!cardMessage.isSelect);
            if (GameManager.Singleton.selectSkill is UserSkill_SouJi)
            {
                UserSkill_SouJi skill_SouJi = GameManager.Singleton.selectSkill as UserSkill_SouJi;
                skill_SouJi.OnClickMessage(cardMessage.isSelect);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(int playerId, List<CardFS> cards, CardFS message)
    {
        gameObject.SetActive(true);

        foreach (var kv in items)
        {
            GameObject.Destroy(kv.Value.gameObject);
        }
        items.Clear();

        ShowHandsCard(playerId, cards);
        ShowMessageCard(message);
    }

    private void ShowHandsCard(int playerId, List<CardFS> cards)
    {
        textTittleHand.text = "" + GameManager.Singleton.players[playerId].name + "的手牌";

        UserSkill_SouJi skill_SouJi = null;
        if (GameManager.Singleton.selectSkill is UserSkill_SouJi)
        {
            skill_SouJi = GameManager.Singleton.selectSkill as UserSkill_SouJi;
        }

        int i = cards.Count;
        foreach (var cardFS in cards)
        {
            UICard card = GameObject.Instantiate(itemCardUI, gridHand.transform);
            int cardId = cardFS.id;
            card.Init(i, cardFS);
            items[cardFS.id] = (card);
            if (skill_SouJi != null)
            {
                if (cardFS.color.Contains(CardColorEnum.Black))
                {
                    card.SetClickAction(() =>
                    {
                        card.OnSelect(!card.isSelect);

                        skill_SouJi.OnClickOthersCard(playerId, cardId, card.isSelect);
                    });
                }
                else
                {
                    card.SetClickAction(() =>
                    {
                        GameManager.Singleton.gameUI.ShowInfo("只能选择黑色牌加入手牌");
                    });
                }
            }
        }
    }

    private void ShowMessageCard(CardFS message)
    {
        cardMessage.Init(1, message);
        cardMessage.OnSelect(false);
        if (message.color.Contains(CardColorEnum.Black))
        {
            cardMessage.SetClickAction(() =>
            {
                cardMessage.OnSelect(!cardMessage.isSelect);
                if (GameManager.Singleton.selectSkill is UserSkill_SouJi)
                {
                    UserSkill_SouJi skill_SouJi = GameManager.Singleton.selectSkill as UserSkill_SouJi;
                    skill_SouJi.OnClickMessage(cardMessage.isSelect);
                }
            });
        }
        else
        {
            cardMessage.SetClickAction(() =>
            {
                GameManager.Singleton.gameUI.ShowInfo("只能选择黑色牌加入手牌");
            });
        }
    }

}
