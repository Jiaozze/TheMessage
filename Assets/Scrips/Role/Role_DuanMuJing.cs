using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_DuanMuJing:RoleBase
{
    public override string name
    {
        get { return "¶ËÄ¾¾²"; }
    }

    public override bool isBack
    {
        get { return false; }
    }

    public Role_DuanMuJing(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new Skill_XinSiChao(id) };
    }
}
