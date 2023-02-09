using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role_XiaoJiu : RoleBase
{
    public override string name
    {
        get { return "Ð¡¾Å"; }
    }
    public override bool isWoman => false;

    public override string spritName
    {
        get { return "XiaoJiu"; }
    }
    //public override bool isBack
    //{
    //    get { return false; }
    //}

    public Role_XiaoJiu(int id)
    {
        playerId = id;
        isBack = true;
        skills = new List<SkillBase>() { new UserSkill_GuangFaBao(id) };
    }
}
