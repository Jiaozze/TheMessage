

public class LanguageUtils
{
    public static string GetCardName(CardNameEnum nameEnum)
    {
        switch(nameEnum)
        {
            case CardNameEnum.Bi_Po:
                return "����";
            case CardNameEnum.Cheng_Qing:
                return "����";
            case CardNameEnum.Diao_Bao:
                return "����";
            case CardNameEnum.Jie_Huo:
                return "�ػ�";
            case CardNameEnum.Li_You:
                return "����";
            case CardNameEnum.Ping_Heng:
                return "ƽ��";
            case CardNameEnum.Po_Yi:
                return "����";
            case CardNameEnum.Shi_Tan:
                return "��̽";
            case CardNameEnum.Wu_Dao:
                return "��";
        }
        return "";
    }
}
