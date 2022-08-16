using Google.Protobuf;
//using Protos;
using System.Collections.Generic;
using UnityEngine;

public static class ProtoHelper
{
    public static void OnReceiveMsg(int id, byte[] contont)
    {
        //Debug.LogError(id);
        // 通知客户端：某个玩家摸了一张卡
        if (GetIdFromProtoName("add_card_toc") == id)
        {
            //Debug.LogError("add_card_toc");
            add_card_toc add_card_toc = add_card_toc.Parser.ParseFrom(contont);
            if (add_card_toc.PlayerId == 0)
            {
                List<CardFS> uno_Cards = new List<CardFS>();
                foreach (var card in add_card_toc.Cards)
                {
                    //Debug.LogError("-----add_card_toc, CardId CardDir CardType:" + card.CardId + "," + card.CardDir + "," + card.CardType);
                    CardFS cardFS = new CardFS(card);
                    uno_Cards.Add(cardFS);
                }
                GameManager.Singleton.OnReceivePlayerDrawCards(uno_Cards);
            }
            else
            {
                GameManager.Singleton.OnReceiveOtherDrawCards((int)add_card_toc.PlayerId, (int)add_card_toc.UnknownCardCount);
                //Debug.LogError("-----add_card_toc, PlayerId:" + add_card_toc.PlayerId);
            }
        }
        // 通知客户端：初始化游戏
        else if (GetIdFromProtoName("init_toc") == id)
        {
            init_toc init_Toc = init_toc.Parser.ParseFrom(contont);
            int playerCount = (int)init_Toc.PlayerCount;
            PlayerColorEnum playerColor = (PlayerColorEnum)init_Toc.Identity;
            SecretTaskEnum secretTask = (SecretTaskEnum)init_Toc.SecretTask;
            GameManager.Singleton.OnReceiveGameStart(playerCount, playerColor, secretTask);
        }
        // 通知客户端，到谁的哪个阶段了
        else if (GetIdFromProtoName("notify_phase_toc") == id)
        {
            notify_phase_toc notify_phase_toc = notify_phase_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceiveTurn((int)notify_phase_toc.CurrentPlayerId, (int)notify_phase_toc.IntelligencePlayerId, (int)notify_phase_toc.WaitingPlayerId, (PhaseEnum)notify_phase_toc.CurrentPhase, (int)notify_phase_toc.WaitingSecond, notify_phase_toc.Seq);
            Debug.Log("_______receive________notify_phase_toc " + notify_phase_toc.WaitingPlayerId + " seq:" + notify_phase_toc.Seq);
        }

        // 通知客户端，谁对谁使用了试探
        else if (GetIdFromProtoName("use_shi_tan_toc") == id)
        {
            use_shi_tan_toc use_Shi_Tan_Toc = use_shi_tan_toc.Parser.ParseFrom(contont);

            int user = (int)use_Shi_Tan_Toc.PlayerId;
            int target = (int)use_Shi_Tan_Toc.TargetPlayerId;
            int cardId = (int)use_Shi_Tan_Toc.CardId;
            GameManager.Singleton.OnRecerveUseShiTan(user, target, cardId);
            Debug.Log("_______receive________ use_shi_tan_toc " + user + "," + target);
        }

        // 向被试探者展示试探，并等待回应
        else if (GetIdFromProtoName("show_shi_tan_toc") == id)
        {
            show_shi_tan_toc show_Shi_Tan_Toc = show_shi_tan_toc.Parser.ParseFrom(contont);

            int user = (int)show_Shi_Tan_Toc.PlayerId;
            int target = (int)show_Shi_Tan_Toc.TargetPlayerId;
            CardFS card = new CardFS(show_Shi_Tan_Toc.Card);
            int time = (int)show_Shi_Tan_Toc.WaitingSecond;
            GameManager.Singleton.OnReceiveShowShiTan(user, target, card, time, show_Shi_Tan_Toc.Seq);
        }
        // 被试探者执行试探
        else if (GetIdFromProtoName("execute_shi_tan_toc") == id)
        {
            execute_shi_tan_toc execute_Shi_Tan_Toc = execute_shi_tan_toc.Parser.ParseFrom(contont);

            int playerId = (int)execute_Shi_Tan_Toc.PlayerId;
            GameManager.Singleton.OnReceiveExcuteShiTan(playerId, execute_Shi_Tan_Toc.IsDrawCard);
            Debug.Log("_______receive________ execute_shi_tan_toc");
        }

        // 通知客户端，牌堆的剩余数量
        else if (GetIdFromProtoName("sync_deck_num_toc") == id)
        {
            Debug.Log("TODO _______receive________ sync_deck_num_toc");

        }
        // 通知客户端，牌从谁的手牌被弃掉
        else if (GetIdFromProtoName("discard_card_toc") == id)
        {
            Debug.Log("TODO _______receive________ discard_card_toc");

            discard_card_toc discard_Card_Toc = discard_card_toc.Parser.ParseFrom(contont);
            int playerId = (int)discard_Card_Toc.PlayerId;
            List<CardFS> cards = new List<CardFS>();
            foreach (var card in discard_Card_Toc.Cards)
            {
                CardFS cardFS = new CardFS(card);
                cards.Add(cardFS);
            }

            GameManager.Singleton.OnReceiveDiscards(playerId, cards);
        }
        // 通知客户端使用利诱的结果
        else if (GetIdFromProtoName("use_li_you_toc") == id)
        {
            Debug.Log(" _______receive________ use_li_you_toc");
            use_li_you_toc use_Li_You_Toc = use_li_you_toc.Parser.ParseFrom(contont);

            int user = (int)use_Li_You_Toc.PlayerId;
            int target = (int)use_Li_You_Toc.TargetPlayerId;
            CardFS cardLiYou = new CardFS(use_Li_You_Toc.LiYouCard);
            CardFS cardMessage = new CardFS(use_Li_You_Toc.MessageCard);
            GameManager.Singleton.OnRecerveUseLiYou(user, target, cardLiYou, cardMessage, use_Li_You_Toc.JoinIntoHand);
        }
        // 通知客户端使用平衡的结果
        else if (GetIdFromProtoName("use_ping_heng_toc") == id)
        {
            Debug.Log(" _______receive________ use_ping_heng_toc");
            use_ping_heng_toc use_Ping_Heng_Toc = use_ping_heng_toc.Parser.ParseFrom(contont);

            int user = (int)use_Ping_Heng_Toc.PlayerId;
            int target = (int)use_Ping_Heng_Toc.TargetPlayerId;
            CardFS cardLiYou = new CardFS(use_Ping_Heng_Toc.PingHengCard);
            //CardFS cardMessage = new CardFS(use_Li_You_Toc.MessageCard);
            GameManager.Singleton.OnReceiveUsePingHeng(user, target, cardLiYou);

            List<CardFS> cards = new List<CardFS>();
            foreach (var card in use_Ping_Heng_Toc.DiscardCards)
            {
                CardFS cardFS = new CardFS(card);
                cards.Add(cardFS);
            }
            GameManager.Singleton.OnReceiveDiscards(user, cards);

            List<CardFS> targetCards = new List<CardFS>();
            foreach (var card in use_Ping_Heng_Toc.TargetDiscardCards)
            {
                CardFS cardFS = new CardFS(card);
                targetCards.Add(cardFS);
            }
            GameManager.Singleton.OnReceiveDiscards(target, targetCards);

        }
        // 通知所有人威逼的牌没有，展示所有手牌
        else if(GetIdFromProtoName("wei_bi_show_hand_card_toc") == id)
        {
            Debug.Log(" _______receive________ wei_bi_show_hand_card_toc");

            wei_bi_show_hand_card_toc wei_Bi_Show_Hand_Card_Toc = wei_bi_show_hand_card_toc.Parser.ParseFrom(contont);
            int user = (int)wei_Bi_Show_Hand_Card_Toc.PlayerId;
            int target = (int)wei_Bi_Show_Hand_Card_Toc.TargetPlayerId;
            CardFS cardUsed = new CardFS(wei_Bi_Show_Hand_Card_Toc.Card);
            List<CardFS> cards = new List<CardFS>();
            foreach (var card in wei_Bi_Show_Hand_Card_Toc.Cards)
            {
                CardFS cardFS = new CardFS(card);
                cards.Add(cardFS);
            }
            GameManager.Singleton.OnReceiveUseWeiBiShowHands(user, target, cardUsed, cards);
        }
        // 通知所有人威逼等待给牌
        else if (GetIdFromProtoName("wei_bi_wait_for_give_card_toc") == id)
        {
            Debug.Log(" _______receive________ wei_bi_wait_for_give_card_toc");

            wei_bi_wait_for_give_card_toc wei_bi_wait_for_give_card_toc = wei_bi_wait_for_give_card_toc.Parser.ParseFrom(contont);
            int user = (int)wei_bi_wait_for_give_card_toc.PlayerId;
            int target = (int)wei_bi_wait_for_give_card_toc.TargetPlayerId;
            CardFS cardUsed = new CardFS(wei_bi_wait_for_give_card_toc.Card);

            int waitTime = (int)wei_bi_wait_for_give_card_toc.WaitingSecond;
            uint seq = wei_bi_wait_for_give_card_toc.Seq;
            CardNameEnum cardWant = (CardNameEnum)wei_bi_wait_for_give_card_toc.WantType;
            GameManager.Singleton.OnReceiveUseWeiBiGiveCard(user, target, cardUsed, cardWant, waitTime, seq);
        }
        // 通知所有人威逼给牌
        else if (GetIdFromProtoName("wei_bi_give_card_toc") == id)
        {
            Debug.Log(" _______receive________ wei_bi_give_card_toc");

            wei_bi_give_card_toc wei_bi_give_card_toc = wei_bi_give_card_toc.Parser.ParseFrom(contont);
            int user = (int)wei_bi_give_card_toc.PlayerId;
            int target = (int)wei_bi_give_card_toc.TargetPlayerId;
            CardFS cardUsed = new CardFS(wei_bi_give_card_toc.Card);

            GameManager.Singleton.OnReceiveExcuteWeiBiGiveCard(user, target, cardUsed);
        }

        else
        {
            Debug.LogError("undefine proto:" + id);
        }
    }

