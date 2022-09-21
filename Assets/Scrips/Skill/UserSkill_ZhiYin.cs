using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【知音】：你接收红色或蓝色情报后，你和传出者各摸一张牌
public class UserSkill_ZhiYin : SkillBase
{
    public UserSkill_ZhiYin(int id)
    {
        playerId = id;
    }

    public override string name => "知音";

    public override bool canUse => false;

    public override string Des => "知音：你接收红色或蓝色情报后，你和传出者各摸一张牌\n";

    public static void OnReceiveUse(int playerId)
    {
        string s = "" + GameManager.Singleton.players[playerId].name + "发动了技能知音";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }

}
