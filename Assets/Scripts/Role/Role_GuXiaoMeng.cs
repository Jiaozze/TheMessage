using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_GuXiaoMeng : RoleBase
{
    public override string name
    {
        get { return "顾小梦"; }
    }
    public override string spritName
    {
        get { return "GuXiaoMeng"; }
    }

    public override bool isWoman => true;
    public Role_GuXiaoMeng(int id)
    {
        isBack = true;
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JiZhi(id), new UserSkill_ChengZhi(id), new UserSkill_WeiSheng(id)   };
    }
}
