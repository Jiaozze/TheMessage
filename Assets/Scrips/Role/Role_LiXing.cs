using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_LiXing : RoleBase
{
    public override string name
    {
        get { return "ÀîÐÑ"; }
    }
    public override bool isWoman => false;

    public override string spritName
    {
        get { return "LiXing"; }
    }
    //public override bool isBack
    //{
    //    get { return false; }
    //}

    public Role_LiXing(int id)
    {
        playerId = id;
        isBack = true;
        skills = new List<SkillBase>() { new UserSkill_SouJi(id) };
    }
}
