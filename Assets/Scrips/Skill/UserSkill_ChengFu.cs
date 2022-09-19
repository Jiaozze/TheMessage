using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【城府】：【试探】和【威逼】对你无效。
public class UserSkill_ChengFu : SkillBase
{
    public UserSkill_ChengFu(int id)
    {
        playerId = id;
    }

    public override string name => "城府";

    public override bool canUse => false;

    public override string Des => "城府：【试探】和【威逼】对你无效\n";

    public static void OnReceiveUse(int playerId, int userId, CardFS card, int cardCount)
    {
        string cardName = "";
        if(cardCount == 1)
        {
            GameManager.Singleton.OnRecerveUseShiTan(userId, playerId);
            cardName = "试探无效";
        }
        else if(card != null)
        {
            if(card.cardName == CardNameEnum.ShiTan)
            {
                GameManager.Singleton.OnRecerveUseShiTan(userId, playerId, card.id);
                cardName = "试探无效";
            }
            else if(card.cardName == CardNameEnum.WeiBi)
            {
                GameManager.Singleton.OnCardUse(userId, card, playerId);
                cardName = "威逼无效";
            }
        }
        else //肥原龙川可以视为使用了【威逼】
        {
            cardName = "威逼无效";
        }

        string s = "" + GameManager.Singleton.players[playerId].name + "触发了技能城府 " + cardName;
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

}
