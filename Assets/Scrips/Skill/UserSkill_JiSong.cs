using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 鬼脚【急送】：争夺阶段限一次，你可以弃置两张手牌，或从你的情报区弃置一张非黑色情报，然后将待收情报移至一名角色面前。
public class UserSkill_JiSong : SkillBase
{
    public UserSkill_JiSong(int id)
    {
        playerId = id;
    }

    private const int needDropCount = 2;
    private int selectMessageId = 0;
    private int selectPlayerId;
    private List<int> selectCardIds = new List<int>();
    private int useCount = 0;

    public override string name => "急送";

    public override bool canUse { get {
            if(useCount > 0)
            {
                return false;
            }
            if (GameManager.Singleton.CurWaitingPlayerId != GameManager.SelfPlayerId)
            {
                return false;
            }
            if (GameManager.Singleton.curPhase != PhaseEnum.Fight_Phase)
            {
                return false;
            }
            bool haveMessage = false;
            foreach(var message in GameManager.Singleton.players[GameManager.SelfPlayerId].messages)
            {
                if(!message.color.Contains(CardColorEnum.Black))
                {
                    haveMessage = true;
                    break;
                }
            }
            if (!haveMessage && GameManager.Singleton.cardsHand.Count < needDropCount)
            {
                return false;
            }
            return true;
        } }

    public override string Des => "急送：争夺阶段限一次，你可以弃置两张手牌，或从你的情报区弃置一张非黑色情报，然后将待收情报移至一名角色面前。\n";

    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        GameManager.Singleton.gameUI.ShowPhase("发动技能急送, 你可以弃置两张手牌，或从你的情报区弃置一张非黑色情报，然后将待收情报移至一名角色面前。");
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
    }
    public override void OnPlayerSelect(int PlayerId)
    {
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        if (selectPlayerId == PlayerId)
        {
            selectPlayerId = -1;
        }
        else if (PlayerId > -1)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {
        if(playerId!= GameManager.SelfPlayerId)
        {
            GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
            return;
        }
        base.OnMessageSelect(playerId, cardId);
        selectMessageId = cardId;

        if (selectCardIds.Count > 0)
        {
            foreach (var id in selectCardIds)
            {
                if (GameManager.Singleton.gameUI.Cards.ContainsKey(id))
                {
                    GameManager.Singleton.gameUI.Cards[id].OnSelect(false);
                }
            }
            selectCardIds.Clear();
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if (selectCardIds.Contains(cardId))
        {
            GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
            selectCardIds.Remove(cardId);
        }
        else
        {
            if (selectCardIds.Count < needDropCount)
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
                selectCardIds.Add(cardId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("只需弃置" + needDropCount + "张手牌");
            }
        }
        selectMessageId = 0;
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
    }

    public override void OnMessageInfoClose()
    {
        base.OnMessageInfoClose();
    }

    public override void Use()
    {
        if(selectPlayerId > -1)
        {
            if(selectMessageId > 0 && !GameManager.Singleton.players[GameManager.SelfPlayerId].GetMessage(selectMessageId).color.Contains(CardColorEnum.Black))
            {
                ProtoHelper.SendSkill_JiSong(selectPlayerId, new List<int>(), selectMessageId, GameManager.Singleton.seqId);
            }
            else if(selectCardIds.Count == 2)
            {
                ProtoHelper.SendSkill_JiSong(selectPlayerId, selectCardIds, 0, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择弃置一张非黑情报或两张手牌");
            }
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择要将情报移至的角色");
        }
    }
    public override void Cancel()
    {
        base.Cancel();
        selectMessageId = 0;
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        if (selectCardIds.Count > 0)
        {
            foreach (var cardId in selectCardIds)
            {
                if (GameManager.Singleton.gameUI.Cards.ContainsKey(cardId))
                {
                    GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
                }
            }
            selectCardIds.Clear();
        }
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.HidePlayerMessageInfo();
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        useCount += 1;
        Cancel();
    }
    public override void OnTurnEnd()
    {
        useCount = 0;
        Cancel();
    }

    public static void OnReceiveUse(int playerId, int targetId, CardFS messageCard)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        string messageInfo = "";
        if (messageCard != null)
        {
            GameManager.Singleton.players[playerId].RemoveMessage(messageCard.id);
            GameManager.Singleton.gameUI.Players[playerId].RefreshMessage();
            string cardInfo = messageCard.GetCardInfo();
            messageInfo = "弃置了一张情报" + cardInfo;
        }


        string s = string.Format("{0}发动了急送{1},将情报移至{2}面前", GameManager.Singleton.players[playerId].name, messageInfo, GameManager.Singleton.players[targetId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }
}
