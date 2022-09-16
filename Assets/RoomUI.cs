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
            ProtoHelper.SendAddRoom(playerName.text);
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
    public void OnRoomInfo(List<string> names, int index)
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
        foreach(var name in names)
        {
            var go = GameObject.Instantiate(goRoomItem, goRoomItem.transform.parent);
            go.SetActive(true);
            go.transform.Find("Text").GetComponent<Text>().text = name;
            items.Add(go);
        }
        items[index].transform.Find("Image").GetComponent<Image>().color = Color.cyan;
        Debug.Log(items.Count);
    }

    public void OnAddPlayer(string name, int index)
    {
        //Debug.Log(index);
        //Debug.Log(items.Count);
        items[index].transform.Find("Text").GetComponent<Text>().text = name;
    }

    public void OnPlayerLeave(int index)
    {
        items[index].transform.Find("Text").GetComponent<Text>().text = "";
    }
}
