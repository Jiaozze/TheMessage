using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_WuZhiGuo : RoleBase
{
    public override string name
    {
        get { return "吴志国"; }
    }
    public override string spritName
    {
        get { return "WuZhiGuo"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_WuZhiGuo(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JianRen(id)  };
    }
}
