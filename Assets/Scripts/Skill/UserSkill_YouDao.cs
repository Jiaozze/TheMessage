using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【诱导】：你使用【误导】后，摸一张牌。
public class UserSkill_YouDao : SkillBase
{
    public UserSkill_YouDao(int id)
    {
        playerId = id;
    }

    public override string name => "诱导";

    public override bool canUse => false;

    public override string Des => "诱导：你使用【误导】后，摸一张牌。\n";

    public static void OnReceiveUse(int playerId)
    {
        string s = "" + GameManager.Singleton.players[playerId].name + "发动了技能诱导";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

}
