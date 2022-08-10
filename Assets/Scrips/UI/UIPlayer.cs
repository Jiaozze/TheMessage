using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public Text textPlayerId;
    public Text textCardCount;

    private int playerId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int id)
    {
        this.gameObject.SetActive(true);
        playerId = id;
        textPlayerId.text = "" + id;
        textCardCount.text = "0";
    }

    public void OnDrawCard(int totalCount, int count)
    {
        textCardCount.text = "" + totalCount;
    }
}
