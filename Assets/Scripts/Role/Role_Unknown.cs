using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_Unknown : RoleBase
{
    public override string name
    {
        get { return "δ֪��ɫ"; }
    }

    public override bool isBack
    {
        get { return true; }
    }

    public override string spritName
    {
        get
        {
            return "Unknown";
        }
    }

    public Role_Unknown(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { };
    }
}
