using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleSelect : MonoBehaviour
{
    public Text textTitle;
    public RoleSelectItem roleItem;
    public Button butSuer;

    private Dictionary<role, RoleSelectItem> items = new Dictionary<role, RoleSelectItem>();

    public role roleSelect;
    private void Awake()
    {
        roleItem.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if(items.Count > 0)
        {
            foreach(var item in items)
            {
                Destroy(item.Value.gameObject);
            }
            items.Clear();
        }
    }
    public void Show(PlayerColorEnum playerColor, SecretTaskEnum secretTask, List<role> roles)
    {
        gameObject.SetActive(true);

        string s = "你的身份是" + LanguageUtils.GetIdentityName(playerColor);
        if (playerColor == PlayerColorEnum.Green)
        {
            s += ",机密任务是" + LanguageUtils.GetTaskName(secretTask);
        }
        textTitle.text = s;

        foreach (var role in roles)
        {
            if(items.ContainsKey(role))
            {
                continue;
            }

            RoleSelectItem item = GameObject.Instantiate<RoleSelectItem>(roleItem, roleItem.transform.parent);
            item.gameObject.SetActive(true);
            item.SetRole(role, () => { OnSelectRole(role); });
            //items[role] = item;
            items.Add(role, item);
        }
        butSuer.interactable = false;
    }

    private void OnSelectRole(role role)
    {
        if (items.ContainsKey(roleSelect))
        {
            items[roleSelect].OnSelect(false);
        }
        items[role].OnSelect(true);
        roleSelect = role;
        butSuer.interactable = true;
    }

    public void OnClickSure()
    {
        if (items.ContainsKey(roleSelect))
        {
            ProtoHelper.SendSelectRole(roleSelect);
        }
        else
        {
            GameManager.Singleton.gameUI.ShowInfo("请选择一个角色");
        }
    }
}
