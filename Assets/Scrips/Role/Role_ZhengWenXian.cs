using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_ZhengWenXian : RoleBase
{
    public DirectionEnum direction;

    public override string name
    {
        get { return "鄭文先"; }
    }

    public override string spritName
    {
        get { return "ZhengWenXian"; }
    }

    public override bool isBack
    {
        get;
        set;
    }

    public Role_ZhengWenXian(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_TouTian(id), new UserSkill_HuanRi(id) };
    }
}
