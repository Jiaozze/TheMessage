using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_BaiFeiFei : RoleBase
{
    public override string name
    {
        get { return "白菲菲"; }
    }
    public override string spritName
    {
        get { return "BaiFeiFei"; }
    }
    public override bool isBack
    {
        get { return false; }
    }
    public override bool isWoman => true;
    public Role_BaiFeiFei(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_LianMin(id), new UserSkill_FuHei(id)  };
    }
}
