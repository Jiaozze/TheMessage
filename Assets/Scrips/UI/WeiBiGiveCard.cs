using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeiBiGiveCard : MonoBehaviour
{
    public Text text;
    public Button button;

    private CardNameEnum cardWant;
    public void Show(CardNameEnum cardWant, int user, int waitTime)
    {
        gameObject.SetActive(true);
        this.cardWant = cardWant;
        text.text = string.Format( "请选择一张{0}交给{1}号玩家", LanguageUtils.GetCardName(cardWant), user);

        TimeDown(waitTime);
    }

    private IEnumerator TimeDown(int seconds)
    {
        float total = seconds;
        float secondsF = seconds;
        while (secondsF > 0)
        {
            button.interactable = true;
            secondsF = secondsF - 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        gameObject.SetActive(false);
    }

    public void OnClickGive()
    {
        CardFS card = null;
        if(GameManager.Singleton.cardsHand.ContainsKey(GameManager.Singleton.SelectCardId))
        {
            card = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId];
            if(card.cardName == cardWant)
            {
                GameManager.Singleton.SendDoWeiBi(GameManager.Singleton.SelectCardId);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("需要给出卡牌" + LanguageUtils.GetCardName(cardWant));
            }
        }
        else
        {
            Debug.LogError("请选一张牌");
        }

    }
}
