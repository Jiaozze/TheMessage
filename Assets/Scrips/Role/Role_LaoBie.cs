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
        get { return true; }
    }

    public Role_LaoBie(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { };
    }
}
