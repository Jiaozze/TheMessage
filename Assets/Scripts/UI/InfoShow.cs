using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoShow : MonoBehaviour
{
    public Text textMidInfo;
    private string infoStr = null;
    private Coroutine hideInfoCorout;

    public void ShowInfo(string info)
    {
        textMidInfo.text = infoStr == null ? info : infoStr + "\n" + info;
        infoStr = info;
        hideInfoCorout = StartCoroutine(HideInfo());
    }

    private IEnumerator HideInfo()
    {
        string s = infoStr;
        yield return new WaitForSeconds(2);
        infoStr = null;
        textMidInfo.text = textMidInfo.text.Replace(s, "");
    }

}
