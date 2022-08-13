

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
}
