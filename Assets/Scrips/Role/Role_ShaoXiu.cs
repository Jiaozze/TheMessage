using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_ShaoXiu:RoleBase
{
    public override string name
    {
        get { return "ÉÛÐã"; }
    }
    public override bool isWoman => true;

    public override string spritName
    {
        get { return "ShaoXiu"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_ShaoXiu(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_MianLiCangZhen(id) };
    }
}
