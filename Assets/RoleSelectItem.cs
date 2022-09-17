using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoleSelectItem : MonoBehaviour
{
    public GameObject goSelect;
    public Image imgRole;
    public Button button;
    private void Awake()
    {
        goSelect.gameObject.SetActive(false);
    }
    public void SetRole(role role, UnityAction p)
    {
        RoleBase role1 = AllRoles.GetRole(role, 0);

        var path = role1.spritName;
        if (!string.IsNullOrEmpty(path))
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Images/Role");
            foreach (Sprite sprite in sprites)
            {
                if (sprite.name == path)
                {
                    imgRole.sprite = sprite;
                    break;
                }
            }
        }

        button.onClick.AddListener(p);
    }

    internal void OnSelect(bool v)
    {
        goSelect.SetActive(v);
    }
}
