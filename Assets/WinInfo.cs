using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinInfo : MonoBehaviour
{
    public WinInfoItem winInfoItem;

    private List<WinInfoItem> items = new List<WinInfoItem>();
    // Start is called before the first frame update
    void Start()
    {
        winInfoItem.gameObject.SetActive(false);
    }

    public void Show(List<int> playerId, List<int> winners, List<PlayerColorEnum> playerColers, List<SecretTaskEnum> playerTasks)
    {
        gameObject.SetActive(true);
        ClearItems();
        foreach (var winnerId in winners)
        {
            WinInfoItem item = GameObject.Instantiate(winInfoItem, winInfoItem.transform.parent);
            item.Show(winnerId, true, playerColers[winnerId], playerTasks[winnerId], playerId.Contains(winnerId));
        }

        foreach (var kv in GameManager.Singleton.players)
        {
            int id = kv.Value.playerId;
            if (!winners.Contains(id))
            {
                WinInfoItem item = GameObject.Instantiate(winInfoItem, winInfoItem.transform.parent);
                item.Show(id, false, playerColers[id], playerTasks[id], playerId.Contains(id));
            }
        }
    }

    private void ClearItems()
    {
        if (items.Count > 0)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                Destroy(items[i].gameObject);
            }
            items.Clear();
        }
    }

    public void Restart()
    {
        GameManager.Singleton.Reset();
        NetWork.Dispose();
        SceneManager.LoadScene("TheMessage");
    }
}
