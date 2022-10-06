using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_GuXiaoMengSP : RoleBase
{
    public override string name
    {
        get { return "SP顾小梦"; }
    }
    public override string spritName
    {
        get { return "GuXiaoMengSP"; }
    }

    public override bool isWoman => true;
    public Role_GuXiaoMengSP(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JiBan(id)   };
    }
}
