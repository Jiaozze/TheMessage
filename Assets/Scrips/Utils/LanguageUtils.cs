

public class LanguageUtils
{
    public static string GetCardName(CardNameEnum nameEnum)
    {
        switch(nameEnum)
        {
            case CardNameEnum.Bi_Po:
                return "±ÆÆÈ";
            case CardNameEnum.Cheng_Qing:
                return "³ÎÇå";
            case CardNameEnum.Diao_Bao:
                return "µô°ü";
            case CardNameEnum.Jie_Huo:
                return "½Ø»ñ";
            case CardNameEnum.Li_You:
                return "ÀûÓÕ";
            case CardNameEnum.Ping_Heng:
                return "Æ½ºâ";
            case CardNameEnum.Po_Yi:
                return "ÆÆÒë";
            case CardNameEnum.Shi_Tan:
                return "ÊÔÌ½";
            case CardNameEnum.Wu_Dao:
                return "Îóµ¼";
        }
        return "";
    }
}
