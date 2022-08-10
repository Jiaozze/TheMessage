

public class LanguageUtils
{
    public static string GetCardName(CardNameEnum nameEnum)
    {
        switch(nameEnum)
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
}
