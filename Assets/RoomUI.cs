using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    private string ip = "127.0.0.1";

    public GameObject goRoomInfo;
    public GameObject goRoomItem;
    public GameObject goLoginButton;
    public GameObject goRecord;
    public Records RecordList;
    public InputField recordId;
    public InputField playerName;
    public Text textOnlineCount;
    public DatePickerControl datePicker;
    public GameObject goOrder;
    public Text textOrder;
    public RectTransform content;
    public RectTransform rectText;
    public Text textInfo;

    private List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName", "");
        GameManager.Singleton.Init();
        goRoomItem.SetActive(false);
        textInfo.text = "";
    }

    private void Update()
    {
        content.sizeDelta = rectText.sizeDelta;
    }
    private void OnDestroy()
    {
        NetWork.Dispose();
    }
    public void OnClickJoinRoom()
    {
        PlayerPrefs.SetString("PlayerName", playerName.text);
        NetWork.Init(ip, () =>
        {
            ProtoHelper.SendAddRoom(playerName.text, SystemInfo.deviceUniqueIdentifier);
        });
        //ProtoHelper.SendGetRoomInfo();
    }

    public void OnClickRecord()
    {
        NetWork.Init(ip, () => { ProtoHelper.SendGetRecords(); });
        //NetWork.Init(ip, () => { ProtoHelper.SendPlayRecord(recordId.text); });
    }
    public void OnClickRecordById()
    {
        //NetWork.Init(ip, () => { ProtoHelper.SendGetRecords(); });
        NetWork.Init(ip, () => { ProtoHelper.SendPlayRecord(recordId.text); });
        GameManager.Singleton.gameUI.gameObject.SetActive(true);
        GameManager.Singleton.gameUI.goStopRecordBut.SetActive(true);
    }

    public void OnClickSeeOrder()
    {
        ProtoHelper.SendGetOrders();
    }
    public void OnClickAddOrder()
    {
        var t = (datePicker.fecha.Ticks - 621355968000000000) / 10000000;
        //Debug.Log(t);
        //Debug.Log(new DateTime(t * 10000000 + 621355968000000000).ToString());
        ProtoHelper.SendAddOrders((uint)t);
    }

    public void OnClickAddAI()
    {
        ProtoHelper.SendAddAI();
    }
    public void OnClickRemoveAI()
    {
        ProtoHelper.SendRemoveAI();
    }

    public void OnClickRemovePosition()
    {
        ProtoHelper.SendRemovePosion();
    }
    public void OnClickAddPosition()
    {
        ProtoHelper.SendAddPosition();
    }
    public void OnRoomInfo(List<string> names, int index, List<uint> winCounts, List<uint> allCounts)
    {
        goRoomInfo.SetActive(true);
        goLoginButton.SetActive(false);
        goRecord.SetActive(false);
        RecordList.gameObject.SetActive(false);
        if (items.Count > 0)
        {
            foreach (var go in items)
            {
                Destroy(go);
            }
        }
        items.Clear();
        for (int i = 0; i < names.Count; i++)
        {
            var go = GameObject.Instantiate(goRoomItem, goRoomItem.transform.parent);
            go.SetActive(true);
            go.transform.Find("Text").GetComponent<Text>().text = names[i];
            if (!string.IsNullOrEmpty(names[i]))
            {
                //uint p = allCounts[i] > 0 ? winCounts[i] * 1000 / allCounts[i] : 0;
                //float f = (float)p / 10;
                go.transform.Find("WinInfo").GetComponent<Text>().text = "" + " 胜场：" + winCounts[i]; // + " 总场数：" + gameCount;
                textInfo.text += names[i] + "加入房间\n";
            }
            else
            {
                go.transform.Find("WinInfo").GetComponent<Text>().text = "";
            }
            items.Add(go);
        }
        items[index].transform.Find("Image").GetComponent<Image>().color = Color.cyan;
        Debug.Log(items.Count);
    }

    public void OnAddPlayer(string name, int index, uint winCount, uint gameCount)
    {
        items[index].transform.Find("Text").GetComponent<Text>().text = name;
        //uint p = gameCount > 0 ? winCount * 1000 / gameCount : 0;
        //float f = (float)p / 10;
        items[index].transform.Find("WinInfo").GetComponent<Text>().text = "" + " 胜场：" + winCount; // + " 总场数：" + gameCount;
        textInfo.text += name + "加入房间\n";
    }

    public void OnPlayerLeave(int index)
    {
        string name = items[index].transform.Find("Text").GetComponent<Text>().text;
        items[index].transform.Find("Text").GetComponent<Text>().text = "";
        items[index].transform.Find("WinInfo").GetComponent<Text>().text = "";
        textInfo.text += name + "离开房间\n";
    }

    public void OnAddPositon()
    {
        var go = GameObject.Instantiate(goRoomItem, goRoomItem.transform.parent);
        go.SetActive(true);
        go.transform.Find("Text").GetComponent<Text>().text = "";
        go.transform.Find("WinInfo").GetComponent<Text>().text = "";
        items.Add(go);
    }

    public void OnRemovePosition(int index)
    {
        var go = items[index];
        items.RemoveAt(index);
        Destroy(go);
    }

    public void SetOnlineCount(int count)
    {
        textOnlineCount.text = "当前在线人数：" + count;
    }

    internal void OnReceiveOrder(List<pb_order> orders)
    {
        string s = "";
        foreach (var order in orders)
        {
            s += order.Name + " ： " + new DateTime((long)(order.Time * 10000000 + 621355968000000000)).ToString() + "\n";
        }
        textOrder.text = s;
        goOrder.SetActive(true);
    }

    internal void OnReceiveRecords(List<string> records)
    {
        RecordList.Refresh(records);
    }
}
