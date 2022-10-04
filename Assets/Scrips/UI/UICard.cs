using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    public Button button;
    public Text textName;
    public GameObject goArrowUp;
    public GameObject goArrowLeft;
    public GameObject goArrowRight;
    public Image imgColor;
    public Image imgColor2;
    public Image image;
    public GameObject goLock;
    public Text textShitan;
    public GameObject goShiTan;
    public Transform transContainer;
    public GameObject goUnknown;

    private int cardId;
    private CardNameEnum cardName;
    private List<PlayerColorEnum> shiTanColor;

    public string cardDes
    {
        get
        {
            if (cardId > 0)
            {
                switch (cardName)
                {
                    case CardNameEnum.ChengQing:
                        return "澄清：出牌阶段，弃掉一名角色面前的一张黑情报。你也可以在一名角色濒死时对其使用";
                    case CardNameEnum.DiaoBao:
                        return "调包：争夺阶段，将这张牌面朝下代替待接收情报。";
                    case CardNameEnum.JieHuo:
                        return "截获：争夺阶段，将待接收情报移动至你面前。";
                    case CardNameEnum.LiYou:
                        return "利诱：出牌阶段，选择一名角色，将牌堆顶的一张牌置于他的情报区。若如此做会让其收集三张或更多同色情报，则改为将该情报加入你的手牌。";
                    case CardNameEnum.PingHeng:
                        return "平衡：出牌阶段，选择一名其他角色，你和他依次弃掉所有手牌，再依次摸3张牌。";
                    case CardNameEnum.PoYi:
                        return "破译：情报传递阶段，当情报传递到自己面前时使用。检视这张情报，若为黑色，你可以将其翻开并摸1张牌";
                    case CardNameEnum.ShiTan:
                        string shitan = "";
                        shitan += shiTanColor.Contains(PlayerColorEnum.Blue) ? LanguageUtils.GetIdentityName(PlayerColorEnum.Blue) : "";
                        shitan += shiTanColor.Contains(PlayerColorEnum.Red) ? LanguageUtils.GetIdentityName(PlayerColorEnum.Red) : "";
                        shitan += shiTanColor.Contains(PlayerColorEnum.Green) ? LanguageUtils.GetIdentityName(PlayerColorEnum.Green) : "";
                        shitan += ":摸一张牌\n";
                        shitan += shiTanColor.Contains(PlayerColorEnum.Blue) ? "" : LanguageUtils.GetIdentityName(PlayerColorEnum.Blue);
                        shitan += shiTanColor.Contains(PlayerColorEnum.Red) ? "" : LanguageUtils.GetIdentityName(PlayerColorEnum.Red);
                        shitan += shiTanColor.Contains(PlayerColorEnum.Green) ? "" : LanguageUtils.GetIdentityName(PlayerColorEnum.Green);
                        shitan += ":弃一张牌\n";

                        return "出牌阶段，将这张牌面朝下交给一名其他角色，其必须根据自己的身份牌如实执行:\n" + shitan + "执行之后将这张牌移出游戏";
                    case CardNameEnum.WeiBi:
                        return "威逼：出牌阶段，选择一名其他角色，声明 【澄清】【截获】【调包】【误导】中的一个卡名，对方必须给你一张你声明的卡。若对方没有，则将所有手牌展示给你。";
                    case CardNameEnum.WuDao:
                        return "误导：争夺阶段，将待接收情报移动至当前角色相邻的角色面前。";
                }
                return "";
            }
            else
            {
                return "";
            }
        }
    }
    void Awake()
    {
        button.onClick.AddListener(() =>
        {
            if (GameManager.Singleton.cardsHand.ContainsKey(cardId))
            {
                if (GameManager.Singleton.SelectCardId == cardId)
                {
                    GameManager.Singleton.SelectCardId = -1;
                }
                else
                {
                    GameManager.Singleton.SelectCardId = cardId;
                }
            }
        });
    }

    public void SetClickAction(UnityEngine.Events.UnityAction onClick)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }

    public void Init(int index, CardFS cardInfo = null)
    {
        if (cardInfo != null)
        {
            SetInfo(cardInfo);
        }
        gameObject.SetActive(true);
        index = index % 4;
        index = index == 0 ? 4 : index;
        animator.SetTrigger("InitCard" + index);
    }

    public void SetInfo(CardFS cardInfo)
    {
        cardId = cardInfo.id;
        cardName = cardInfo.cardName;
        shiTanColor = cardInfo.shiTanColor;
        if (cardInfo.id > 0)
        {
            goUnknown.SetActive(false);

            textName.text = LanguageUtils.GetCardName(cardInfo.cardName);
            goArrowLeft.SetActive(cardInfo.direction == DirectionEnum.Left);
            goArrowRight.SetActive(cardInfo.direction == DirectionEnum.Right);
            goArrowUp.SetActive(cardInfo.direction == DirectionEnum.Up);

            imgColor.color = GameUtils.GetCardColor(cardInfo.color[0]);
            imgColor2.gameObject.SetActive(cardInfo.color.Count > 1);
            if (cardInfo.color.Count > 1)
            {
                imgColor2.color = GameUtils.GetCardColor(cardInfo.color[1]);
            }

            goLock.SetActive(cardInfo.canLock);

            image.sprite = Resources.Load<Sprite>("Images/cards/" + cardInfo.cardName.ToString());
            if (cardInfo.cardName == CardNameEnum.ShiTan)
            {
                goShiTan.SetActive(true);
                string black = cardInfo.shiTanColor.Contains(PlayerColorEnum.Green) ? "+1" : "-1";
                string red = cardInfo.shiTanColor.Contains(PlayerColorEnum.Red) ? "+1" : "-1";
                string blue = cardInfo.shiTanColor.Contains(PlayerColorEnum.Blue) ? "+1" : "-1";
                //textShitan.text = "<color=#0000FF>" + blue + "</color>\n"
                //    + "<color=#FF0000>" + red + "</color>\n"
                //    + "<color=#000000>" + black + "</color>";
                textShitan.text = blue + "  " + red + "  " + "<color=#000000>" + black + "</color>";
            }
            else
            {
                goShiTan.SetActive(false);
            }
        }
        // 暗牌
        else
        {
            goUnknown.SetActive(true);
        }
    }

    public void OnSelect(bool select)
    {
        float y = select ? 30 : 0;
        transContainer.localPosition = new Vector3(0, y);
    }

    public void OnSend()
    {
        Debug.Log("卡被传出去了" + textName.text);
        Destroy(gameObject);
    }

    public void OnDiscard()
    {
        //TODO
        Debug.Log("卡被弃了" + textName.text);
        Destroy(gameObject);
    }

    public bool IsUnknown()
    {
        return goUnknown.activeSelf;
    }

    public void TurnOn(CardFS cardInfo)
    {
        gameObject.SetActive(true);
        animator.SetTrigger("TurnOn");
        SetInfo(cardInfo);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_STANDALONE
        if(cardId > 0)
        {
            GameUI.ShowDesInfo(cardDes, eventData.position);
        }
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {
#if UNITY_STANDALONE
        GameUI.HideDesInfo();
#endif
    }
    private Coroutine showInfoCorout;

    public void PointerUp()
    {
#if UNITY_ANDROID
        //GameUI.HideDesInfo();
        if(showInfoCorout!=null)
        {
            StopCoroutine(showInfoCorout);
        }
#endif
    }

    public void PointerDown()
    {
#if UNITY_ANDROID
        GameUI.HideDesInfo();
        if (cardId > 0)
        {
            showInfoCorout = StartCoroutine(ShowInfo());
        }
        //GameUI.ShowDesInfo(roleDes, eventData.position);
#endif
    }

    private IEnumerator ShowInfo()
    {
        yield return new WaitForSeconds(0.2f);
        GameUI.ShowDesInfo(cardDes, transform.position);
    }
}
