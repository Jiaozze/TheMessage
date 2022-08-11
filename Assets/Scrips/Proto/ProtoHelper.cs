using Google.Protobuf;
//using Protos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

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
                GameManager.Singleton.OnPlayerDrawCards(uno_Cards);
            }
            else
            {
                GameManager.Singleton.OnOtherDrawCards((int)add_card_toc.PlayerId, (int)add_card_toc.UnknownCardCount);
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
            GameManager.Singleton.OnTurn((int)notify_phase_toc.CurrentPlayerId, (int)notify_phase_toc.IntelligencePlayerId, (int)notify_phase_toc.WaitingPlayerId, (PhaseEnum)notify_phase_toc.CurrentPhase, (int)notify_phase_toc.WaitingSecond, notify_phase_toc.Seq);
            Debug.Log("_______receive________notify_phase_toc " + notify_phase_toc.WaitingPlayerId + " seq:" + notify_phase_toc.Seq);
        }
        // 通知客户端，谁对谁使用了试探
        else if (GetIdFromProtoName("use_shi_tan_toc") == id)
        {
            Debug.LogError("TODO use_shi_tan_toc");
        }
        // 向被试探者展示试探，并等待回应
        else if (GetIdFromProtoName("show_shi_tan_toc") == id)
        {
            Debug.LogError("TODO show_shi_tan_toc");
        }
        // 被试探者执行试探
        else if (GetIdFromProtoName("execute_shi_tan_toc") == id)
        {
            Debug.LogError("TODO execute_shi_tan_toc");
        }
        // 通知客户端，牌堆的剩余数量
        else if (GetIdFromProtoName("sync_deck_num_toc") == id)
        {
            Debug.LogError("TODO sync_deck_num_toc");

        }
        // 通知客户端，牌从谁的手牌被弃掉
        else if (GetIdFromProtoName("discard_card_toc") == id)
        {
            Debug.LogError("TODO discard_card_toc");

        }
        else
        {
            Debug.LogError("undefine proto:" + id);
        }
    }

    public static void SendUserCardMessage_ShiTan(int cardId, int playerId, uint seq)
    {
        Debug.Log("____send___________________SendUserCardMessage_ShiTan, seq:" + seq);
        use_shi_tan_tos use_shi_tan_tos = new use_shi_tan_tos() { CardId = (uint)cardId, PlayerId = (uint)playerId, Seq = seq};

        byte[] proto = use_shi_tan_tos.ToByteArray();
        SendProto("use_shi_tan_tos", proto);
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
