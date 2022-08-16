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
        if (curCard.cardName == CardNameEnum.Wei_Bi && GameManager.Singleton.SelectPlayerId != -1 && GameManager.Singleton.SelectPlayerId != 0)
        {
            CardNameEnum cardNameEnum = CardNameEnum.Jie_Huo;
            if (togChengQing.isOn) cardNameEnum = CardNameEnum.Cheng_Qing;
            if (togDiaoBao.isOn) cardNameEnum = CardNameEnum.Diao_Bao;
            if (togJieHuo.isOn) cardNameEnum = CardNameEnum.Jie_Huo;
            if (togWuDao.isOn) cardNameEnum = CardNameEnum.Wu_Dao;

            ProtoHelper.SendUseCardMessage_WeiBi(GameManager.Singleton.SelectCardId, GameManager.Singleton.SelectPlayerId, GameManager.Singleton.seqId, cardNameEnum);
            gameObject.SetActive(false);
        }
    }
}
