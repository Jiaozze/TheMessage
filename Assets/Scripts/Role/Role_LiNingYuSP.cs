using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_LiNingYuSP : RoleBase
{
    public override string name
    {
        get { return "SP李宁玉"; }
    }
    public override string spritName
    {
        get { return "LiNingYuSP"; }
    }

    public override bool isWoman => true;
    public Role_LiNingYuSP(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_YingBian(id), new UserSkill_YouDao(id)   };
    }
}
