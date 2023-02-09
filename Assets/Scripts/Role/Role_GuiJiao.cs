using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_GuiJiao : RoleBase
{
    public override string name
    {
        get { return "鬼脚"; }
    }
    public override string spritName
    {
        get { return "GuiJiao"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_GuiJiao(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_JiSong(id)  };
    }
}
