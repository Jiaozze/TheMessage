

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
        return "";
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
        return "";

    }
}
