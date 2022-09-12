using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_LaoHan : RoleBase
{
    public override string name
    {
        get { return "老汉"; }
    }
    public override string spritName
    {
        get { return "LaoHan"; }
    }
    public override bool isBack
    {
        get { return false; }
    }
    public override bool isWoman => true;
    public Role_LaoHan(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_ShiSi(id), new UserSkill_RuGui(id)  };
    }
}
