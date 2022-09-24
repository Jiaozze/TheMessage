using Google.Protobuf;
//using Protos;
using System.Collections.Generic;
using UnityEngine;

public static class ProtoHelper
{
    private const uint PROTO_VERSION = 9;
    public static void OnReceiveMsg(int id, byte[] contont)
    {
        //GetIdFromProtoName("wait_for_select_role_toc");
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
            List<string> names = new List<string>();
            for (int i = 0; i < init_Toc.Roles.Count; i++)
            {
                roles.Add(AllRoles.GetRole(init_Toc.Roles[i], i));
            }
            for (int i = 0; i < init_Toc.Names.Count; i++)
            {
                names.Add(init_Toc.Names[i]);
            }
            GameManager.Singleton.OnReceiveGameStart(playerCount, playerColor, secretTask, roles, names);
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

            use_jie_huo_toc use_jie_huo_toc = use_jie_huo_toc.Parser.ParseFrom(contont);
            int user = (int)use_jie_huo_toc.PlayerId;
            CardFS cardUsed = use_jie_huo_toc.Card == null ? null : new CardFS(use_jie_huo_toc.Card);
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
        // 通知客户端角色变化
        else if (GetIdFromProtoName("notify_role_update_toc") == id)
        {
            Debug.Log(" _______receive________ notify_role_update_toc");
            notify_role_update_toc notify_role_update_toc = notify_role_update_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceivePlayerUpDate((int)notify_role_update_toc.PlayerId, notify_role_update_toc.Role );
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
        else if (GetIdFromProtoName("skill_yi_ya_huan_ya_toc") == id)
        {
            Debug.Log(" _______receive________ skill_yi_ya_huan_ya_toc");
            skill_yi_ya_huan_ya_toc skill_Yi_Ya_Huan_Ya_Toc = skill_yi_ya_huan_ya_toc.Parser.ParseFrom(contont);
            CardFS card = new CardFS(skill_Yi_Ya_Huan_Ya_Toc.Card);
            UserSkill_YiYaHuanYa.OnReceiveUse((int)skill_Yi_Ya_Huan_Ya_Toc.PlayerId, (int)skill_Yi_Ya_Huan_Ya_Toc.TargetPlayerId, card);
        }
        // 鄭文先【偷天】：争夺阶段你可以翻开此角色牌，然后视为你使用了一张【截获】。
        else if (GetIdFromProtoName("skill_tou_tian_toc") == id)
        {
            Debug.Log(" _______receive________ skill_tou_tian_toc");
            skill_tou_tian_toc skill_tou_tian_toc = skill_tou_tian_toc.Parser.ParseFrom(contont);
            UserSkill_TouTian.OnReceiveUse((int)skill_tou_tian_toc.PlayerId);
        }
        // 鄭文先 【换日】：你使用【调包】或【破译】后，可以将你的角色牌翻至面朝下。
        else if (GetIdFromProtoName("skill_huan_ri_toc") == id)
        {
            Debug.Log(" _______receive________ skill_huan_ri_toc");
            skill_huan_ri_toc skill_Yi_Ya_Huan_Ya_Toc = skill_huan_ri_toc.Parser.ParseFrom(contont);
            UserSkill_HuanRi.OnReceiveUse((int)skill_Yi_Ya_Huan_Ya_Toc.PlayerId);
        }
        // 广播使用【明饵】：你传出的红色或蓝色情报被接收后，你和接收者各摸一张牌。
        else if (GetIdFromProtoName("skill_ming_er_toc") == id)
        {
            Debug.Log(" _______receive________ skill_ming_er_toc");
            skill_ming_er_toc skill_ming_er_toc = skill_ming_er_toc.Parser.ParseFrom(contont);
            UserSkill_MingEr.OnReceiveUse((int)skill_ming_er_toc.PlayerId);
        }
        // 广播使用【移花接木】争夺阶段，你可以翻开此角色牌，然后从一名角色的情报区选择一张情报，将其置入另一名角色的情报区，若如此做会让其收集三张或更多同色情报，则改为将该情牌加入你的手牌。
        else if (GetIdFromProtoName("skill_yi_hua_jie_mu_toc") == id)
        {
            Debug.Log(" _______receive________ skill_yi_hua_jie_mu_toc");
            skill_yi_hua_jie_mu_toc skill_yi_hua_jie_mu_toc = skill_yi_hua_jie_mu_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_yi_hua_jie_mu_toc.PlayerId;
            int cardId = (int)skill_yi_hua_jie_mu_toc.CardId;
            int from = (int)skill_yi_hua_jie_mu_toc.FromPlayerId;
            int to = (int)skill_yi_hua_jie_mu_toc.ToPlayerId;
            UserSkill_YiHuaJieMu.OnReceiveUse(playerId, cardId, from, to, skill_yi_hua_jie_mu_toc.JoinIntoHand);
        }
        // 广播使用【视死】：你接收黑色情报后，摸两张牌
        else if (GetIdFromProtoName("skill_shi_si_toc") == id)
        {
            Debug.Log(" _______receive________ skill_shi_si_toc");
            skill_shi_si_toc skill_shi_si_toc = skill_shi_si_toc.Parser.ParseFrom(contont);
            UserSkill_ShiSi.OnReceiveUse((int)skill_shi_si_toc.PlayerId);
        }

        // 广播询问客户端使用【如归】
        else if (GetIdFromProtoName("skill_wait_for_ru_gui_toc") == id)
        {
            Debug.Log(" _______receive________ skill_wait_for_ru_gui_toc");
            skill_wait_for_ru_gui_toc skill_wait_for_ru_gui_toc = skill_wait_for_ru_gui_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceiveWaitSkillRuGui((int)skill_wait_for_ru_gui_toc.PlayerId, (int)skill_wait_for_ru_gui_toc.WaitingSecond, skill_wait_for_ru_gui_toc.Seq);
        }
        // 广播使用【如归】
        else if (GetIdFromProtoName("skill_ru_gui_toc") == id)
        {
            Debug.Log(" _______receive________ skill_ru_gui_toc");
            skill_ru_gui_toc skill_ru_gui_toc = skill_ru_gui_toc.Parser.ParseFrom(contont);
            UserSkill_RuGui.OnReceiveUse((int)skill_ru_gui_toc.PlayerId, (int)skill_ru_gui_toc.CardId, skill_ru_gui_toc.Enable);
        }
        // 广播使用【怜悯】
        else if (GetIdFromProtoName("skill_lian_min_toc") == id)
        {
            Debug.Log(" _______receive________ skill_lian_min_toc");
            skill_lian_min_toc skill_lian_min_toc = skill_lian_min_toc.Parser.ParseFrom(contont);
            UserSkill_LianMin.OnReceiveUse((int)skill_lian_min_toc.PlayerId, (int)skill_lian_min_toc.CardId, (int)skill_lian_min_toc.TargetPlayerId);
        }
        // 广播使用【腹黑】：你传出的黑色情报被接收后，你摸一张牌。
        else if (GetIdFromProtoName("skill_fu_hei_toc") == id)
        {
            Debug.Log(" _______receive________ skill_fu_hei_toc");
            skill_fu_hei_toc skill_fu_hei_toc = skill_fu_hei_toc.Parser.ParseFrom(contont);
            UserSkill_FuHei.OnReceiveUse((int)skill_fu_hei_toc.PlayerId);
        }
        // 顾小梦【集智】：一名角色濒死时，或争夺阶段，你可以翻开此角色牌，然后摸四张牌。
        else if (GetIdFromProtoName("skill_ji_zhi_toc") == id)
        {
            Debug.Log(" _______receive________ skill_ji_zhi_toc");
            skill_ji_zhi_toc skill_ji_zhi_toc = skill_ji_zhi_toc.Parser.ParseFrom(contont);
            UserSkill_JiZhi.OnReceiveUse((int)skill_ji_zhi_toc.PlayerId);
        }
        // 广播询问客户端使用【承志】
        else if (GetIdFromProtoName("skill_wait_for_cheng_zhi_toc") == id)
        {
            Debug.Log(" _______receive________ skill_wait_for_cheng_zhi_toc");
            skill_wait_for_cheng_zhi_toc skill_wait_for_cheng_zhi_toc = skill_wait_for_cheng_zhi_toc.Parser.ParseFrom(contont);
            int userId = (int)skill_wait_for_cheng_zhi_toc.PlayerId;
            int diePlayerId = (int)skill_wait_for_cheng_zhi_toc.DiePlayerId;
            //int cardsCount = (int)skill_wait_for_cheng_zhi_toc.UnknownCardCount;
            List<CardFS> cards = new List<CardFS>();
            foreach(var card in skill_wait_for_cheng_zhi_toc.Cards)
            {
                cards.Add(new CardFS(card));
            }
            PlayerColorEnum playerColor = (PlayerColorEnum)skill_wait_for_cheng_zhi_toc.Identity;
            SecretTaskEnum secretTask = (SecretTaskEnum)skill_wait_for_cheng_zhi_toc.SecretTask;
            int waitingSeconds = (int)skill_wait_for_cheng_zhi_toc.WaitingSecond;
            
            GameManager.Singleton.OnReceiveWaitSkillChengZhi(userId, diePlayerId, cards: cards, playerColor, secretTask, waitingSeconds, skill_wait_for_cheng_zhi_toc.Seq);
        }
        // 广播使用【承志】
        else if (GetIdFromProtoName("skill_cheng_zhi_toc") == id)
        {
            Debug.Log(" _______receive________ skill_cheng_zhi_toc");
            skill_cheng_zhi_toc skill_cheng_zhi_toc = skill_cheng_zhi_toc.Parser.ParseFrom(contont);
            UserSkill_ChengZhi.OnReceiveUse((int)skill_cheng_zhi_toc.PlayerId, (int)skill_cheng_zhi_toc.DiePlayerId, skill_cheng_zhi_toc.Enable);
        }
        // 广播使用【就计】A：你被【试探】【威逼】或【利诱】指定为目标后，你可以翻开此角色牌，然后摸两张牌。
        else if (GetIdFromProtoName("skill_jiu_ji_a_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jiu_ji_a_toc");
            skill_jiu_ji_a_toc skill_jiu_ji_a_toc = skill_jiu_ji_a_toc.Parser.ParseFrom(contont);
            UserSkill_JiuJi.OnReceiveUseA((int)skill_jiu_ji_a_toc.PlayerId);
        }
        // 广播使用【就计】B：并在触发此技能的卡牌结算后，将其加入你的手牌。
        else if (GetIdFromProtoName("skill_jiu_ji_b_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jiu_ji_b_toc");
            skill_jiu_ji_b_toc skill_jiu_ji_b_toc = skill_jiu_ji_b_toc.Parser.ParseFrom(contont);
            CardFS card = skill_jiu_ji_b_toc.Card != null ? new CardFS(skill_jiu_ji_b_toc.Card) : null;
            UserSkill_JiuJi.OnReceiveUseB((int)skill_jiu_ji_b_toc.PlayerId, card, skill_jiu_ji_b_toc.UnknownCardCount);
        }
        // 广播使用【城府】：【试探】和【威逼】对你无效。
        else if (GetIdFromProtoName("skill_cheng_fu_toc") == id)
        {
            Debug.Log(" _______receive________ skill_cheng_fu_toc");
            skill_cheng_fu_toc skill_cheng_fu_toc = skill_cheng_fu_toc.Parser.ParseFrom(contont);
            CardFS card = null;
            if(skill_cheng_fu_toc.Card != null)
            {
                card = new CardFS(skill_cheng_fu_toc.Card);
            }
            UserSkill_ChengFu.OnReceiveUse((int)skill_cheng_fu_toc.PlayerId, (int)skill_cheng_fu_toc.FromPlayerId, card, (int)skill_cheng_fu_toc.UnknownCardCount);
        }
        // 广播询问客户端使用【遗信】
        else if (GetIdFromProtoName("skill_wait_for_yi_xin_toc") == id)
        {
            Debug.Log(" _______receive________ skill_wait_for_yi_xin_toc");
            skill_wait_for_yi_xin_toc skill_wait_for_yi_xin_toc = skill_wait_for_yi_xin_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceiveWaitSkillYiXin((int)skill_wait_for_yi_xin_toc.PlayerId, (int)skill_wait_for_yi_xin_toc.WaitingSecond, skill_wait_for_yi_xin_toc.Seq);
        }
        // 广播使用【遗信】
        else if (GetIdFromProtoName("skill_yi_xin_toc") == id)
        {
            Debug.Log(" _______receive________ skill_yi_xin_toc");
            skill_yi_xin_toc skill_yi_xin_toc = skill_yi_xin_toc.Parser.ParseFrom(contont);
            CardFS card = new CardFS(skill_yi_xin_toc.Card);
            UserSkill_YiXin.OnReceiveUse((int)skill_yi_xin_toc.PlayerId, (int)skill_yi_xin_toc.TargetPlayerId, card, skill_yi_xin_toc.Enable);
        }
        // 广播使用【知音】：你接收红色或蓝色情报后，你和传出者各摸一张牌
        else if (GetIdFromProtoName("skill_zhi_yin_toc") == id)
        {
            Debug.Log(" _______receive________ skill_zhi_yin_toc");
            skill_zhi_yin_toc skill_zhi_yin_toc = skill_zhi_yin_toc.Parser.ParseFrom(contont);
            UserSkill_ZhiYin.OnReceiveUse((int)skill_zhi_yin_toc.PlayerId);
        }
        // 程小蝶【惊梦】A：你接收黑色情报后，可以查看一名角色的手牌。
        else if (GetIdFromProtoName("skill_jing_meng_a_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jing_meng_a_toc");
            skill_jing_meng_a_toc skill_jing_meng_a_toc = skill_jing_meng_a_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_jing_meng_a_toc.PlayerId;
            int targetId = (int)skill_jing_meng_a_toc.TargetPlayerId;
            int waitSeconds = (int)skill_jing_meng_a_toc.WaitingSecond;
            List<CardFS> cards = new List<CardFS>();
            foreach(var card in skill_jing_meng_a_toc.Cards)
            {
                cards.Add(new CardFS(card));
            }
            GameManager.Singleton.OnReceiveWaitSkillJingMeng(playerId, targetId, cards, waitSeconds, skill_jing_meng_a_toc.Seq);
        }
        // 广播使用【惊梦】B，弃牌走弃牌协议
        else if (GetIdFromProtoName("skill_jing_meng_b_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jing_meng_b_toc");
            skill_jing_meng_b_toc skill_jing_meng_b_toc = skill_jing_meng_b_toc.Parser.ParseFrom(contont);
            UserSkill_JingMeng.OnReceiveUseB((int)skill_jing_meng_b_toc.PlayerId, (int)skill_jing_meng_b_toc.TargetPlayerId);
        }
        // 广播使用【借刀杀人】A 争夺阶段，你可以翻开此角色牌，然后抽取另一名角色的一张手牌并展示之。若展示的牌是非黑色，则你摸一张牌。
        else if (GetIdFromProtoName("skill_jie_dao_sha_ren_a_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jie_dao_sha_ren_a_toc");
            skill_jie_dao_sha_ren_a_toc skill_jie_dao_sha_ren_a_toc = skill_jie_dao_sha_ren_a_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_jie_dao_sha_ren_a_toc.PlayerId;
            int targetId = (int)skill_jie_dao_sha_ren_a_toc.TargetPlayerId;
            CardFS card = new CardFS(skill_jie_dao_sha_ren_a_toc.Card);
            int waitSeconds = (int)skill_jie_dao_sha_ren_a_toc.WaitingSecond;
            UserSkill_JieDaoShaRen.OnReceiveUseA(playerId, targetId, card, waitSeconds, skill_jie_dao_sha_ren_a_toc.Seq);
        }
        // 广播使用【借刀杀人】B
        else if (GetIdFromProtoName("skill_jie_dao_sha_ren_b_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jie_dao_sha_ren_b_toc");
            skill_jie_dao_sha_ren_b_toc skill_jie_dao_sha_ren_b_toc = skill_jie_dao_sha_ren_b_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_jie_dao_sha_ren_b_toc.PlayerId;
            int targetId = (int)skill_jie_dao_sha_ren_b_toc.TargetPlayerId;
            CardFS card = new CardFS(skill_jie_dao_sha_ren_b_toc.Card);
            UserSkill_JieDaoShaRen.OnReceiveUseB(playerId, targetId, card);
        }
        // 广播使用【交际】A 出牌阶段限一次，你可以抽取一名角色的最多两张手牌。
        else if (GetIdFromProtoName("skill_jiao_ji_a_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jiao_ji_a_toc");
            skill_jiao_ji_a_toc skill_jiao_ji_a_toc = skill_jiao_ji_a_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_jiao_ji_a_toc.PlayerId;
            int targetId = (int)skill_jiao_ji_a_toc.TargetPlayerId;
            List<CardFS> cards = new List<CardFS>();
            foreach(var card in skill_jiao_ji_a_toc.Cards)
            {
                cards.Add(new CardFS(card));
            }
            UserSkill_JiaoJi.OnReceiveUseA(playerId, targetId, cards, (int)skill_jiao_ji_a_toc.UnknownCardCount, (int)skill_jiao_ji_a_toc.WaitingSecond, skill_jiao_ji_a_toc.Seq);
        }
        // 广播使用【交际】B 然后将等量手牌交给该角色。你每收集一张黑色情报，便可以少交一张牌。
        else if (GetIdFromProtoName("skill_jiao_ji_b_toc") == id)
        {
            Debug.Log(" _______receive________ skill_jiao_ji_b_toc");
            skill_jiao_ji_b_toc skill_jiao_ji_b_toc = skill_jiao_ji_b_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_jiao_ji_b_toc.PlayerId;
            int targetId = (int)skill_jiao_ji_b_toc.TargetPlayerId;
            List<CardFS> cards = new List<CardFS>();
            foreach (var card in skill_jiao_ji_b_toc.Cards)
            {
                cards.Add(new CardFS(card));
            }
            UserSkill_JiaoJi.OnReceiveUseB(playerId, targetId, cards, (int)skill_jiao_ji_b_toc.UnknownCardCount);
        }
        // 鬼脚【急送】：争夺阶段限一次，你可以弃置两张手牌，或从你的情报区弃置一张非黑色情报，然后将待收情报移至一名角色面前。
        else if (GetIdFromProtoName("skill_ji_song_toc") == id)
        {
            Debug.Log(" _______receive________ skill_ji_song_toc");
            skill_ji_song_toc skill_ji_song_toc = skill_ji_song_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_ji_song_toc.PlayerId;
            int targetId = (int)skill_ji_song_toc.TargetPlayerId;
            CardFS card = new CardFS(skill_ji_song_toc.MessageCard);

        }
        // 广播询问客户端使用【转交】
        else if (GetIdFromProtoName("skill_wait_for_zhuan_jiao_toc") == id)
        {
            Debug.Log(" _______receive________ skill_wait_for_zhuan_jiao_toc");
            skill_wait_for_zhuan_jiao_toc skill_wait_for_zhuan_jiao_toc = skill_wait_for_zhuan_jiao_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_wait_for_zhuan_jiao_toc.PlayerId;
            UserSkill_ZhuanJiao.OnReceiveWaitUse(playerId, (int)skill_wait_for_zhuan_jiao_toc.WaitingSecond, skill_wait_for_zhuan_jiao_toc.Seq);
        }
        // 白小年【转交】：你使用一张手牌后，可以从你的情报区选择一张非黑色情报，将其置入另一名角色的情报区，然后你摸两张牌。你不能通过此技能让任何角色收集三张或更多同色情报。
        else if (GetIdFromProtoName("skill_zhuan_jiao_toc") == id)
        {
            Debug.Log(" _______receive________ skill_zhuan_jiao_toc");
            skill_zhuan_jiao_toc skill_ji_song_toc = skill_zhuan_jiao_toc.Parser.ParseFrom(contont);
            int playerId = (int)skill_ji_song_toc.PlayerId;
            int targetId = (int)skill_ji_song_toc.TargetPlayerId;
            int cardId = (int)skill_ji_song_toc.CardId;
            UserSkill_ZhuanJiao.OnReceiveUse(playerId, cardId, targetId);
        }

        #endregion
        // 通知客户端谁死亡了（通知客户端将其置灰，之后不能再成为目标了）
        else if (GetIdFromProtoName("notify_dying_toc") == id)
        {
            Debug.Log(" _______receive________ notify_dying_toc");

            notify_dying_toc notify_die_toc = notify_dying_toc.Parser.ParseFrom(contont);
            int playerId = (int)notify_die_toc.PlayerId;
            bool loseGame = notify_die_toc.LoseGame;
            GameManager.Singleton.OnReceivePlayerDying(playerId, loseGame);
        }

        // 通知客户端谁死亡了（通知客户端弃掉所有情报）
        else if (GetIdFromProtoName("notify_die_toc") == id)
        {
            Debug.Log(" _______receive________ notify_die_toc");

            notify_die_toc notify_die_toc = notify_die_toc.Parser.ParseFrom(contont);
            int playerId = (int)notify_die_toc.PlayerId;
            //bool loseGame = notify_die_toc.LoseGame;
            GameManager.Singleton.OnReceivePlayerDied(playerId);
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
            int cardCount = (int)wait_for_die_give_card_toc.UnknownCardCount;
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
        // 广播房间加一个位置
        else if (GetIdFromProtoName("add_one_position_toc") == id)
        {
            add_one_position_toc add_one_position_toc = add_one_position_toc.Parser.ParseFrom(contont);

            GameManager.Singleton.roomUI.OnAddPositon();
        }
        // 广播房间减少一个位置
        else if (GetIdFromProtoName("remove_one_position_toc") == id)
        {
            remove_one_position_toc remove_one_position_toc = remove_one_position_toc.Parser.ParseFrom(contont);

            GameManager.Singleton.roomUI.OnRemovePosition((int)remove_one_position_toc.Position);
        }
        //通知客户端录像存好了
        else if (GetIdFromProtoName("save_record_success_toc") == id)
        {
            Debug.Log(" _______receive________ save_record_success_toc ");

            save_record_success_toc error_Code_Toc = save_record_success_toc.Parser.ParseFrom(contont);
            UnityEngine.GUIUtility.systemCopyBuffer = error_Code_Toc.RecordId;
            GameManager.Singleton.gameUI.ShowInfo("录像已成功保存，录像Id已复制到粘贴板");
        }
        // 等待客户端选角色
        else if (GetIdFromProtoName("wait_for_select_role_toc") == id)
        {
            Debug.Log(" _______receive________ wait_for_select_role_toc ");

            wait_for_select_role_toc wait_for_select_role_toc = wait_for_select_role_toc.Parser.ParseFrom(contont);
            int playerCount = (int)wait_for_select_role_toc.PlayerCount;
            PlayerColorEnum playerColor = (PlayerColorEnum)wait_for_select_role_toc.Identity;
            SecretTaskEnum secretTask = (SecretTaskEnum)wait_for_select_role_toc.SecretTask;
            List<role> roles = new List<role>();
            foreach(var role in wait_for_select_role_toc.Roles)
            {
                roles.Add(role);
            }
            GameManager.Singleton.OnReceiveChooseRole(playerCount, playerColor, secretTask, roles);
        }
        // 通知客户端选角色成功了
        else if (GetIdFromProtoName("select_role_toc") == id)
        {
            Debug.Log(" _______receive________ select_role_toc ");

            select_role_toc select_role_toc = select_role_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceiveChooseRoleSuccess(select_role_toc.Role);
        }
        // 托管返回
        else if (GetIdFromProtoName("auto_play_toc") == id)
        {
            Debug.Log(" _______receive________ auto_play_toc ");

            auto_play_toc auto_play_toc = auto_play_toc.Parser.ParseFrom(contont);
            GameManager.Singleton.OnReceiveTuoGuan(auto_play_toc.Enable);
        }
        // 心跳：服务器回复客户端
        else if (GetIdFromProtoName("heart_toc") == id)
        {
            //Debug.Log(" _______receive________ heart_toc ");

            //auto_play_toc auto_play_toc = auto_play_toc.Parser.ParseFrom(contont);
            //GameManager.Singleton.OnReceiveTuoGuan(auto_play_toc.Enable);
        }

        //errorCode
        else if (GetIdFromProtoName("error_code_toc") == id)
        {
            Debug.Log(" _______receive________ error_code_toc ");

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
    // 白小年【转交】：你使用一张手牌后，可以从你的情报区选择一张非黑色情报，将其置入另一名角色的情报区，然后你摸两张牌。你不能通过此技能让任何角色收集三张或更多同色情报。
    public static void SendSkill_ZhuanJiao(bool enable, int playerId, int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_zhuan_jiao_tos, playerId:" + playerId);

        skill_zhuan_jiao_tos skill_zhuan_jiao_tos = new skill_zhuan_jiao_tos() { Enable = enable, CardId = (uint)cardId, TargetPlayerId = (uint)playerId, Seq = seq };
        byte[] proto = skill_zhuan_jiao_tos.ToByteArray();
        SendProto("skill_zhuan_jiao_tos", proto);
    }
    // 鬼脚【急送】：争夺阶段限一次，你可以弃置两张手牌，或从你的情报区弃置一张非黑色情报，然后将待收情报移至一名角色面前。
    public static void SendSkill_JiSong(int playerId, List<int> cardId, int messageId, uint seq)
    {
        Debug.Log("____send___________________ skill_ji_song_tos, playerId:" + playerId);

        skill_ji_song_tos skill_ji_song_tos = new skill_ji_song_tos() { MessageCard = (uint)messageId, TargetPlayerId = (uint)playerId, Seq = seq };
        if(cardId.Count > 0)
        {
            foreach(var id in cardId)
            {
                skill_ji_song_tos.CardIds.Add((uint)id);
            }
        }
        byte[] proto = skill_ji_song_tos.ToByteArray();
        SendProto("skill_ji_song_tos", proto);
    }

    // 裴玲【交际】A：出牌阶段限一次，你可以抽取一名角色的最多两张手牌。
    public static void SendSkill_JiaoJiA(int playerId, uint seq)
    {
        Debug.Log("____send___________________ skill_jiao_ji_a_tos, playerId:" + playerId);

        skill_jiao_ji_a_tos skill_jiao_ji_a_tos = new skill_jiao_ji_a_tos() { TargetPlayerId = (uint)playerId, Seq = seq };
        byte[] proto = skill_jiao_ji_a_tos.ToByteArray();
        SendProto("skill_jiao_ji_a_tos", proto);
    }
    // 裴玲【交际】B：然后将等量手牌交给该角色。你每收集一张黑色情报，便可以少交一张牌。
    public static void SendSkill_JiaoJiB(List<int> cardsId, uint seq)
    {
        Debug.Log("____send___________________ skill_jiao_ji_b_tos ");

        skill_jiao_ji_b_tos skill_jiao_ji_b_tos = new skill_jiao_ji_b_tos() { Seq = seq };
        foreach(var id in cardsId)
        {
            skill_jiao_ji_b_tos.CardIds.Add((uint)id);
        }
        byte[] proto = skill_jiao_ji_b_tos.ToByteArray();
        SendProto("skill_jiao_ji_b_tos", proto);
    }

    // 商玉【借刀杀人】A：争夺阶段，你可以翻开此角色牌，然后抽取另一名角色的一张手牌并展示之。若展示的牌是非黑色，则你摸一张牌。
    public static void SendSkill_JieDaoShaRenA(int playerId, uint seq)
    {
        Debug.Log("____send___________________ skill_jie_dao_sha_ren_a_tos, playerId:" + playerId);

        skill_jie_dao_sha_ren_a_tos skill_jie_dao_sha_ren_a_tos = new skill_jie_dao_sha_ren_a_tos() { TargetPlayerId = (uint)playerId, Seq = seq };
        byte[] proto = skill_jie_dao_sha_ren_a_tos.ToByteArray();
        SendProto("skill_jie_dao_sha_ren_a_tos", proto);
    }
    // 商玉【借刀杀人】B：若展示的牌是黑色，则你可以将其置入一名角色的情报区，并将你的角色牌翻至面朝下。
    public static void SendSkill_JieDaoShaRenB(bool enable, int playerId, uint seq)
    {
        Debug.Log("____send___________________ skill_jie_dao_sha_ren_b_tos, playerId:" + playerId);

        skill_jie_dao_sha_ren_b_tos skill_jie_dao_sha_ren_b_tos = new skill_jie_dao_sha_ren_b_tos() { Enable = enable, TargetPlayerId = (uint)playerId, Seq = seq };
        byte[] proto = skill_jie_dao_sha_ren_b_tos.ToByteArray();
        SendProto("skill_jie_dao_sha_ren_b_tos", proto);
    }
    // 程小蝶【惊梦】A：你接收黑色情报后，可以查看一名角色的手牌。
    public static void SendSkill_JingMengA(int playerId, uint seq)
    {
        Debug.Log("____send___________________ skill_jing_meng_a_tos, playerId:" + playerId);

        skill_jing_meng_a_tos skill_jing_meng_a_tos = new skill_jing_meng_a_tos() { TargetPlayerId = (uint)playerId, Seq = seq };
        byte[] proto = skill_jing_meng_a_tos.ToByteArray();
        SendProto("skill_jing_meng_a_tos", proto);

    }
    // 程小蝶【惊梦】B：然后从中选择一张弃置。
    public static void SendSkill_JingMengB(int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_jing_meng_b_tos, cardId:" + cardId);

        skill_jing_meng_b_tos skill_jing_meng_b_tos = new skill_jing_meng_b_tos() { CardId = (uint)cardId, Seq = seq };
        byte[] proto = skill_jing_meng_b_tos.ToByteArray();
        SendProto("skill_jing_meng_b_tos", proto);

    }
    // 李宁玉【遗信】：你死亡前，可以将一张手牌置入另一名角色的情报区。
    public static void SendSkill_YiXin(bool use, int playerId, int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_yi_xin_tos, seq:" + seq);
        skill_yi_xin_tos skill_ji_zhi_tos = new skill_yi_xin_tos() { Enable = use, CardId = (uint)cardId, TargetPlayerId = (uint)playerId, Seq = seq };
        byte[] proto = skill_ji_zhi_tos.ToByteArray();
        SendProto("skill_yi_xin_tos", proto);
    }
    // 顾小梦【集智】：一名角色濒死时，或争夺阶段，你可以翻开此角色牌，然后摸四张牌。
    public static void SendSkill_JiZhi(uint seq)
    {
        Debug.Log("____send___________________ skill_ji_zhi_tos, seq:" + seq);

        skill_ji_zhi_tos skill_ji_zhi_tos = new skill_ji_zhi_tos() { Seq = seq };
        byte[] proto = skill_ji_zhi_tos.ToByteArray();
        SendProto("skill_ji_zhi_tos", proto);

    }
    // 顾小梦【承志】：一名其他角色死亡前，若此角色牌已翻开，则你获得其所有手牌，并查看其身份牌，你可以获得该身份牌，并将你原本的身份牌面朝下移出游戏。
    public static void SendSkill_ChengZhi(bool use, uint seq)
    {
        Debug.Log("____send___________________ skill_cheng_zhi_tos, seq:" + seq);

        skill_cheng_zhi_tos skill_cheng_zhi_tos = new skill_cheng_zhi_tos() { Enable = use, Seq = seq };
        byte[] proto = skill_cheng_zhi_tos.ToByteArray();
        SendProto("skill_cheng_zhi_tos", proto);
    }

    // 鄭文先【偷天】：争夺阶段你可以翻开此角色牌，然后视为你使用了一张【截获】。
    public static void SendSkill_TouTian(uint seq)
    {
        Debug.Log("____send___________________ skill_tou_tian_tos, seq:" + seq);

        skill_tou_tian_tos skill_tou_tian_tos = new skill_tou_tian_tos() { Seq = seq };
        byte[] proto = skill_tou_tian_tos.ToByteArray();
        SendProto("skill_tou_tian_tos", proto);
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

        skill_mian_li_cang_zhen_tos skill_mian_li_cang_zhen_tos = new skill_mian_li_cang_zhen_tos() { CardId = (uint)cardId, Seq = seq };
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
    // 移花接木
    public static void SendSkill_YiHuaJieMu(int from, int to, int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_yi_hua_jie_mu_tos, seq:" + seq);

        skill_yi_hua_jie_mu_tos skill_yi_hua_jie_mu_tos = new skill_yi_hua_jie_mu_tos() { CardId = (uint)cardId, FromPlayerId = (uint)from, ToPlayerId = (uint)to, Seq = seq };
        byte[] proto = skill_yi_hua_jie_mu_tos.ToByteArray();

        SendProto("skill_yi_hua_jie_mu_tos", proto);

    }
    // 老汉【如归】：你死亡前，可以将你情报区中的一张情报置入当前回合角色的情报区中。
    public static void SendSkill_RuGui(bool used, int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_ru_gui_tos, seq:" + seq);

        skill_ru_gui_tos skill_ru_gui_tos = new skill_ru_gui_tos() { CardId = (uint)cardId, Enable = used, Seq = seq };
        byte[] proto = skill_ru_gui_tos.ToByteArray();

        SendProto("skill_ru_gui_tos", proto);
    }
    // 白菲菲【怜悯】：你传出的非黑色情报被接收后，可以从你或接收者的情报区选择一张黑色情报加入你的手牌。
    public static void SendSkill_LianMin(int playerId, int cardId, uint seq)
    {
        Debug.Log("____send___________________ skill_lian_min_tos, seq:" + seq);

        skill_lian_min_tos skill_lian_min_tos = new skill_lian_min_tos() { CardId = (uint)cardId, TargetPlayerId = (uint)playerId, Seq = seq };
        byte[] proto = skill_lian_min_tos.ToByteArray();

        SendProto("skill_lian_min_tos", proto);
    }

    #endregion
    public static void SendAddPosition()
    {
        Debug.Log("____send___________________ add_one_position_tos");

        add_one_position_tos add_one_position_tos = new add_one_position_tos();
        byte[] proto = add_one_position_tos.ToByteArray();
        SendProto("add_one_position_tos", proto);
    }
    public static void SendRemovePosion()
    {
        Debug.Log("____send___________________ remove_one_position_tos");

        remove_one_position_tos remove_one_position_tos = new remove_one_position_tos();
        byte[] proto = remove_one_position_tos.ToByteArray();
        SendProto("remove_one_position_tos", proto);
    }
    public static void SendAddAI()
    {
        Debug.Log("____send___________________ add_robot_tos");

        add_robot_tos add_robot_tos = new add_robot_tos();
        byte[] proto = add_robot_tos.ToByteArray();
        SendProto("add_robot_tos", proto);
    }

    public static void SendRemoveAI()
    {
        Debug.Log("____send___________________ remove_robot_tos");

        remove_robot_tos remove_robot_tos = new remove_robot_tos();
        byte[] proto = remove_robot_tos.ToByteArray();
        SendProto("remove_robot_tos", proto);
    }
    public static void SendPlayRecord(string recordId)
    {
        Debug.Log("____send___________________ display_record_tos");

        display_record_tos display_record_tos = new display_record_tos() { Version = PROTO_VERSION, RecordId = recordId };
        byte[] proto = display_record_tos.ToByteArray();
        SendProto("display_record_tos", proto);
    }

    public static void SendAddRoom(string name = "", string deviceId = "")
    {
        join_room_tos join_Room_Tos = new join_room_tos() { Name = name, Device = deviceId};
        join_Room_Tos.Version = PROTO_VERSION;
        byte[] proto = join_Room_Tos.ToByteArray();
        SendProto("join_room_tos", proto);
    }

    public static void SendSelectRole(role roleSelect)
    {
        select_role_tos join_Room_Tos = new select_role_tos() { Role = roleSelect };
        byte[] proto = join_Room_Tos.ToByteArray();
        SendProto("select_role_tos", proto);
    }

    public static void SendTuoGuan(bool enable)
    {
        Debug.Log("____send___________________ auto_play_tos");

        auto_play_tos auto_play_tos = new auto_play_tos() { Enable = enable };
        byte[] proto = auto_play_tos.ToByteArray();
        SendProto("auto_play_tos", proto);
    }
    public static void SendHeart()
    {
        Debug.Log("heart_tos");
        heart_tos heart_tos = new heart_tos() { };
        byte[] proto = heart_tos.ToByteArray();
        SendProto("heart_tos", proto);
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
            //Debug.LogError("protoName:" + protoName + "," + hash);
            return (int)hash;
        }
    }

}
