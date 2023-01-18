using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Records : MonoBehaviour
{
    public RecordItem recordItem;
    public Transform transParent;
    private List<RecordItem> items = new List<RecordItem>();

    public void Refresh(List<string> records)
    {
        gameObject.SetActive(true);
        foreach(var item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();

        Dictionary<string, Dictionary<string, string>> dic_time_s = new Dictionary<string, Dictionary<string, string>>();
        foreach(var record in records)
        {
            var ss = record.Split('-');
            
            string key = record.Substring(0, record.Length - 9); // -i-******
            if(!dic_time_s.ContainsKey(key))
            {
                dic_time_s.Add(key, new Dictionary<string, string>());
            }
            int id = int.Parse(ss[ss.Length - 2]);
            string name = ss[id + 6]; // 2022-10-05-06-24-01-程小蝶-王田香-连鸢-韩梅-鄭文先-0-urpr52
            dic_time_s[key].Add(name, ss[ss.Length - 1]);
        }

        foreach(var kv in dic_time_s)
        {
            var item = GameObject.Instantiate(recordItem, transParent);
            item.SetInfo(kv.Key, kv.Value);
            item.gameObject.SetActive(true);
            items.Add(item);
        }
    }
}
