using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinInfo : MonoBehaviour
{
    public WinInfoItem winInfoItem;

    private List<WinInfoItem> items = new List<WinInfoItem>();
    // Start is called before the first frame update
    void Start()
    {
        winInfoItem.gameObject.SetActive(false);
    }
    
    public void Show(int playerId, List<int> winners, List<PlayerColorEnum> playerColers, List<SecretTaskEnum> playerTasks)
    {
        gameObject.SetActive(true);
        ClearItems();
        foreach(var winnerId in winners)
        {
            WinInfoItem item = GameObject.Instantiate(winInfoItem);
            item.Show(winnerId, true, playerColers[winnerId], playerTasks[winnerId], winnerId == playerId);
        }

        foreach(var kv in GameManager.Singleton.players)
        {
            int id = kv.Value.playerId;
            if (!winners.Contains(id))
            {
                WinInfoItem item = GameObject.Instantiate(winInfoItem);
                item.Show(id, false, playerColers[id], playerTasks[id], id == playerId);
            }
        }
    }

    private void ClearItems()
    {
        if(items.Count > 0)
        {
            for(int i = items.Count - 1; i >=0; i--)
            {
                Destroy(items[i].gameObject);
            }
            items.Clear();
        }
    }
}
