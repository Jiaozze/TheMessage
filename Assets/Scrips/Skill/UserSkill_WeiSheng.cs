using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【尾声】：你获得胜利时，没有身份牌的玩家与你一同获得胜利
public class UserSkill_WeiSheng : SkillBase
{
    public UserSkill_WeiSheng(int id)
    {
        playerId = id;
    }

    public override string name => "尾声";

    public override bool canUse => false;

    public override string Des => "尾声：你获得胜利时，没有身份牌的玩家与你一同获得胜利\n";

    //public static void OnReceiveUse(int playerId)
    //{
    //    string s = "" + playerId + "号玩家发动了技能明饵";
    //    GameManager.Singleton.gameUI.ShowInfo(s);
    //    GameManager.Singleton.gameUI.AddMsg(s);

    //}

}
