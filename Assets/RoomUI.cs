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
    public InputField recordId;
    public InputField playerName;
    public Text textOnlineCount;

    private List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName", "");
        GameManager.Singleton.Init();
        goRoomItem.SetActive(false);
    }

    private void OnDestroy()
    {
        NetWork.Dispose();
    }
    public void OnClickJoinRoom()
    {
        PlayerPrefs.SetString("PlayerName", playerName.text);
        NetWork.Init(ip, () => {
            ProtoHelper.SendAddRoom(playerName.text, SystemInfo.deviceUniqueIdentifier);
        });
        //ProtoHelper.SendGetRoomInfo();
    }

    public void OnClickRecord()
    {       
        NetWork.Init(ip, () =>{ ProtoHelper.SendPlayRecord(recordId.text); });        
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
        if(items.Count > 0)
        {
            foreach(var go in items)
            {
                Destroy(go);
            }
        }
        items.Clear();
        for (int i = 0; i < names.Count; i ++)
        {
            var go = GameObject.Instantiate(goRoomItem, goRoomItem.transform.parent);
            go.SetActive(true);
            go.transform.Find("Text").GetComponent<Text>().text = names[i];
            if(!string.IsNullOrEmpty(names[i]))
            {
                //uint p = allCounts[i] > 0 ? winCounts[i] * 1000 / allCounts[i] : 0;
                //float f = (float)p / 10;
                go.transform.Find("WinInfo").GetComponent<Text>().text = "" + " 胜场：" + winCounts[i]; // + " 总场数：" + gameCount;
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
    }

    public void OnPlayerLeave(int index)
    {
        items[index].transform.Find("Text").GetComponent<Text>().text = "";
        items[index].transform.Find("WinInfo").GetComponent<Text>().text = "";
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
}
