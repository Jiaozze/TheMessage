using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����������㴫���ĺ�ɫ����ɫ�鱨�����պ���ͽ����߸���һ����
public class UserSkill_MingEr : SkillBase
{
    public UserSkill_MingEr(int id)
    {
        playerId = id;
    }

    public override string name => "����";

    public override bool canUse => false;

    public static void OnReceiveUse(int playerId)
    {
        string s = "" + playerId + "����ҷ����˼�������";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);

    }

}
