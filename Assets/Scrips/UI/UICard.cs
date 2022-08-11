using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    public Animator animator;
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        if (cardInfo.id != -1)
        {
            textName.text = LanguageUtils.GetCardName(cardInfo.name);
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

            image.sprite = Resources.Load<Sprite>("Images/cards/" + cardInfo.name.ToString());
            if(cardInfo.name == CardNameEnum.Shi_Tan)
            {
                goShiTan.SetActive(true);
                string black = cardInfo.shiTanColor.Contains(PlayerColorEnum.Green) ? "+1" : "-1";
                string red = cardInfo.shiTanColor.Contains(PlayerColorEnum.Red) ? "+1" : "-1";
                string blue = cardInfo.shiTanColor.Contains(PlayerColorEnum.Blue) ? "+1" : "-1";
                //textShitan.text = "<color=#0000FF>" + blue + "</color>\n"
                //    + "<color=#FF0000>" + red + "</color>\n"
                //    + "<color=#000000>" + black + "</color>";
                textShitan.text = blue + "\n" + red + "\n" + "<color=#000000>" + black + "</color>";
            }
            else
            {
                goShiTan.SetActive(false);
            }
        }
    }
}