    public static void SendEndWaiting(uint seq)
    {
        Debug.Log("____send___________________ end_Main_Phase_Tos, seq:" + seq);
        end_main_phase_tos end_Main_Phase_Tos = new end_main_phase_tos() { Seq = seq };

        byte[] proto = end_Main_Phase_Tos.ToByteArray();
        SendProto("end_main_phase_tos", proto);
    }

    public static void SendUseCardMessage_ShiTan(int cardId, int playerId, uint seq)
    {
        Debug.Log("____send___________________ use_shi_tan_tos, seq:" + seq);
        use_shi_tan_tos use_shi_tan_tos = new use_shi_tan_tos() { CardId = (uint)cardId, PlayerId = (uint)playerId, Seq = seq };

        byte[] proto = use_shi_tan_tos.ToByteArray();
        SendProto("use_shi_tan_tos", proto);
    }

    public static void SendUseCardMessage_WeiBi(int cardId, int playerId, uint seq, CardNameEnum cardWant)
    {
        Debug.Log("____send___________________ use_wei_bi_tos, seq:" + seq);

        use_wei_bi_tos use_Wei_Bi_Tos = new use_wei_bi_tos() { CardId = (uint)cardId, PlayerId = (uint)playerId, Seq = seq, WantType = (card_type)cardWant };
        byte[] proto = use_Wei_Bi_Tos.ToByteArray();
        SendProto("use_wei_bi_tos", proto);
    }
    public static void SendUseCardMessage_LiYou(int cardId, int playerId, uint seq)
    {
        Debug.Log("____send___________________ use_li_you_tos, seq:" + seq);
        use_li_you_tos use_li_you_tos = new use_li_you_tos() { CardId = (uint)cardId, PlayerId = (uint)playerId, Seq = seq };

        byte[] proto = use_li_you_tos.ToByteArray();
        SendProto("use_li_you_tos", proto);
    }

