using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_BaiCangLang : RoleBase
{
    public override string name
    {
        get { return "白沧浪"; }
    }
    public override string spritName
    {
        get { return "BaiCangLang"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_BaiCangLang(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_BoAi(id)  };
    }
}
