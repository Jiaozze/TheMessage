using Google.Protobuf;
//using Protos;
using System.Collections.Generic;
using UnityEngine;

public static class ProtoHelper
{
    private const uint PROTO_VERSION = 2;
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
            List<RoleBase> roles = new List<RoleBase>();
            for (int i = 0; i < init_Toc.Roles.Count; i++)
            {
                roles.Add(AllRoles.GetRole(init_Toc.Roles[i], i));
            }
            GameManager.Singleton.OnReceiveGameStart(playerCount, playerColor, secretTask, roles);
        }
        // 通知客户端，到谁的哪个阶段了
        else if (GetIdFromProtoName("notify_phase_toc") == id)
        {
            notify_phase_toc notify_phase_toc = notify_phase_toc.Parser.ParseFrom(contont);
            CardFS message = new CardFS(notify_phase_toc.MessageCard);
            GameManager.Singleton.OnReceiveTurn((int)notify_phase_toc.CurrentPlayerId, (int)notify_phase_toc.MessagePlayerId, (int)notify_phase_toc.WaitingPlayerId, (PhaseEnum)notify_phase_toc.CurrentPhase, (int)notify_phase_toc.WaitingSecond, (DirectionEnum)notify_phase_toc.MessageCardDir, message, notify_phase_toc.Seq);
            Debug.Log("_______receive________notify_phase_toc " + notify_phase_toc.WaitingPlayerId + " seq:" + notify_phase_toc.Seq);
        }
        // 通知所有人传情报
        else if (GetIdFromProtoName("send_message_card_toc") == id)
        {
            send_message_card_toc send_message_card_toc = send_message_card_toc.Parser.ParseFrom(contont);
            List<int> locks = new List<int>();
            foreach (var lockId in send_message_card_toc.LockPlayerIds)
            {
                locks.Add((int)lockId);
            }
            GameManager.Singleton.OnReceiveMessageSend((int)send_message_card_toc.PlayerId, (int)send_message_card_toc.CardId, (int)send_message_card_toc.TargetPlayerId, locks, (DirectionEnum)send_message_card_toc.CardDir);
            Debug.Log("_______receive________ send_message_card_toc " + send_message_card_toc.PlayerId);
        }
        // 通知所有人选择要接收情报（只有选择要收时有这条协议）
        else if (GetIdFromProtoName("choose_receive_toc") == id)
        {
            choose_receive_toc choose_receive_toc = choose_receive_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceiveMessageAccept((int)choose_receive_toc.PlayerId);
            Debug.Log("_______receive________ choose_receive_toc " + choose_receive_toc.PlayerId);
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
            Debug.Log("_______receive________ sync_deck_num_toc");
            sync_deck_num_toc sync_deck_num_toc = sync_deck_num_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceiveDeckNum((int)sync_deck_num_toc.Num, sync_deck_num_toc.Shuffled);
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
            CardFS cardLiYou = use_Li_You_Toc.LiYouCard == null ? null : new CardFS(use_Li_You_Toc.LiYouCard);
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

            //List<CardFS> cards = new List<CardFS>();
            //foreach (var card in use_Ping_Heng_Toc.DiscardCards)
            //{
            //    CardFS cardFS = new CardFS(card);
            //    cards.Add(cardFS);
            //}
            //GameManager.Singleton.OnReceiveDiscards(user, cards);

            //List<CardFS> targetCards = new List<CardFS>();
            //foreach (var card in use_Ping_Heng_Toc.TargetDiscardCards)
            //{
            //    CardFS cardFS = new CardFS(card);
            //    targetCards.Add(cardFS);
            //}
            //GameManager.Singleton.OnReceiveDiscards(target, targetCards);

        }
        // 通知所有人威逼的牌没有，展示所有手牌
        else if (GetIdFromProtoName("wei_bi_show_hand_card_toc") == id)
        {
            Debug.Log(" _______receive________ wei_bi_show_hand_card_toc");

            wei_bi_show_hand_card_toc wei_Bi_Show_Hand_Card_Toc = wei_bi_show_hand_card_toc.Parser.ParseFrom(contont);
            int user = (int)wei_Bi_Show_Hand_Card_Toc.PlayerId;
            int target = (int)wei_Bi_Show_Hand_Card_Toc.TargetPlayerId;
            CardFS cardUsed = wei_Bi_Show_Hand_Card_Toc.Card == null ? null : new CardFS(wei_Bi_Show_Hand_Card_Toc.Card);
            List<CardFS> cards = new List<CardFS>();
            foreach (var card in wei_Bi_Show_Hand_Card_Toc.Cards)
            {
                CardFS cardFS = new CardFS(card);
                cards.Add(cardFS);
            }
            CardNameEnum cardWant = (CardNameEnum)wei_Bi_Show_Hand_Card_Toc.WantType;

            GameManager.Singleton.OnReceiveUseWeiBiShowHands(user, target, cardUsed, cards, cardWant);
        }
        // 通知所有人威逼等待给牌
        else if (GetIdFromProtoName("wei_bi_wait_for_give_card_toc") == id)
        {
            Debug.Log(" _______receive________ wei_bi_wait_for_give_card_toc");

            wei_bi_wait_for_give_card_toc wei_bi_wait_for_give_card_toc = wei_bi_wait_for_give_card_toc.Parser.ParseFrom(contont);
            int user = (int)wei_bi_wait_for_give_card_toc.PlayerId;
            int target = (int)wei_bi_wait_for_give_card_toc.TargetPlayerId;
            CardFS cardUsed = wei_bi_wait_for_give_card_toc.Card == null ? null : new CardFS(wei_bi_wait_for_give_card_toc.Card);

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
        // 通知所有人澄清
        else if (GetIdFromProtoName("use_cheng_qing_toc") == id)
        {
            Debug.Log(" _______receive________ use_cheng_qing_toc");

            use_cheng_qing_toc use_cheng_qing_toc = use_cheng_qing_toc.Parser.ParseFrom(contont);
            int user = (int)use_cheng_qing_toc.PlayerId;
            int target = (int)use_cheng_qing_toc.TargetPlayerId;
            CardFS cardUsed = new CardFS(use_cheng_qing_toc.Card);
            int targetCardId = (int)use_cheng_qing_toc.TargetCardId;

            GameManager.Singleton.OnReceiveUseChengQing(user, target, cardUsed, targetCardId);
        }
        // 通知所有人使用破译，并询问是否翻开并摸一张牌（只有黑情报才能翻开）
        else if (GetIdFromProtoName("use_po_yi_toc") == id)
        {
            Debug.Log(" _______receive________ use_po_yi_toc");

            use_po_yi_toc use_po_yi_toc = use_po_yi_toc.Parser.ParseFrom(contont);
            int user = (int)use_po_yi_toc.PlayerId;
            CardFS cardUsed = new CardFS(use_po_yi_toc.Card);
            CardFS messageCard = new CardFS(use_po_yi_toc.MessageCard);
            int waitingTime = (int)use_po_yi_toc.WaitingSecond;

            GameManager.Singleton.OnReceiveUsePoYi(user, cardUsed, messageCard, waitingTime, use_po_yi_toc.Seq);
        }
        // 通知客户端破译翻牌结果
        else if (GetIdFromProtoName("po_yi_show_toc") == id)
        {
            Debug.Log(" _______receive________ po_yi_show_toc");

            po_yi_show_toc po_yi_show_toc = po_yi_show_toc.Parser.ParseFrom(contont);
            int user = (int)po_yi_show_toc.PlayerId;
            CardFS messageCard = new CardFS(po_yi_show_toc.MessageCard);
            bool show = po_yi_show_toc.Show;
            GameManager.Singleton.OnReceivePoYiShow(user, show, messageCard);
        }
        // 通知所有人使用调包
        else if (GetIdFromProtoName("use_diao_bao_toc") == id)
        {
            Debug.Log(" _______receive________ use_diao_bao_toc");

            use_diao_bao_toc use_diao_bao_toc = use_diao_bao_toc.Parser.ParseFrom(contont);
            int user = (int)use_diao_bao_toc.PlayerId;
            CardFS messageCard = new CardFS(use_diao_bao_toc.OldMessageCard);
            int cardUsed = (int)use_diao_bao_toc.CardId;
            GameManager.Singleton.OnReceiveUseDiaoBao(user, cardUsed, messageCard);
        }
        // 通知所有人使用截获
        else if (GetIdFromProtoName("use_jie_huo_toc") == id)
        {
            Debug.Log(" _______receive________ use_jie_huo_toc");

            use_jie_huo_toc use_diao_bao_toc = use_jie_huo_toc.Parser.ParseFrom(contont);
            int user = (int)use_diao_bao_toc.PlayerId;
            CardFS cardUsed = new CardFS(use_diao_bao_toc.Card);
            GameManager.Singleton.OnReceiveUseJieHuo(user, cardUsed);
        }
        // 通知所有人使用误导
        else if (GetIdFromProtoName("use_wu_dao_toc") == id)
        {
            Debug.Log(" _______receive________ use_wu_dao_toc");

            use_wu_dao_toc use_wu_dao_toc = use_wu_dao_toc.Parser.ParseFrom(contont);
            int user = (int)use_wu_dao_toc.PlayerId;
            int target = (int)use_wu_dao_toc.TargetPlayerId;
            CardFS cardUsed = new CardFS(use_wu_dao_toc.Card);
            GameManager.Singleton.OnReceiveUseWuDao(user, target, cardUsed);
        }
        #region 技能
        //端木静【新思潮】
        else if (GetIdFromProtoName("skill_xin_si_chao_toc") == id)
        {
            Debug.Log(" _______receive________ skill_xin_si_chao_toc");
            skill_xin_si_chao_toc skill_Xin_Si_Chao_Toc = skill_xin_si_chao_toc.Parser.ParseFrom(contont);
            UserSkill_XinSiChao.OnReceiveUse((int)skill_Xin_Si_Chao_Toc.PlayerId);
        }
        //邵秀【绵里藏针】
        else if (GetIdFromProtoName("skill_mian_li_cang_zhen_toc") == id)
        {
            Debug.Log(" _______receive________ skill_mian_li_cang_zhen_toc");
            skill_mian_li_cang_zhen_toc skill_mian_li_cang_zhen_toc = skill_mian_li_cang_zhen_toc.Parser.ParseFrom(contont);
            CardFS card = new CardFS(skill_mian_li_cang_zhen_toc.Card);
            UserSkill_MianLiCangZhen.OnReceiveUse((int)skill_mian_li_cang_zhen_toc.PlayerId, (int)skill_mian_li_cang_zhen_toc.TargetPlayerId, card);
        }
        // 金生火【谨慎】
        else if (GetIdFromProtoName("skill_jin_shen_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jin_shen_toc");
            skill_jin_shen_toc skill_jin_shen_toc = skill_jin_shen_toc.Parser.ParseFrom(contont);
            CardFS card = new CardFS(skill_jin_shen_toc.Card);
            UserSkill_JinShen.OnReceiveUse((int)skill_jin_shen_toc.PlayerId, card);
        }
        // 毛不拔【奇货可居】
        else if (GetIdFromProtoName("skill_qi_huo_ke_ju_toc") == id)
        {
            Debug.Log(" _______receive________ skill_qi_huo_ke_ju_toc");
            skill_qi_huo_ke_ju_toc skill_qi_huo_ke_ju_toc = skill_qi_huo_ke_ju_toc.Parser.ParseFrom(contont);
            UserSkill_QiHuoKeJu.OnReceiveUse((int)skill_qi_huo_ke_ju_toc.PlayerId, (int)skill_qi_huo_ke_ju_toc.CardId);
        }
        // 肥原龙川【诡诈】
        else if (GetIdFromProtoName("skill_gui_zha_toc") == id)
        {
            Debug.Log(" _______receive________ skill_gui_zha_toc");
            skill_gui_zha_toc skill_gui_zha_toc = skill_gui_zha_toc.Parser.ParseFrom(contont);
            UserSkill_GuiZha_LiYou.OnReceiveUse((int)skill_gui_zha_toc.PlayerId, (int)skill_gui_zha_toc.TargetPlayerId, (CardNameEnum)skill_gui_zha_toc.CardType);
            //UserSkill_QiHuoKeJu.OnReceiveUse((int)skill_gui_zha_toc.PlayerId, (int)skill_gui_zha_toc.CardId);
        }
        // 王魁【以牙还牙】
        else if(GetIdFromProtoName("skill_yi_ya_huan_ya_toc") == id)
        {
            Debug.Log(" _______receive________ skill_yi_ya_huan_ya_toc");
            skill_yi_ya_huan_ya_toc skill_Yi_Ya_Huan_Ya_Toc = skill_yi_ya_huan_ya_toc.Parser.ParseFrom(contont);
            CardFS card = new CardFS(skill_Yi_Ya_Huan_Ya_Toc.Card);
            UserSkill_YiYaHuanYa.OnReceiveUse((int)skill_Yi_Ya_Huan_Ya_Toc.PlayerId, (int)skill_Yi_Ya_Huan_Ya_Toc.PlayerId, card);
        }
        #endregion
        // 通知客户端谁死亡了
        else if (GetIdFromProtoName("notify_die_toc") == id)
        {
            Debug.Log(" _______receive________ notify_die_toc");

            notify_die_toc notify_die_toc = notify_die_toc.Parser.ParseFrom(contont);
            int playerId = (int)notify_die_toc.PlayerId;
            bool loseGame = notify_die_toc.LoseGame;
            GameManager.Singleton.OnReceivePlayerDied(playerId, loseGame);
        }
        // 通知客户端谁胜利了
        else if (GetIdFromProtoName("notify_winner_toc") == id)
        {
            Debug.Log(" _______receive________ notify_winner_toc");

            notify_winner_toc notify_winner_toc = notify_winner_toc.Parser.ParseFrom(contont);
            List<int> playerIds = new List<int>();
            foreach (var playerid in notify_winner_toc.DeclarePlayerIds)
            {
                playerIds.Add((int)playerid);
            }
            List<int> winners = new List<int>();
            foreach (var winner in notify_winner_toc.WinnerIds)
            {
                winners.Add((int)winner);
            }
            List<PlayerColorEnum> playerColers = new List<PlayerColorEnum>();
            List<SecretTaskEnum> playerTasks = new List<SecretTaskEnum>();
            for (int i = 0; i < notify_winner_toc.Identity.Count; i++)
            {
                playerColers.Add((PlayerColorEnum)notify_winner_toc.Identity[i]);
            }
            for (int i = 0; i < notify_winner_toc.SecretTasks.Count; i++)
            {
                playerTasks.Add((SecretTaskEnum)notify_winner_toc.SecretTasks[i]);
            }

            GameManager.Singleton.OnReceiveWinner(playerIds, winners, playerColers, playerTasks);
        }
        // 濒死求澄清
        else if (GetIdFromProtoName("wait_for_cheng_qing_toc") == id)
        {
            Debug.Log(" _______receive________ wait_for_cheng_qing_toc");

            wait_for_cheng_qing_toc wait_for_cheng_qing_toc = wait_for_cheng_qing_toc.Parser.ParseFrom(contont);
            int playerId = (int)wait_for_cheng_qing_toc.DiePlayerId;
            int waitingPlayer = (int)wait_for_cheng_qing_toc.WaitingPlayerId;
            int waitingSecond = (int)wait_for_cheng_qing_toc.WaitingSecond;
            GameManager.Singleton.OnReceiveWaitSaving(playerId, waitingPlayer, waitingSecond, wait_for_cheng_qing_toc.Seq);
        }
        // 等待死亡时给三张牌
        else if (GetIdFromProtoName("wait_for_die_give_card_toc") == id)
        {
            Debug.Log(" _______receive________ wait_for_die_give_card_toc");

            wait_for_die_give_card_toc wait_for_die_give_card_toc = wait_for_die_give_card_toc.Parser.ParseFrom(contont);
            int playerId = (int)wait_for_die_give_card_toc.PlayerId;
            int waitingSecond = (int)wait_for_die_give_card_toc.WaitingSecond;
            GameManager.Singleton.OnReceiveDieGiveingCard(playerId, waitingSecond, wait_for_die_give_card_toc.Seq);
        }
        else if (GetIdFromProtoName("notify_die_give_card_toc") == id)
        {
            Debug.Log(" _______receive________ notify_die_give_card_toc");

            notify_die_give_card_toc wait_for_die_give_card_toc = notify_die_give_card_toc.Parser.ParseFrom(contont);
            int playerId = (int)wait_for_die_give_card_toc.PlayerId;
            int targetPlayerId = (int)wait_for_die_give_card_toc.TargetPlayerId;
            List<CardFS> cards = new List<CardFS>();
            int cardCount = (int)wait_for_die_give_card_toc.CardCount;
            foreach (var card in wait_for_die_give_card_toc.Card)
            {
                cards.Add(new CardFS(card));
            }
            GameManager.Singleton.OnReceiveDieGivenCard(playerId, targetPlayerId, cardCount, cards);
        }
        // 返回房间所有人的信息
        else if (GetIdFromProtoName("get_room_info_toc") == id)
        {
            Debug.Log(" _______receive________ get_room_info_toc ");

            get_room_info_toc get_Room_Info_Toc = get_room_info_toc.Parser.ParseFrom(contont);
            List<string> names = new List<string>();
            foreach (var name in get_Room_Info_Toc.Names)
            {
                names.Add(name);
            }
            GameManager.Singleton.OnReceiveRoomInfo(names, (int)get_Room_Info_Toc.MyPosition);
        }
        // 通知谁加入了房间
        else if (GetIdFromProtoName("join_room_toc") == id)
        {
            join_room_toc join_room_toc = join_room_toc.Parser.ParseFrom(contont);
            Debug.Log(" _______receive________ join_room_toc " + join_room_toc.Position);

            GameManager.Singleton.OnAddPlayer(join_room_toc.Name, (int)join_room_toc.Position);

        }
        // 通知谁离开的房间
        else if (GetIdFromProtoName("leave_room_toc") == id)
        {
            leave_room_toc leave_room_toc = leave_room_toc.Parser.ParseFrom(contont);

            GameManager.Singleton.OnPlayerLeave((int)leave_room_toc.Position);
        }
        //errorCode
        else if (GetIdFromProtoName("error_code_toc") == id)
        {
            error_code_toc error_Code_Toc = error_code_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnErrorCode(error_Code_Toc.Code);
        }
        else
        {
            Debug.LogError("undefine proto:" + id);
        }
    }

    #region 出牌阶段协议
    public static void SendEndWaiting(uint seq)
    {
        Debug.Log("____send___________________ end_Main_Phase_Tos, seq:" + seq);
        end_main_phase_tos end_Main_Phase_Tos = new end_main_phase_tos() { Seq = seq };

        byte[] proto = end_Main_Phase_Tos.ToByteArray();
        SendProto("end_main_phase_tos", proto);
    }

    public static void SendEndFight(uint seq)
    {
        Debug.Log("____send___________________ end_fight_phase_tos, seq:" + seq);
        end_fight_phase_tos end_fight_phase_tos = new end_fight_phase_tos() { Seq = seq };

        byte[] proto = end_fight_phase_tos.ToByteArray();
        SendProto("end_fight_phase_tos", proto);
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

    // 请求使用澄清
    public static void SendUseCardMessage_ChengQing(int cardId, int playerId, int targetCardId, uint seq)
    {
        Debug.Log("____send___________________ use_cheng_qing_tos, seq:" + seq);
        use_cheng_qing_tos use_cheng_qing_tos = new use_cheng_qing_tos() { CardId = (uint)cardId, PlayerId = (uint)playerId, TargetCardId = (uint)targetCardId, Seq = seq };

        byte[] proto = use_cheng_qing_tos.ToByteArray();
        SendProto("use_cheng_qing_tos", proto);
    }


    // 试探弃牌或者摸牌
    public static void SendDoShiTan(int cardId, uint seq)
    {
        Debug.Log("____send___________________ execute_shi_tan_tos, seq:" + seq);
        execute_shi_tan_tos execute_Shi_Tan_Tos = new execute_shi_tan_tos() { Seq = seq };
        if (cardId > 0)
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
    #endregion

    #region 传情报阶段协议
    // 请求传情报
    public static void SendMessageCard(int cardId, int targetPlayer, List<int> lockPlayers, DirectionEnum dir, uint seq)
    {
        Debug.Log("____send___________________ send_message_card_tos, seq:" + seq);

        send_message_card_tos send_message_card_tos = new send_message_card_tos() { CardId = (uint)cardId, TargetPlayerId = (uint)targetPlayer, CardDir = (direction)dir, Seq = seq };
        foreach (var lockPlayer in lockPlayers)
        {
            if (lockPlayer != 0)
            {
                send_message_card_tos.LockPlayerId.Add((uint)lockPlayer);
            }
        }

        byte[] proto = send_message_card_tos.ToByteArray();
        SendProto("send_message_card_tos", proto);
    }

    // 选择是否接收情报
    public static void SendWhetherReceive(bool isReceive, uint seq)
    {
        Debug.Log("____send___________________ choose_whether_receive_tos, seq:" + seq);

        choose_whether_receive_tos choose_whether_receive_tos = new choose_whether_receive_tos() { Receive = isReceive, Seq = seq };

        byte[] proto = choose_whether_receive_tos.ToByteArray();
        SendProto("choose_whether_receive_tos", proto);
    }
    // 使用调包
    public static void SendUseDiaoBao(int cardId, uint seq)
    {
        Debug.Log("____send___________________ use_diao_bao_tos, seq:" + seq);

        use_diao_bao_tos use_Diao_Bao_Tos = new use_diao_bao_tos() { CardId = (uint)cardId, Seq = seq };
        byte[] proto = use_Diao_Bao_Tos.ToByteArray();
        SendProto("use_diao_bao_tos", proto);
    }
    // 使用破译
    public static void SendUseCardMessage_PoYi(int cardId, uint seq)
    {
        Debug.Log("____send___________________ use_po_yi_tos, seq:" + seq);
        use_po_yi_tos use_po_yi_tos = new use_po_yi_tos() { CardId = (uint)cardId, Seq = seq };

        byte[] proto = use_po_yi_tos.ToByteArray();
        SendProto("use_po_yi_tos", proto);
    }
    //使用截获
    public static void SendUseCardMessage_JieHuo(int cardId, uint seq)
    {
        Debug.Log("____send___________________ use_jie_huo_tos, seq:" + seq);
        use_jie_huo_tos use_po_yi_tos = new use_jie_huo_tos() { CardId = (uint)cardId, Seq = seq };

        byte[] proto = use_po_yi_tos.ToByteArray();
        SendProto("use_jie_huo_tos", proto);
    }
    public static void SendUseCardMessage_WuDao(int cardId, int target, uint seq)
    {
        Debug.Log("____send___________________ use_wu_dao_tos, seq:" + seq);
        use_wu_dao_tos use_wu_dao_tos = new use_wu_dao_tos() { CardId = (uint)cardId, TargetPlayerId = (uint)target, Seq = seq };

        byte[] proto = use_wu_dao_tos.ToByteArray();
        SendProto("use_wu_dao_tos", proto);
    }
    public static void SendPoYiShow(bool show, uint seq)
    {
        Debug.Log("____send___________________ po_yi_show_tos, seq:" + seq);
        po_yi_show_tos po_yi_show_tos = new po_yi_show_tos() { Show = show, Seq = seq };

        byte[] proto = po_yi_show_tos.ToByteArray();
        SendProto("po_yi_show_tos", proto);

    }
    #endregion

    // 是否使用澄清
    public static void SendChengQingSaveDying(bool use, int cardId, int targetCard, uint seq)
    {
        Debug.Log("____send___________________ cheng_qing_save_die_tos, seq:" + seq);

        cardId = cardId < 0 ? 0 : cardId;
        cheng_qing_save_die_tos cheng_Qing_Save_Die_Tos = new cheng_qing_save_die_tos() { Use = use, CardId = (uint)cardId, TargetCardId = (uint)targetCard, Seq = seq };
        byte[] proto = cheng_Qing_Save_Die_Tos.ToByteArray();
        SendProto("cheng_qing_save_die_tos", proto);
    }
    // 等待死亡时给三张牌
    public static void SendDieGiveCard(uint seq, List<int> cardIds = null, int playerId = 0)
    {
        Debug.Log("____send___________________ die_give_card_tos, seq:" + seq);

        die_give_card_tos die_give_card_tos = new die_give_card_tos() { TargetPlayerId = (uint)playerId, Seq = seq };
        if (cardIds != null)
        {
            foreach (var id in cardIds)
            {
                die_give_card_tos.CardId.Add((uint)id);
            }
        }
        byte[] proto = die_give_card_tos.ToByteArray();
        SendProto("die_give_card_tos", proto);
    }

    #region 技能
    //接收阶段，轮到自己，选择什么都不做
    public static void SendEndReceive(uint seq)
    {
        Debug.Log("____send___________________ end_receive_phase_tos, seq:" + seq);

        end_receive_phase_tos end_Receive_Phase_Tos = new end_receive_phase_tos() { Seq = seq };
        byte[] proto = end_Receive_Phase_Tos.ToByteArray();
        SendProto("end_receive_phase_tos", proto);
    }
    //奇货可居
    public static void SendSkill_QiHuoKeJu(int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_qi_huo_ke_ju_tos, seq:" + seq);

        skill_qi_huo_ke_ju_tos skill_qi_huo_ke_ju_tos = new skill_qi_huo_ke_ju_tos() { CardId = (uint)cardId, Seq = seq };
        byte[] proto = skill_qi_huo_ke_ju_tos.ToByteArray();
        SendProto("skill_qi_huo_ke_ju_tos", proto);

    }
    // 王魁【以牙还牙】
    public static void SendSkill_YiYaHuanYa(int cardId, int target, uint seq)
    {
        Debug.Log("____send___________________ skill_yi_ya_huan_ya_tos, seq:" + seq);
        skill_yi_ya_huan_ya_tos skill_yi_ya_huan_ya_tos = new skill_yi_ya_huan_ya_tos() { CardId = (uint)cardId, TargetPlayerId = (uint)target, Seq = seq };
        byte[] proto = skill_yi_ya_huan_ya_tos.ToByteArray();
        SendProto("skill_yi_ya_huan_ya_tos", proto);
    }

    // 肥原龙川【诡诈】
    public static void SendSkill_GuiZha(int target,CardNameEnum cardType, CardNameEnum cardWant, uint seq)
    {
        Debug.Log("____send___________________ skill_gui_zha_tos, seq:" + seq);

        skill_gui_zha_tos skill_gui_zha_tos = new skill_gui_zha_tos() { TargetPlayerId = (uint)target, CardType = (card_type)cardType, WantType = (card_type)cardWant,  Seq = seq };
        byte[] proto = skill_gui_zha_tos.ToByteArray();
        SendProto("skill_gui_zha_tos", proto);

    }
    //绵里藏针
    public static void SendSkill_MianLiCangZhen(int cardId, int targetId, uint seq)
    {
        Debug.Log("____send___________________ skill_mian_li_cang_zhen_tos, seq:" + seq);

        skill_mian_li_cang_zhen_tos skill_mian_li_cang_zhen_tos = new skill_mian_li_cang_zhen_tos() { CardId = (uint)cardId, TargetPlayerId = (uint)targetId, Seq = seq };
        byte[] proto = skill_mian_li_cang_zhen_tos.ToByteArray();

        SendProto("skill_mian_li_cang_zhen_tos", proto);
    }
    //谨慎
    public static void SendSkill_JinShen(int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_jin_shen_tos, seq:" + seq);

        skill_jin_shen_tos skill_jin_shen_tos = new skill_jin_shen_tos() { CardId = (uint)cardId, Seq = seq };
        byte[] proto = skill_jin_shen_tos.ToByteArray();

        SendProto("skill_jin_shen_tos", proto);
    }

    //新思潮
    public static void SendSkill_XinSiChao(int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_mian_li_cang_zhen_tos, seq:" + seq);

        skill_xin_si_chao_tos skill_xin_si_chao_tos = new skill_xin_si_chao_tos() { CardId = (uint)cardId, Seq = seq };
        byte[] proto = skill_xin_si_chao_tos.ToByteArray();
        
        SendProto("skill_xin_si_chao_tos", proto);
    }
    #endregion

    public static void SendAddAI()
    {
        add_robot_tos add_robot_tos = new add_robot_tos();
        byte[] proto = add_robot_tos.ToByteArray();
        SendProto("add_robot_tos", proto);
    }

    public static void SendAddRoom()
    {
        join_room_tos join_Room_Tos = new join_room_tos();
        join_Room_Tos.Version = PROTO_VERSION;
        byte[] proto = join_Room_Tos.ToByteArray();
        SendProto("join_room_tos", proto);
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
