using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoleBase
{
    public abstract string name { get; }
    public abstract string spritName { get; } //���ؽ�ɫͼƬ�õ�sprite����
    public virtual bool isWoman { get; }
    public virtual bool isBack
    {
        get;
        set;
    }

    public int playerId;
    public List<SkillBase> skills;
}
