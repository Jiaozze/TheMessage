using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public bool dir;
    public GameUI gameUI;

    public Dictionary<int, PlayerBase> players = new Dictionary<int, PlayerBase>();
    public Dictionary<int, CardFS> cards = new Dictionary<int, CardFS>(); //<id, card>

    public int topColor; // 黑色牌声明的颜色
    public int topCardCount;
    public int wantColor;

    public int onTurnPlayerId = -1;
    private List<int> selectCardIds = new List<int>();
    private int selectCard = -1;
    //public 
    private static GameManager gameManager;
    private int DeckNum;

    public static GameManager Singleton
    {
        get
        {
            if (gameManager == null)
            {
                gameManager = new GameManager();
                gameManager.Init();
            }
            return gameManager;
        }
    }
    private GameManager()
    {
    }

    public void Init()
    {
        GameObject windowGo = GameObject.Find("GameUI");
        if (gameUI == null && windowGo != null)
        {
            gameUI = windowGo.GetComponent<GameUI>();
            if (gameUI == null)
            {
                gameUI = windowGo.AddComponent<GameUI>();
            }
        }
        else
        {
            //TODO
        }
    }

    public void InitPlayers(int num)
    {
        players.Clear();
        for (int i = 0; i < num; i++)
        {
            PlayerBase player = new PlayerBase();
            players.Add(i, player);
        }
    }
    private void InitCards(List<CardFS> cards)
    {
        foreach (var card in cards)
        {
            card.isHand = true;
            this.cards.Add(card.id, card);
        }
    }
    public void SetDeckNum(int num)
    {
        DeckNum = num;
        gameUI.SetDeckNum(num);
    }

    public void SetCardSelect(int id, bool select)
    {
        if (selectCard != -1 && id != selectCard)
        {
            selectCard = -1;
        }

        if (select)
        {
            selectCard = id;
        }
    }

    public bool IsCardAvailable(int color, int num)
    {
        return true;
    }

    public Color GetColorById(int colorId)
    {
        Color color = Color.black;
        switch (colorId)
        {
            case 0:
                color = Color.black;
                break;
            case 1:
                color = Color.red;
                break;
            case 2:
                color = Color.green;
                break;
            case 3:
                color = Color.yellow;
                break;
            case 4:
                color = Color.blue;
                break;

            default:
                color = Color.gray;

                break;
        }
        return color;
    }

    public string GetCardInfo(int colorId, int num)
    {
        Color color1 = GetColorById(colorId);

        string nunStr = "";

        if (num < 10)
        {
            nunStr = "" + num;
        }
        else
        {
            switch (num)
            {
                case 10:
                    nunStr = "跳过";
                    break;
                case 11:
                    nunStr = "反向";
                    break;
                case 12:
                    nunStr = "+2";
                    break;
                case 13:
                    nunStr = "变色";
                    break;
                case 14:
                    nunStr = "+4";
                    break;
                default:
                    break;
            }
        }

            string colorStr = "<color=#" + ColorUtility.ToHtmlStringRGBA(color1) + ">" + num + "</color>";
        return colorStr;
    }

    public string GetCardsInfo(List<UICard> cards)
    {
        string ret = "";
        foreach (var card in cards)
        {
            //ret += GetCardInfo(card.color, card.num);
            //ret += ",";
        }
        return ret;
    }

    #region 服务器消息处理


    public void OnReceiveGameStart(int player_num, List<CardFS> cards)
    {
        InitPlayers(player_num);
        gameUI.InitPlayers(player_num);

        InitCards(cards);
        gameUI.InitCards(cards.Count);
        //gameUI.AddMsg(string.Format("你摸了{0}张牌, {1}", cards.Count, GetCardsInfo(cards)));

    }

    public void OnPlayerDrawCards(List<CardFS> cards)
    {
        string cardInfo = "";
        //foreach(var card in cards)
        //{
        //    Player.Singleton.DrawCards((int)card.CardId, (int)card.Color, (int)card.Num);
        //    cardInfo += GetCardInfo((int)card.Color, (int)card.Num) + ",";

        //}
        //DeckNum = DeckNum - 1;
        //SetDeckNum(DeckNum);
        gameUI.DrawCards(cards);
        gameUI.AddMsg(string.Format("你摸了{0}张牌; {1}", cards.Count, cardInfo));

    }

    public void OnOtherDrawCards(int id, int num)
    {
        DeckNum = DeckNum - num;
        SetDeckNum(DeckNum);
        gameUI.AddMsg(string.Format("{0}号玩家摸了{1}张牌", id, num));
    }

    public void OnTurnTo(int id, bool dir)
    {
        gameUI.SetTurn();
        gameUI.AddMsg(string.Format("{0}号玩家回合开始", id));
    }

    public void OnDeckNumTo(int num)
    {
        DeckNum = num;
        SetDeckNum(num);
    }

    public void OnDisCard(int playerId, int id, int color, int num, int colorEx)
    {
        string log = "";
        if (playerId > 999)
        {
            log = "从牌堆顶翻出一张牌：{0}";
        }
        else if (playerId == 0)
        {
            log = "你打出了一张牌：{0}";
        }
        else
        {
            log = "" + playerId + "号玩家打出一张牌：{0}";
        }
        gameUI.AddMsg(string.Format(log, GetCardInfo(color, num)));
    }
    #endregion


    #region
    public void Discard()
    {
        if (selectCard == -1)
        {
            return;
        }
        CardFS card = cards[selectCard];

        if (IsCardAvailable(card.id, card.id))
        {
            //if (card.color == 0 && wantColor == 0)
            //{
            //    return;
            //    //ProtoHelper.SendDiscardMessage(card.id, UnityEngine.Random.Range(1, 5));
            //}
            //else
            //{
            //    ProtoHelper.SendDiscardMessage(card.id, wantColor);
            //    wantColor = 0;
            //}
        }
        selectCard = -1;
        //TODO
        //OnDisCard(0, card.id, card.color, card.num);
    }

    public void DrawCard()
    {
        ProtoHelper.SendDiscardMessage(0, 0);

    }

    #endregion
}
