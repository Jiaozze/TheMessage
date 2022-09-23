using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_ShangYu : RoleBase
{
    public override string name
    {
        get { return "商玉"; }
    }
    public override string spritName
    {
        get { return "ShangYu"; }
    }

    public override bool isWoman => true;
    public Role_ShangYu(int id)
    {
        isBack = true;
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JieDaoShaRen(id) };
    }
}
