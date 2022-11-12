using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_HuangJiRen : RoleBase
{
    public override string name
    {
        get { return "»Æ¼ÃÈÊ"; }
    }
    public override bool isWoman => false;

    public override string spritName
    {
        get { return "HuangJiRen"; }
    }
    //public override bool isBack
    //{
    //    get { return false; }
    //}

    public Role_HuangJiRen(int id)
    {
        playerId = id;
        isBack = true;
        skills = new List<SkillBase>() { new UserSkill_DuiZhengXiaYao(id) };
    }
}
