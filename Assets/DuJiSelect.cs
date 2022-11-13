using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuJiSelect : MonoBehaviour
{
    public Text textTittleHand;
    public GridLayoutGroup gridMessage;
    public UICard itemCardUI;

    private Dictionary<int, UICard> items = new Dictionary<int, UICard>();
    public static int selectCardId = 0;
    private Dictionary<int, int> dic_cardId_PlayerId = new Dictionary<int, int>();

    public void Show(List<int> cardIds, List<int> players)
    {
        gameObject.SetActive(true);
        dic_cardId_PlayerId.Clear();
        if(items.Count > 0)
        {
            foreach(var kv in items)
            {
                Destroy(kv.Value.gameObject);
            }
        }
        items.Clear();
        selectCardId = 0;
        textTittleHand.text = "选择一张牌由抽取者选择，置入自己的情报区/置入对方的情报区，或点取消将牌留在你的手中";

        for (int i = 0; i < cardIds.Count; i++)
        {
            if(GameManager.Singleton.cardsHand.ContainsKey(cardIds[i]))
            {
                int cardId = cardIds[i];
                dic_cardId_PlayerId.Add(cardIds[i], players[i]);
                var item = GameObject.Instantiate(itemCardUI, gridMessage.transform);
                item.gameObject.SetActive(true);
                item.SetInfo(GameManager.Singleton.cardsHand[cardIds[i]]);
                item.SetClickAction(() => { 
                    if(selectCardId != 0 && items.ContainsKey(selectCardId))
                    {
                        items[selectCardId].OnSelect(false);
                        selectCardId = 0;
                    }
                    if(items.ContainsKey(cardId))
                    {
                        items[cardId].OnSelect(true);
                        selectCardId = cardId;
                        textTittleHand.text = "让" + GameManager.Singleton.players[dic_cardId_PlayerId[cardId]].name + "选择置入自己的情报区/置入对方的情报区， 或点取消将牌留在你的手中";
                    }
                });
                items.Add(cardIds[i], item);
            }
            else
            {
                Debug.LogError("手牌没有对应卡牌，id：" + cardIds[i]);
            }
        }
    }

    //public void Sure()
    //{
    //    if(selectCardId != 0)
    //    {
    //        ProtoHelper.SendSkill_DuJiB(true, selectCardId, GameManager.Singleton.seqId);
    //    }
    //    else
    //    {
    //        GameManager.Singleton.gameUI.ShowInfo("选择一张牌， 令抽取者选择置入自己的情报区或对方情报区");
    //    }
    //}
    //public void Cancel()
    //{
    //    ProtoHelper.SendSkill_DuJiB(false, selectCardId, GameManager.Singleton.seqId);
    //}

    public void ShowUseC(int targetId, CardFS card)
    {
        gameObject.SetActive(true);
        dic_cardId_PlayerId.Clear();
        if (items.Count > 0)
        {
            foreach (var kv in items)
            {
                Destroy(kv.Value.gameObject);
            }
        }
        items.Clear();
        selectCardId = 0;

        var item = GameObject.Instantiate(itemCardUI, gridMessage.transform);
        item.gameObject.SetActive(true);
        item.SetInfo(card);
        items.Add(card.id, item);

        textTittleHand.text = "点确定将该牌置入自己情报区，或点取消将该牌置入" + GameManager.Singleton.players[targetId].name + "情报区";
    }
}
