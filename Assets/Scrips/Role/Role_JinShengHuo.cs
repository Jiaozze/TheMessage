using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_JinShengHuo : RoleBase
{
    public override string name
    {
        get { return "½ðÉú»ð"; }
    }
    public override string spritName
    {
        get { return "JinShengHuo"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_JinShengHuo(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JinShen(id) };
    }
}
