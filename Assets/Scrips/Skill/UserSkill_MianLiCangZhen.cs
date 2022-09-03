using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�㴫�����鱨�����պ󣬿��Խ�һ�ź�ɫ������������ߵ��鱨����Ȼ����һ����
public class UserSkill_MianLiCangZhen : SkillBase
{
    public override string name { get { return "�������"; } }
    public override bool canUse { get { return false; } }

    private int selectCardId;
    public UserSkill_MianLiCangZhen(int id)
    {
        playerId = id;
    }
    public override bool CheckTriger()
    {
        //GameManager.Singleton.gameUI.ShowPhase("�Ƿ񷢶�����-�������");
        if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
        {
            return false;
        }
        if (GameManager.Singleton.CurTurnPlayerId != GameManager.SelfPlayerId)
        {
            return false;
        }
        if (GameManager.Singleton.curPhase != PhaseEnum.Receive_Phase)
        {
            return false;
        }

        PrepareUse();
        return true;
    }

    public override void PrepareUse()
    {
        if (GameManager.Singleton.cardsHand.Count == 0)
        {
            Cancel();
            return;
        }

        if (GameManager.Singleton.selectSkill == this)
        {
            return;
        }
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("���Է������ܽ�һ�ź�ɫ������������ߵ��鱨������һ����");
    }
    public override void Use()
    {
        if (selectCardId > 0 && GameManager.Singleton.cardsHand[selectCardId].color.Contains(CardColorEnum.Black))
        {
            ProtoHelper.SendSkill_MianLiCangZhen(selectCardId, GameManager.Singleton.CurMessagePlayerId, GameManager.Singleton.seqId);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("��ѡ��һ�ź�ɫ����");
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if (!GameManager.Singleton.cardsHand[cardId].color.Contains(CardColorEnum.Black))
        {
            GameManager.Singleton.gameUI.ShowInfo("��ѡ��һ�ź�ɫ����");
            return;
        }

        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
        }

        if (selectCardId == cardId)
        {
            selectCardId = 0;
        }
        else
        {
            selectCardId = cardId;
            GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
        }

    }

    public override void OnPlayerSelect(int PlayerId)
    {

    }

    public override void OnMessageSelect(int playerId, int cardId)
    {

    }

    public override void Cancel()
    {
        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            selectCardId = 0;
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        ProtoHelper.SendEndReceive(GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        if (selectCardId > 0)
        {
            GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            selectCardId = 0;
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUse(int playerId, int targetPlayerId, CardFS card)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            GameManager.Singleton.cardsHand.Remove(card.id);
            GameManager.Singleton.gameUI.Cards.Remove(card.id);
        }
        GameManager.Singleton.players[playerId].cardCount = GameManager.Singleton.players[playerId].cardCount - 1;
        GameManager.Singleton.players[targetPlayerId].AddMessage(card);

        GameManager.Singleton.gameUI.ShowAddMessage(targetPlayerId, card, false);
        GameManager.Singleton.gameUI.Players[targetPlayerId].RefreshMessage();

        string s = "" + playerId + "�����ʹ���˼����������";
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
