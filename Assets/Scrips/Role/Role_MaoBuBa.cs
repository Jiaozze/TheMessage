using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_MaoBuBa : RoleBase
{
    public override string name
    {
        get { return "Ã«²»°Î"; }
    }
    public override string spritName
    {
        get { return "MaoBuBa"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_MaoBuBa(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_QiHuoKeJu(id) };
    }
}
