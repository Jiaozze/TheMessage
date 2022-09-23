using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_PeiLing:RoleBase
{
    public override string name
    {
        get { return "ÅáÁá"; }
    }
    public override bool isWoman => true;
    public override string spritName
    {
        get { return "PeiLing"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_PeiLing(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JiaoJi(id) };
    }
}
