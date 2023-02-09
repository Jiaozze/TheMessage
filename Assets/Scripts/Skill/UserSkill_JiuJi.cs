using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【就计】A：你被【试探】【威逼】或【利诱】指定为目标后，你可以翻开此角色牌，然后摸两张牌。B：并在触发此技能的卡牌结算后，将其加入你的手牌。
public class UserSkill_JiuJi : SkillBase
{
    public UserSkill_JiuJi(int id)
    {
        playerId = id;
    }

    public override string name => "就计";

    public override bool canUse => false;

    public override string Des => "就计：你被【试探】【威逼】或【利诱】指定为目标后，你可以翻开此角色牌，然后摸两张牌。并在触发此技能的卡牌结算后，将其加入你的手牌\n";

    public static void OnReceiveUseA(int playerId)
    {
        string s = "" + GameManager.Singleton.players[playerId].name + "发动了技能就计";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, CardFS cardFS, uint unknownCardCount)
    {
        if(cardFS != null)
        {
            GameManager.Singleton.players[playerId].cardCount += 1;
            GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
            if(playerId == GameManager.SelfPlayerId)
            {
                GameManager.Singleton.cardsHand.Add(cardFS.id, cardFS);
                GameManager.Singleton.gameUI.DrawCards(new List<CardFS>() { cardFS });
            }
            string s = "" + GameManager.Singleton.players[playerId].name + "通过技能就计获得一张" + LanguageUtils.GetCardName(cardFS.cardName); 
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
        else if(unknownCardCount == 1)
        {
            GameManager.Singleton.players[playerId].cardCount += 1;
            GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
            string s = "" + GameManager.Singleton.players[playerId].name + "通过技能就计获得一张试探";
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
    }
}
