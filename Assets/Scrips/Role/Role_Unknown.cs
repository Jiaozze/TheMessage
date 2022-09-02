using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_Unknown : RoleBase
{
    public override string name
    {
        get { return "δ֪"; }
    }

    public override bool isBack
    {
        get { return true; }
    }

    public Role_Unknown(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { };
    }
}
