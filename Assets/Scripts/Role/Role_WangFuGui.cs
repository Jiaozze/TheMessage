using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_WangFuGui : RoleBase
{
    public override string name
    {
        get { return "王富贵"; }
    }
    public override string spritName
    {
        get { return "WangFuGui"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_WangFuGui(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JiangHuLing(id)  };
    }
}
