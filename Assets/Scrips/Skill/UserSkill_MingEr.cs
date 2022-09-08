using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【明饵】：你传出的红色或蓝色情报被接收后，你和接收者各摸一张牌
public class UserSkill_MingEr : SkillBase
{
    public UserSkill_MingEr(int id)
    {
        playerId = id;
    }

    public override string name => "明饵";

    public override bool canUse => false;

    public static void OnReceiveUse(int playerId)
    {
        string s = "" + playerId + "号玩家发动了技能明饵";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }

}
