using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 顾小梦【承志】：一名其他角色死亡前，若此角色牌已翻开，则你获得其所有手牌，并查看其身份牌，你可以获得该身份牌，并将你原本的身份牌面朝下移出游戏。
public class UserSkill_ChengZhi : SkillBase
{
    public UserSkill_ChengZhi(int id)
    {
        playerId = id;
    }

    public override string name => "承志";

    public override bool canUse => false;

    public override string Des => "承志：一名其他角色死亡前，若此角色牌已翻开，则你获得其所有手牌，并查看其身份牌，你可以获得该身份牌，并将你原本的身份牌面朝下移出游戏。\n";

    public static PlayerColorEnum dieColor;
    public static SecretTaskEnum dieTask;
    public override void PrepareUse()
    {
        base.PrepareUse();
        GameManager.Singleton.IsUsingSkill = true;
        GameManager.Singleton.selectSkill = this;
        string taskString = dieColor == PlayerColorEnum.Green ? " 机密任务为:" + LanguageUtils.GetTaskName(dieTask) : "";
        GameManager.Singleton.gameUI.ShowPhase("触发技能承志，死亡角色身份为" + LanguageUtils.GetIdentityName(dieColor) + ",是否获得该身份牌");
        //GameManager.Singleton.gameUI.ShowPlayerMessageInfo(GameManager.SelfPlayerId);
    }

    public override void Use()
    {
        ProtoHelper.SendSkill_ChengZhi(true, GameManager.Singleton.seqId);
    }
    public override void Cancel()
    {
        base.Cancel();
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
        ProtoHelper.SendSkill_ChengZhi(false, GameManager.Singleton.seqId);
    }

    public override void OnUse()
    {
        GameManager.Singleton.IsUsingSkill = false;
        GameManager.Singleton.selectSkill = null;
        GameManager.Singleton.gameUI.ShowPhase();
    }

    public static void OnReceiveUse(int playerId, int diePlayerId, bool enable)
    {
        if (playerId == GameManager.SelfPlayerId)
        {
            if (GameManager.Singleton.selectSkill != null)
            {
                GameManager.Singleton.selectSkill.OnUse();
            }
        }
        if(enable)
        {
            if(playerId == GameManager.SelfPlayerId)
            {
                GameManager.Singleton.players[playerId].playerColor = new List<PlayerColorEnum>() { UserSkill_ChengZhi.dieColor };
                GameManager.Singleton.OnTaskChange(dieTask);
            }
            else
            {
                GameManager.Singleton.players[playerId].playerColor = GameManager.Singleton.players[diePlayerId].playerColor;
            }
            GameManager.Singleton.players[diePlayerId].playerColor = new List<PlayerColorEnum>();
            GameManager.Singleton.gameUI.Players[playerId].playerColor.SetColor(GameManager.Singleton.players[playerId].playerColor);
            GameManager.Singleton.gameUI.Players[playerId].playerColor.goTogBlue.isOn = GameManager.Singleton.gameUI.Players[diePlayerId].playerColor.goTogBlue.isOn;
            GameManager.Singleton.gameUI.Players[playerId].playerColor.goTogGreen.isOn = GameManager.Singleton.gameUI.Players[diePlayerId].playerColor.goTogGreen.isOn;
            GameManager.Singleton.gameUI.Players[playerId].playerColor.goTogRed.isOn = GameManager.Singleton.gameUI.Players[diePlayerId].playerColor.goTogRed.isOn;
            GameManager.Singleton.gameUI.Players[diePlayerId].playerColor.SetColor(GameManager.Singleton.players[diePlayerId].playerColor);

            string s = string.Format("{0}发动了承志, 获得{1}的身份牌", GameManager.Singleton.players[playerId].name, GameManager.Singleton.players[diePlayerId].name);
            GameManager.Singleton.gameUI.ShowInfo(s);
            GameManager.Singleton.gameUI.AddMsg(s);
        }
    }
}
