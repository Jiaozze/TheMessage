using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoleSelectItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject goSelect;
    public Image imgRole;
    public Button button;
    private RoleBase roleBase;

    private void Awake()
    {
        goSelect.gameObject.SetActive(false);
    }
    public void SetRole(role role, UnityAction p)
    {
        roleBase = AllRoles.GetRole(role, 0);

        var path = roleBase.spritName;
        if (!string.IsNullOrEmpty(path))
        {
            Sprite sprite = Resources.Load<Sprite>("Images/role/" + path);
            imgRole.sprite = sprite;
        }

        button.onClick.AddListener(p);
    }

    internal void OnSelect(bool v)
    {
        goSelect.SetActive(v);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_STANDALONE
        string info = "";
        info += roleBase.name + "\n";
        if (!(roleBase is Role_Unknown))
        {
            foreach (var skill in roleBase.skills)
            {
                info += skill.Des;
            }
        }
        GameUI.ShowDesInfo(info, eventData.position);
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
        GameUI.HideDesInfo();
        if(showInfoCorout!=null)
        {
            StopCoroutine(showInfoCorout);
        }
#endif
    }

    public void PointerDown()
    {
#if UNITY_ANDROID
        showInfoCorout = StartCoroutine(ShowInfo());
        //GameUI.ShowDesInfo(roleDes, eventData.position);
#endif
    }

    private IEnumerator ShowInfo()
    {
        yield return new WaitForSeconds(0.5f);
        string info = "";
        info += roleBase.name + "\n";
        if (!(roleBase is Role_Unknown))
        {
            foreach (var skill in roleBase.skills)
            {
                info += skill.Des;
            }
        }

        GameUI.ShowDesInfo(info, transform.position);
    }
}
