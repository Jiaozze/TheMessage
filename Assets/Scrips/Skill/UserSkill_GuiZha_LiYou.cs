using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ԭ��������թ�������ƽ׶���һ�Σ������ָ��һ����ɫ��Ȼ����Ϊ�����ʹ����һ�š����ơ������ա���
public class UserSkill_GuiZha_LiYou : SkillBase
{
    public override string name { get { return "��թ-����"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase;
        }
    }

    private int usedCount = 0;

    private int selectPlayerId = -1;

    public UserSkill_GuiZha_LiYou(int id)
    {
        playerId = id;
    }
    public override void PrepareUse()
    {
        if(!canUse)
        {
            return;
        }
        base.PrepareUse();
        if(GameManager.Singleton.selectSkill == this)
        {
            Cancel();
        }
        else
        {
            if(GameManager.Singleton.selectSkill!= null)
            {
                GameManager.Singleton.selectSkill.Cancel();
            }
            GameManager.Singleton.IsUsingSkill = true;
            GameManager.Singleton.selectSkill = this;
            GameManager.Singleton.gameUI.ShowPhase("����ʹ�ü��ܡ���թ������ѡ��һ�������Ϊ����Ŀ��");
        }
    }

    public override void Use()
    {
        if(selectPlayerId > -1)
        {
            ProtoHelper.SendSkill_GuiZha(selectPlayerId, CardNameEnum.LiYou, CardNameEnum.ChengQing, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("��ѡ��һ�������Ϊ����Ŀ��");
        }
    }

    public override void OnCardSelect(int cardId)
    {

    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        if(selectPlayerId == PlayerId)
        {
            selectPlayerId = -1;
        }
        else
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {

    }

    public override void Cancel()
    {
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
            selectPlayerId = -1; 
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        usedCount = usedCount + 1;
        Cancel();
    }

    public static void OnReceiveUse(int playerId, int tatgetId, CardNameEnum cardName)
    {
        if(playerId == GameManager.SelfPlayerId)
        {
            if(GameManager.Singleton.selectSkill != null)
            {
                foreach(var skill in GameManager.Singleton.players[playerId].role.skills)
                {
                    skill.OnUse();
                }
            }
        }
        string s = string.Format("{0}����Ҷ�{1}�����ʹ���˼��ܹ�թ����Ϊʹ����һ��{2}", playerId, tatgetId, LanguageUtils.GetCardName(cardName));
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
