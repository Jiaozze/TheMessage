using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoYiResult : MonoBehaviour
{
    public UICard uiMessage;
    public GameObject goCanShow;
    public GameObject goCanNotShow;

    public void Show(CardFS message)
    {
        gameObject.SetActive(true);
        uiMessage.SetInfo(message);
        bool canShow = message.color.Contains(CardColorEnum.Black);
        goCanShow.SetActive(canShow);
        goCanNotShow.SetActive(!canShow);
    }

    public void OnClickNotShow()
    {
        GameManager.Singleton.SendPoYiShow(false);
        gameObject.SetActive(false);
    }

    public void OnClickShow()
    {
        GameManager.Singleton.SendPoYiShow(true);
        gameObject.SetActive(false);
    }
}
