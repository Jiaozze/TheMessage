using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

public class RoomUI : MonoBehaviour
{
    public string ip = "127.0.0.1";

    public GameObject goRoomInfo;
    public GameObject goRoomItem;
    public GameObject goLoginButton;
    public GameObject goRecord;
    public Records RecordList;
    public InputField recordId;
    public InputField playerName;
    public InputField playerPassword;
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
        playerPassword.text = PlayerPrefs.GetString("PlayerPassword", "");
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
        PlayerPrefs.SetString("PlayerPassword", playerPassword.text);
        if (playerPassword.text == "") playerPassword.text = "nopassword"; 
        NetWork.Init(ip, () =>
        {
            ProtoHelper.SendAddRoom(playerName.text, SystemInfo.deviceUniqueIdentifier, StringToMD5(playerPassword.text));
        });
        Debug.Log(StringToMD5(playerPassword.text));
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
                go.transform.Find("WinInfo").GetComponent<Text>().text = "" + " ʤ����" + winCounts[i]; // + " �ܳ�����" + gameCount;
                textInfo.text += names[i] + "���뷿��\n";
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
        items[index].transform.Find("WinInfo").GetComponent<Text>().text = "" + " ʤ����" + winCount; // + " �ܳ�����" + gameCount;
        textInfo.text += name + "���뷿��\n";
    }

    public void OnPlayerLeave(int index)
    {
        string name = items[index].transform.Find("Text").GetComponent<Text>().text;
        items[index].transform.Find("Text").GetComponent<Text>().text = "";
        items[index].transform.Find("WinInfo").GetComponent<Text>().text = "";
        textInfo.text += name + "�뿪����\n";
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
        textOnlineCount.text = "��ǰ����������" + count;
    }

    internal void OnReceiveOrder(List<pb_order> orders)
    {
        string s = "";
        foreach (var order in orders)
        {
            s += order.Name + " �� " + new DateTime((long)(order.Time * 10000000 + 621355968000000000)).ToString() + "\n";
        }
        textOrder.text = s;
        goOrder.SetActive(true);
    }

    internal void OnReceiveRecords(List<string> records)
    {
        RecordList.Refresh(records);
    }

    public static string StringToMD5(string str)
    {
        //�õ��侲̬����������MD5����
        MD5 md5 = MD5.Create();
        //�ֽ�����
        byte[] strbuffer = Encoding.Default.GetBytes(str);
        //���ܲ������ֽ�����
        strbuffer = md5.ComputeHash(strbuffer);
        string strNew = "";
        foreach (byte item in strbuffer)
        {
            //���ֽ�������Ԫ�ظ�ʽ����ƴ��
            strNew += item.ToString("x2");
        }
        return strNew;
    }
}
