

using System;

public class LanguageUtils
{
    public static string GetCardName(CardNameEnum nameEnum)
    {
        switch (nameEnum)
        {
            case CardNameEnum.Wei_Bi:
                return "威逼";
            case CardNameEnum.Cheng_Qing:
                return "澄清";
            case CardNameEnum.Diao_Bao:
                return "掉包";
            case CardNameEnum.Jie_Huo:
                return "截获";
            case CardNameEnum.Li_You:
                return "利诱";
            case CardNameEnum.Ping_Heng:
                return "平衡";
            case CardNameEnum.Po_Yi:
                return "破译";
            case CardNameEnum.Shi_Tan:
                return "试探";
            case CardNameEnum.Wu_Dao:
                return "误导";
        }
        return "undifined";
    }

    public static string GetTaskName(SecretTaskEnum secretTaskEnum)
    {
        switch(secretTaskEnum)
        {
            case SecretTaskEnum.Collector:
                return "你获得3张红色情报或者3张蓝色情报";
            case SecretTaskEnum.Killer:
                return "你的回合中，一名红色和蓝色情报合计不少于2张的人死亡";
            case SecretTaskEnum.Stealer:
                return "你的回合中，有人宣胜，则你代替他胜利";
            default:
                return "undifined";
        }
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
}
