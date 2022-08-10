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
        }
    }
}
