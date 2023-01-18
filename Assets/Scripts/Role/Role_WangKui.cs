using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_WangKui : RoleBase
{
    public override string name
    {
        get { return "王魁"; }
    }
    public override string spritName
    {
        get { return "WangKui"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_WangKui(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_YiYaHuanYa(id)  };
    }
}
