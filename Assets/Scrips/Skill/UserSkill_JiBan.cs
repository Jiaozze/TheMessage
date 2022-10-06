using System.Collections.Generic;

// SP顾小梦【羁绊】A：出牌阶段限一次，可以摸两张牌。
// SP顾小梦【羁绊】B：然后将至少一张手牌交给另一名角色。
public class UserSkill_JiBan : SkillBase
{
    public override string name { get { return "羁绊"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase && !isWaitGive;
        }
    }

    public override string Des => "羁绊：出牌阶段限一次，你可以摸两张牌,然后将至少一张手牌交给另一名角色。\n";

    private int usedCount = 0;

    private int selectPlayerId = -1;
    private List<int> selectCardIds = new List<int>();

    private bool isWaitGive = false;

    //private int needGiveCount = 0;

    public UserSkill_JiBan(int id)
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
            GameManager.Singleton.IsUsingSkill = true;
            GameManager.Singleton.selectSkill = this;
            GameManager.Singleton.gameUI.ShowPhase("是否发动技能羁绊");
        }
    }

    public override void Use()
    {
        if (isWaitGive)
        {
            if (selectCardIds.Count > 0 && selectPlayerId > 0)
            {
                ProtoHelper.SendSkill_JiBanB(selectCardIds, selectPlayerId, GameManager.Singleton.seqId);
            }
            else if (selectPlayerId < 1)
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择要给牌的目标玩家");
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("需要给出至少一张手牌");
            }
        }
        else
        {
            ProtoHelper.SendSkill_JiBanA(GameManager.Singleton.seqId);
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if (isWaitGive)
        {
            if (selectCardIds.Contains(cardId))
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
                selectCardIds.Remove(cardId);
            }
            else
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(true);
                selectCardIds.Add(cardId);
            }
        }
    }

    public override void OnPlayerSelect(int PlayerId)
    {
        if(!isWaitGive)
        {
            return;
        }

        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        if (selectPlayerId == PlayerId)
        {
            selectPlayerId = -1;
        }
        else if (PlayerId > 0)
        {
            selectPlayerId = PlayerId;
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(true);
        }
    }

    public override void OnMessageSelect(int playerId, int cardId)
    {

    }

    public override void OnTurnEnd()
    {
        usedCount = 0;
        isWaitGive = false;
        if (selectCardIds.Count > 0)
        {
            foreach (var cardId in selectCardIds)
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
            }
            selectCardIds.Clear();
        }
    }

    public override void Cancel()
    {
        if (isWaitGive)
        {
            GameManager.Singleton.gameUI.ShowInfo("需要给出至少一张手牌");
            return;
        }
        if (selectCardIds.Count > 0)
        {
            foreach (var cardId in selectCardIds)
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
            }
            selectCardIds.Clear();
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;

        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        usedCount = usedCount + 1;
        isWaitGive = false;
        if (selectCardIds.Count > 0)
        {
            foreach (var cardId in selectCardIds)
            {
                GameManager.Singleton.gameUI.Cards[cardId].OnSelect(false);
            }
            selectCardIds.Clear();
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();

    }

    public static void OnReceiveUseA(int playerId, int waitSeconds, uint seq)
    {
        GameManager.Singleton.seqId = seq;

        if (waitSeconds > 0)
        {
            GameManager.Singleton.OnWait(playerId, waitSeconds);
        }

        if (playerId == GameManager.SelfPlayerId)
        {
            if(GameManager.Singleton.selectSkill != null)
            {
                UserSkill_JiBan skill_JiBan = GameManager.Singleton.selectSkill as UserSkill_JiBan;
                skill_JiBan.isWaitGive = true;
                GameManager.Singleton.gameUI.ShowPhase("选择至少一张手牌交给一名其他角色");
            }
        }

        string s = string.Format("{0}使用了技能羁绊", GameManager.Singleton.players[playerId].name);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

    public static void OnReceiveUseB(int playerId, int targetId, List<CardFS> cards, int unknowCount)
    {
        int cardCount = cards.Count > 0 ? cards.Count : unknowCount;
        GameManager.Singleton.players[playerId].cardCount -= cardCount;
        GameManager.Singleton.players[targetId].cardCount += cardCount;
        GameManager.Singleton.gameUI.Players[playerId].RefreshCardCount();
        GameManager.Singleton.gameUI.Players[targetId].RefreshCardCount();

        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
            foreach (var card in cards)
            {
                GameManager.Singleton.cardsHand.Remove(card.id);
                //var cardUI = GameManager.Singleton.gameUI.Cards[card.id];
                //GameManager.Singleton.gameUI.Cards.Remove(card.id);
                //GameObject.Destroy(cardUI.gameObject);
            }
        }
        else if (targetId == GameManager.SelfPlayerId)
        {
            foreach (var card in cards)
            {
                GameManager.Singleton.cardsHand[card.id] = card;
            }
            GameManager.Singleton.gameUI.DrawCards(cards);
        }

        string cardStr = "";
        if (cards.Count > 0)
        {
            foreach (var card in cards)
            {
                cardStr += LanguageUtils.GetCardName(card.cardName);
            }
            GameManager.Singleton.gameUI.ShowCardsMove(playerId, targetId, cards);

        }
        else
        {
            List<CardFS> unknownCards = new List<CardFS>();
            for (int i = 0; i < unknowCount; i++)
            {
                unknownCards.Add(new CardFS(null));
            }
            GameManager.Singleton.gameUI.ShowCardsMove(playerId, targetId, unknownCards);
        }

        string s = string.Format("{0}交给{1}{2}张牌{3}", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[targetId].name, cardCount, cardStr);
        GameManager.Singleton.gameUI.ShowInfo(s);
        GameManager.Singleton.gameUI.AddMsg(s);
    }

}
