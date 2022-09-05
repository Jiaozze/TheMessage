using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_FeiYuanLongChuan : RoleBase
{
    public int skillUseCount = 0;
    public override string name
    {
        get { return "·ÊÔ­Áú´¨"; }
    }
    public override bool isWoman => true;
    public override string spritName
    {
        get { return "FeiYuanLongChuan"; }
    }
    public override bool isBack
    {
        get { return false; }
    }

    public Role_FeiYuanLongChuan(int id)
    {
        playerId = id;
        skills = new List<SkillBase>() { new UserSkill_GuiZha_LiYou(id), new UserSkill_GuiZha_WeiBi(id)  };
    }
}
