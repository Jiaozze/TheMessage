using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_LiNingYu : RoleBase
{
    public override string name
    {
        get { return "李宁玉"; }
    }
    public override string spritName
    {
        get { return "LiNingYu"; }
    }

    public override bool isWoman => true;
    public Role_LiNingYu(int id)
    {
        isBack = true;
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_ChengFu(id), new UserSkill_JiuJi(id), new UserSkill_YiXin(id)   };
    }
}
