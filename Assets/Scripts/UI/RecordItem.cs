using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordItem : MonoBehaviour
{
    public Text text;
    public Button button;
    public Transform transGrid;

    public void SetInfo(string info, Dictionary<string, string> records)
    {
        text.text = info;
        foreach(var name_id in records)
        {
            var but = GameObject.Instantiate(button, transGrid);
            but.gameObject.SetActive(true);
            but.transform.Find("Text").GetComponent<Text>().text = name_id.Key;
            but.onClick.AddListener(() => {
                ProtoHelper.SendPlayRecord(name_id.Value);
                GameManager.Singleton.gameUI.gameObject.SetActive(true);
                GameManager.Singleton.gameUI.goStopRecordBut.SetActive(true);
            });
        }
    }
}
