

using System;
using System.Collections.Generic;

public class LanguageUtils
{
    public static string GetCardName(CardNameEnum nameEnum)
    {
        switch (nameEnum)
        {
            case CardNameEnum.WeiBi:
                return "威逼";
            case CardNameEnum.ChengQing:
                return "澄清";
            case CardNameEnum.DiaoBao:
                return "调包";
            case CardNameEnum.JieHuo:
                return "截获";
            case CardNameEnum.LiYou:
                return "利诱";
            case CardNameEnum.PingHeng:
                return "平衡";
            case CardNameEnum.PoYi:
                return "破译";
            case CardNameEnum.ShiTan:
                return "试探";
            case CardNameEnum.WuDao:
                return "误导";
            case CardNameEnum.FengYunBianHuan:
                return "风云变幻";
        }
        return "undifined";
    }

    public static string GetTaskName(SecretTaskEnum secretTaskEnum)
    {
        switch(secretTaskEnum)
        {
            case SecretTaskEnum.Collector:
                return "[双面间谍]你获得3张<color=\"red\">红色情报</color>或者3张<color=\"blue\">蓝色情报</color>";
            case SecretTaskEnum.Killer:
                return "[镇压者]你的回合中，一名<color=\"red\">红色情报</color>和<color=\"blue\">蓝色情报</color>合计不少于2张的人死亡";
            case SecretTaskEnum.Stealer:
                return "[篡夺者]你的回合中，有人宣胜，则你代替他胜利";
            case SecretTaskEnum.Mutator:
                return "[诱变者]当一名角色收集了三张<color=\"red\">红色情报</color>或三张<color=\"blue\">蓝色情报</color>后，若其没有宣告胜利，则你胜利";
            case SecretTaskEnum.Pioneer:
                return "[先行者]你死亡时，已收集了至少一张<color=\"red\">红色情报</color>或<color=\"blue\">蓝色情报</color>";
            default:
                return "undifined";
        }
    }

    public static string GetColorsName(List<CardColorEnum> cardColors)
    {
        string s = "";
        foreach(var color in cardColors)
        {
            s += GetColorName(color);
        }
        return s;
    }

    public static string GetColorName(CardColorEnum cardColor)
    {
        switch (cardColor)
        {
            case CardColorEnum.Red:
                return "<color=#FF0000>红色</color>";
            case CardColorEnum.Blue:
                return "<color=#0000FF>蓝色</color>";
            case CardColorEnum.Black:
                return "<color=#FFFFFF>黑色</color>";
            default:
                return "undifined";
        }
    }

    public static string GetPhaseName(PhaseEnum phase)
    {
        switch (phase)
        {
            case PhaseEnum.Draw_Phase:
                return "摸牌阶段";
            case PhaseEnum.Main_Phase:
                return "出牌阶段";
            case PhaseEnum.Send_Start_Phase:
                return "开始传递情报";
            case PhaseEnum.Send_Phase:
                return "情报传递阶段";
            case PhaseEnum.Fight_Phase:
                return "情报争夺阶段";
            case PhaseEnum.Receive_Phase:
                return "情报接收阶段";
        }
        return "undifined";

    }

    public static string GetIdentityName(PlayerColorEnum playerColor)
    {
        switch(playerColor)
        {
            case PlayerColorEnum.Blue:
                return "<color=#0000FF>特工机关</color>";
            case PlayerColorEnum.Red:
                return "<color=#FF0000>潜伏战线</color>";
            case PlayerColorEnum.Green:
                return "<color=#00FF00>神秘人</color>";
            default:
                return "undifined";
        }
    }

    public static string IdToWord(int id)
    {
        switch(id)
        {
            case 0:
                return "零";
            case 1:
                return "一";
            case 2:
                return "二";
            case 3:
                return "三";
            case 4:
                return "四";
            case 5:
                return "五";
            case 6:
                return "六";
            case 7:
                return "七";
            case 8:
                return "八";
            case 9:
                return "九";

            default:
                return "" + id;
        }
    }
}
