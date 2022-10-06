using System.Collections.Generic;

// 白沧浪【博爱】A：出牌阶段限一次，你可以摸一张牌。
// 白沧浪【博爱】B：然后可以将一张手牌交给另一名角色，若交给了女性角色，则你再摸一张牌。
public class UserSkill_BoAi : SkillBase
{
    public override string name { get { return "博爱"; } }
    public override bool canUse
    {
        get
        {
            return usedCount < 1 && playerId == GameManager.SelfPlayerId && GameManager.Singleton.CurWaitingPlayerId == GameManager.SelfPlayerId && GameManager.Singleton.curPhase == PhaseEnum.Main_Phase && !isWaitGive;
        }
    }

    public override string Des => "博爱：出牌阶段限一次，你可以摸一张牌。然后可以将一张手牌交给另一名角色，若交给了女性角色，则你再摸一张牌。\n";

    private int usedCount = 0;

    private int selectPlayerId = -1;
    private int selectCardId = 0;
    private bool isWaitGive = false;

    //private int needGiveCount = 0;

    public UserSkill_BoAi(int id)
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
            GameManager.Singleton.gameUI.ShowPhase("是否发动技能博爱");
        }
    }

    public override void Use()
    {
        if (isWaitGive)
        {
            if (selectPlayerId > 0 && selectCardId > 0)
            {
                ProtoHelper.SendSkill_BoAiB(selectCardId, selectPlayerId, GameManager.Singleton.seqId);
            }
            else
            {
                GameManager.Singleton.gameUI.ShowInfo("请选择正确的卡牌和角色");
            }
        }
        else
        {
            ProtoHelper.SendSkill_BoAiA(GameManager.Singleton.seqId);
        }
    }

    public override void OnCardSelect(int cardId)
    {
        if (isWaitGive)
        {
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
        Cancel();
    }

    public override void Cancel()
    {
        if(GameManager.Singleton.IsUsingSkill && isWaitGive)
        {
            ProtoHelper.SendSkill_BoAiB(0, 0, GameManager.Singleton.seqId);
        }
        if (selectCardId > 0)
        {
            if (GameManager.Singleton.gameUI.Cards.ContainsKey(selectCardId))
            {
                GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            }

            selectCardId = 0;
        }
        if (selectPlayerId > -1)
        {
            GameManager.Singleton.gameUI.Players[selectPlayerId].OnSelect(false);
        }
        selectPlayerId = -1;
        isWaitGive = false;

        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public override void OnUse()
    {
        //usedCount = usedCount + 1;
        isWaitGive = false;
        if (selectCardId > 0)
        {
            if (GameManager.Singleton.gameUI.Cards.ContainsKey(selectCardId))
            {
                GameManager.Singleton.gameUI.Cards[selectCardId].OnSelect(false);
            }

            selectCardId = 0;
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
                UserSkill_BoAi skill_BoAi = GameManager.Singleton.selectSkill as UserSkill_BoAi;
                skill_BoAi.isWaitGive = true;
                skill_BoAi.usedCount += 1;
                GameManager.Singleton.gameUI.ShowPhase("可以将一张手牌交给另一名角色，若交给了女性角色，则你再摸一张牌");
            }
        }

        string s = string.Format("{0}使用了技能博爱", GameManager.Singleton.players[playerId].name);
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
