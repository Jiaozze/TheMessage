using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【腹黑】：你传出的黑色情报被接收后，你摸一张牌。
public class UserSkill_FuHei : SkillBase
{
    public UserSkill_FuHei(int id)
    {
        playerId = id;
    }

    public override string name => "腹黑";

    public override bool canUse => false;

    public override string Des => "腹黑：你传出的黑色情报被接收后，你摸一张牌。\n";

    public static void OnReceiveUse(int playerId)
    {
        string s = "" + GameManager.Singleton.players[playerId].name + "发动了技能腹黑";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }

}
