using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_BaiXiaoNian : RoleBase
{
    public override string name
    {
        get { return "白小年"; }
    }
    public override string spritName
    {
        get { return "BaiXiaoNian"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_BaiXiaoNian(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_ZhuanJiao(id)  };
    }
}
