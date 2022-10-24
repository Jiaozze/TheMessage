using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiaoShouSelect : MonoBehaviour
{
    public Text textTittleHand;
    public Text textTittleMessage;
    public GridLayoutGroup gridHand;
    public GridLayoutGroup gridMessage;
    public UICard itemCardUI;

    private Dictionary<int, UICard> items = new Dictionary<int, UICard>();

    private int cardId = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(int playerId, List<CardFS> cards)
    {
        gameObject.SetActive(true);
        cardId = 0;

        foreach (var kv in items)
        {
            GameObject.Destroy(kv.Value.gameObject);
        }
        items.Clear();

        ShowHandsCard(playerId, cards);
        ShowMessageCard(playerId);
    }

    private void ShowHandsCard(int playerId, List<CardFS> cards)
    {
        textTittleHand.text = "" + GameManager.Singleton.players[playerId].name + "的手牌";

        UserSkill_MiaoShou skill_MiaoShou = null;
        if (GameManager.Singleton.selectSkill is UserSkill_MiaoShou)
        {
            skill_MiaoShou = GameManager.Singleton.selectSkill as UserSkill_MiaoShou;
        }

        int i = cards.Count;
        foreach (var cardFS in cards)
        {
            UICard card = GameObject.Instantiate(itemCardUI, gridHand.transform);
            card.Init(i, cardFS);
            items[cardFS.id] = (card);
            if (skill_MiaoShou != null)
            {
                card.SetClickAction(() =>
                {
                    if (items.ContainsKey(cardId))
                    {
                        items[cardId].OnSelect(false);
                    }
                    cardId = cardFS.id;
                    if (items.ContainsKey(cardId))
                    {
                        items[cardId].OnSelect(true);
                    }

                    skill_MiaoShou.OnClickOthersCard(playerId, cardId);
                });
            }
        }
    }

    private void ShowMessageCard(int playerId)
    {
        textTittleMessage.text = "" + GameManager.Singleton.players[playerId].name + "的情报";

        int i = GameManager.Singleton.players[playerId].messages.Count;
        foreach (var msg in GameManager.Singleton.players[playerId].messages)
        {
            UICard card = GameObject.Instantiate(itemCardUI, gridMessage.transform);
            card.Init(i, msg);
            card.SetClickAction(() =>
            {
                if (items.ContainsKey(cardId))
                {
                    items[cardId].OnSelect(false);
                }
                cardId = msg.id;
                if (items.ContainsKey(cardId))
                {
                    items[cardId].OnSelect(true);
                }

                if (GameManager.Singleton.IsUsingSkill && GameManager.Singleton.selectSkill != null)
                {
                    GameManager.Singleton.selectSkill.OnMessageSelect(playerId, cardId);
                }
            });
            items[msg.id] = (card);
        }
    }

}