    public static void SendUseCardMessage_PingHeng(int cardId, int playerId, uint seq)
    {
        Debug.Log("____send___________________ use_ping_heng_tos, seq:" + seq);
        use_ping_heng_tos use_ping_heng_tos = new use_ping_heng_tos() { CardId = (uint)cardId, PlayerId = (uint)playerId, Seq = seq };

        byte[] proto = use_ping_heng_tos.ToByteArray();
        SendProto("use_ping_heng_tos", proto);
    }
    // 试探弃牌或者摸牌
    public static void SendDoShiTan(int cardId, uint seq)
    {
        Debug.Log("____send___________________ execute_shi_tan_tos, seq:" + seq);
        execute_shi_tan_tos execute_Shi_Tan_Tos = new execute_shi_tan_tos() { Seq = seq };
        if(cardId > 0)
        {
            execute_Shi_Tan_Tos.CardId.Add((uint)cardId);
        }

        byte[] proto = execute_Shi_Tan_Tos.ToByteArray();
        SendProto("execute_shi_tan_tos", proto);
    }
    // 威逼给牌
    public static void SendDoWeiBi(int cardId, uint seq)
    {
        Debug.Log("____send___________________ wei_bi_give_card_tos, seq:" + seq);
        wei_bi_give_card_tos wei_Bi_Give_Card_Tos = new wei_bi_give_card_tos() { CardId = (uint)cardId, Seq = seq };
        byte[] proto = wei_Bi_Give_Card_Tos.ToByteArray();
        SendProto("wei_bi_give_card_tos", proto);
    }

    private static void SendProto(string protoName, byte[] proto)
    {
        int protoId = GetIdFromProtoName(protoName);
        short len = (short)(proto.Length + 2);
        short shortId = (short)protoId;
        List<byte> vs = new List<byte>() { (byte)len, (byte)(len >> 8), (byte)shortId, (byte)(shortId >> 8), };

        foreach (var bt in proto)
        {
            vs.Add(bt);
        }
        byte[] msg = vs.ToArray();
        //Debug.Log(BitConverter.ToString(msg, 0, msg.Length));
        NetWork.Send(msg);
    }

    private static Dictionary<string, int> proto_name_id = new Dictionary<string, int>();

    public static int GetIdFromProtoName(string protoName)
    {
        if (proto_name_id.ContainsKey(protoName))
        {
            return proto_name_id[protoName];
        }
        else
        {
            ushort hash = 0;
            foreach (var c in protoName)
            {
                ushort ch = (ushort)c;
                hash = (ushort)(hash + ((hash) << 5) + ch + (ch << 7));
            }
            proto_name_id.Add(protoName, (int)hash);
            //Debug.LogError("protoName:" + hash);
            return (int)hash;
        }
    }


}
