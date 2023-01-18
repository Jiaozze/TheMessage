using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【视死】：你接收黑色情报后，摸两张牌
public class UserSkill_ShiSi : SkillBase
{
    public UserSkill_ShiSi(int id)
    {
        playerId = id;
    }

    public override string name => "视死";

    public override bool canUse => false;

    public override string Des => "视死：你接收黑色情报后，摸两张牌\n";

    public static void OnReceiveUse(int playerId)
    {
        string s = "" + GameManager.Singleton.players[playerId].name + "发动了视死";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
