using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoleBase
{
    public abstract string name { get; }
    public abstract string spritName { get; }
    public virtual bool isWoman { get; }
    public virtual bool isBack
    {
        get { return false; }
    }

    public int playerId;
    public List<SkillBase> skills;
}
