using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����硿���㴫���鱨ʱ�����Խ��鱨���ϵļ�ͷ�������ⷽ��
public class UserSkill_LianLuo : SkillBase
{
    public UserSkill_LianLuo(int id)
    {
        playerId = id;
    }

    public override string name => "����";

    public override bool canUse => false;

}
