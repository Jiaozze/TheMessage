using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//【联络】：你传出情报时，可以将情报牌上的箭头视作任意方向
public class UserSkill_LianLuo : SkillBase
{
    public UserSkill_LianLuo(int id)
    {
        playerId = id;
    }

    public override string name => "联络";

    public override bool canUse => false;

    public override string Des => "联络：你传出情报时，可以将情报牌上的箭头视作任意方向\n";
}
