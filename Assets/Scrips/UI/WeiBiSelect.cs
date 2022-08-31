using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeiBiSelect : MonoBehaviour
{
    public Toggle togJieHuo;
    public Toggle togWuDao;
    public Toggle togDiaoBao;
    public Toggle togChengQing;

    public void OnClickCancel()
    {
        gameObject.SetActive(false);
    }

    public void OnClickSure()
    {
        var curCard = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId];
        if (curCard.cardName == CardNameEnum.WeiBi && GameManager.Singleton.SelectPlayerId != -1 && GameManager.Singleton.SelectPlayerId != 0)
        {
            CardNameEnum cardNameEnum = CardNameEnum.JieHuo;
            if (togChengQing.isOn) cardNameEnum = CardNameEnum.ChengQing;
            if (togDiaoBao.isOn) cardNameEnum = CardNameEnum.DiaoBao;
            if (togJieHuo.isOn) cardNameEnum = CardNameEnum.JieHuo;
            if (togWuDao.isOn) cardNameEnum = CardNameEnum.WuDao;

            ProtoHelper.SendUseCardMessage_WeiBi(GameManager.Singleton.SelectCardId, GameManager.Singleton.SelectPlayerId, GameManager.Singleton.seqId, cardNameEnum);
            gameObject.SetActive(false);
        }
    }
}
