using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_ShaoXiu:RoleBase
{
    public override string name
    {
        get { return "…€–„"; }
    }

    public override bool isBack
    {
        get { return false; }
    }

    public Role_ShaoXiu(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_MianLiCangZhen(id) };
    }
}
