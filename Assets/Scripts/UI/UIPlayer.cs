using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    public Image imgRole;
    public Button button;
    public Button butSkill1;
    public Button butSkill2;
    public Button butSkill3;
    public Text textPhase;
    public Text textPlayerId;
    public Text textPlayerName;
    public Text textCardCount;
    public Text textMessageRedCount;
    public Text textMessageBlueCount;
    public Text textMessageBlackCount;
    public Slider slider;
    public GameObject goSelect;
    public GameObject goTurnOn;
    public GameObject goMessageOn;
    public UIPlayerColor playerColor;
    public GameObject goDie;
    public GameObject goLose;
    public GameObject goLock;
    public GameObject goJin;
    public GameObject goCard;
    public GameObject goMask;


    private int playerId;
    private List<Button> skillButtons;
    private string roleDes = "";
    // Start is called before the first frame update
    void Awake()
    {
        skillButtons = new List<Button>() { butSkill1, butSkill2, butSkill3 };
        butSkill1.gameObject.SetActive(false);
        butSkill2.gameObject.SetActive(false);
        butSkill3.gameObject.SetActive(false);
        goMask.SetActive(false);
        goDie.SetActive(false);
        goLock.SetActive(false);
        goJin.SetActive(false);
        slider.gameObject.SetActive(false);
        button.onClick.AddListener(() => { GameManager.Singleton.SelectPlayerId = playerId; });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(int id)
    {
        this.gameObject.SetActive(true);
        goSelect.SetActive(false);
        goTurnOn.SetActive(false);
        goMessageOn.SetActive(false);
        playerId = id;
        textPlayerName.text = GameManager.Singleton.players[id].playerName;
        textPlayerId.text = LanguageUtils.IdToWord(id);
        textCardCount.text = "0";
        playerColor.SetColor(GameManager.Singleton.players[id].playerColor);
        RefreshMessage();
        InitRole();
        InitSkill();
    }

    public void InitRole()
    {
        roleDes = "";
        roleDes += GameManager.Singleton.players[playerId].role.name + "\n";
        if (!(GameManager.Singleton.players[playerId].role is Role_Unknown))
        {
            foreach (var skill in GameManager.Singleton.players[playerId].role.skills)
            {
                roleDes += skill.Des;
            }
        }

        if (GameManager.Singleton.players[playerId].role.isBack)
        {
            imgRole.gameObject.SetActive(false);
            return;
        }
        var path = GameManager.Singleton.players[playerId].role.spritName;
        if (!string.IsNullOrEmpty(path))
        {
            Sprite sprite = Resources.Load<Sprite>("Images/role/" + path);
            imgRole.sprite = sprite;
            imgRole.gameObject.SetActive(true);
        }
    }

    public void InitSkill()
    {
        if (playerId != GameManager.SelfPlayerId)
        {
            return;
        }
        for (int i = 0; i < GameManager.Singleton.players[playerId].role.skills.Count; i++)
        {
            SkillBase skill = GameManager.Singleton.players[playerId].role.skills[i];
            skillButtons[i].gameObject.SetActive(true);
            skillButtons[i].transform.Find("Text").GetComponent<Text>().text = skill.name;
            skillButtons[i].onClick.RemoveAllListeners();
            skillButtons[i].onClick.AddListener(() => { skill.PrepareUse(); });
            skillButtons[i].interactable = skill.canUse;
        }
    }

    public void RefreshSkillState()
    {
        for (int i = 0; i < GameManager.Singleton.players[playerId].role.skills.Count; i++)
        {
            SkillBase skill = GameManager.Singleton.players[playerId].role.skills[i];
            skillButtons[i].gameObject.SetActive(true);
            skillButtons[i].transform.Find("Text").GetComponent<Text>().text = skill.name;
            skillButtons[i].interactable = skill.canUse;
        }
    }

    public void OnClickMessage()
    {
        GameManager.Singleton.gameUI.ShowPlayerMessageInfo(playerId);
    }

    public void RefreshCardCount()
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
    }
    public void OnDrawCard(int count)
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
        if (playerId != GameManager.SelfPlayerId)
        {
            if (count > 1)
            {
                for (int i = 1; i < count; i++)
                {
                    var go = GameObject.Instantiate(goCard, goCard.transform.parent);
                    Destroy(go, 0.5f);
                }
            }
            animator.SetTrigger("DrawCards");
        }
    }

    public void UseCard(CardFS cardInfo = null)
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
    }
    public void SendCard()
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
    }
    public void Discard(List<CardFS> cards)
    {
        textCardCount.text = "" + GameManager.Singleton.players[playerId].cardCount;
    }

    public void RefreshMessage()
    {
        textMessageRedCount.text = "" + GameManager.Singleton.players[playerId].GetMessageCount(CardColorEnum.Red);
        textMessageBlueCount.text = "" + GameManager.Singleton.players[playerId].GetMessageCount(CardColorEnum.Blue);
        textMessageBlackCount.text = "" + GameManager.Singleton.players[playerId].GetMessageCount(CardColorEnum.Black);
    }

    public void OnTurn(bool isTurn)
    {
        goTurnOn.SetActive(isTurn);
    }

    public void OnMessageTurnTo(bool isTurn)
    {
        goMessageOn.SetActive(isTurn);
    }

    public void OnWaiting(int seconds)
    {
        //if (playerId == 0) Debug.LogError(seconds);

        if (seconds <= 0)
        {
            StopAllCoroutines();
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
            StartCoroutine(ShowSlider(seconds));
        }
    }

    public void OnDie(bool loseGame)
    {
        goDie.SetActive(true);
        goLose.SetActive(loseGame);
    }

    private IEnumerator ShowSlider(int seconds)
    {
        float total = seconds;
        float secondsF = seconds;
        while (secondsF > 0)
        {
            slider.value = secondsF / total;
            secondsF = secondsF - 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        //if (playerId == 0) Debug.LogError("0000000000000000");
        slider.gameObject.SetActive(false);
    }

    public void OnSelect(bool select)
    {
        goSelect.SetActive(select);
    }

    public void SetLock(bool isLock)
    {
        goLock.SetActive(isLock);
    }
    public void SetJinBi(bool isLock)
    {
        goJin.SetActive(isLock);
    }

    public void SetPhase(PhaseEnum phase)
    {
        textPhase.text = LanguageUtils.GetPhaseName(phase);
    }

    public void HidePhase()
    {
        textPhase.text = "";
    }

    internal void SetBanClick(bool v)
    {
        if (!goDie.activeSelf)
        {
            goMask.SetActive(v);
        }
    }

    public void OnTurnBack(bool isBack)
    {
        //textPlayerId.text = GameManager.Singleton.players[playerId].role.name == "" ? "" + playerId + "�����" : GameManager.Singleton.players[playerId].role.name;
        animator.SetTrigger("TurnBack");
        if (isBack)
        {
            imgRole.gameObject.SetActive(false);
        }
        else
        {
            InitRole();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_STANDALONE
        GameUI.ShowDesInfo(roleDes, eventData.position);
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
        showInfoCorout = StartCoroutine(ShowInfo());
        //GameUI.ShowDesInfo(roleDes, eventData.position);
#endif
    }

    private IEnumerator ShowInfo()
    {
        yield return new WaitForSeconds(0.2f);
        GameUI.ShowDesInfo(roleDes, transform.position);
    }

}
