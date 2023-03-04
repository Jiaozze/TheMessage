

using UnityEngine;

public class GameUtils
{
    public static Color GetCardColor(CardColorEnum colorEnum)
    {
        switch (colorEnum)
        {
            case CardColorEnum.Black:
                return Color.black;
            case CardColorEnum.Blue:
                return Color.blue;
            case CardColorEnum.Red:
                return Color.red;
        }

        return Color.white;
    }

    public static bool IsFightPhase()
    {
        if(GameManager.Singleton.curPhase == PhaseEnum.Fight_Phase && GameManager.Singleton.IsWaitSaving <= 0 && !GameManager.Singleton.IsWaitGiveCard)
        {
            return true;
        }
        return false;
    }
}
