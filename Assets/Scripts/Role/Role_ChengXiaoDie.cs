using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_ChengXiaoDie : RoleBase
{
    public override string name
    {
        get { return "³ÌÐ¡µû"; }
    }
    public override bool isWoman => false;

    public override string spritName
    {
        get { return "ChengXiaoDie"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_ChengXiaoDie(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_ZhiYin(id), new UserSkill_JingMeng(id) };
    }
}
