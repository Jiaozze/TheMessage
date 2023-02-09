using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_DuanMuJing:RoleBase
{
    public override string name
    {
        get { return "¶ËÄ¾¾²"; }
    }
    public override bool isWoman => true;
    public override string spritName
    {
        get { return "DuanMuJing"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_DuanMuJing(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_XinSiChao(id) };
    }
}
