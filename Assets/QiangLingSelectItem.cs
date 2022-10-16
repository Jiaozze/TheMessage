using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QiangLingSelectItem : MonoBehaviour
{
    public Toggle toggle;
    public CardNameEnum cardName;
    public QiangLingSelect qiangLingSelect;
    private void OnEnable()
    {
        toggle.isOn = false;
    }
    public void OnToggle(bool isOn)
    {
        qiangLingSelect.OnToggle(cardName, isOn);
    }
}
