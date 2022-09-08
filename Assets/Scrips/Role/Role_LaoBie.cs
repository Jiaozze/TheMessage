using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_LaoBie : RoleBase
{
    public DirectionEnum direction;

    public override string name
    {
        get { return "老鳖"; }
    }

    public override string spritName
    {
        get { return "LaoBie"; }
    }

    public override bool isBack
    {
        get { return false; }
    }

    public Role_LaoBie(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_LianLuo(id), new UserSkill_MingEr(id) };
    }
}
