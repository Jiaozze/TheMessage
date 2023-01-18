using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBreak : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartHeartBreak());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator StartHeartBreak()
    {
        yield return null;
        while(true)
        {
            yield return new WaitForSeconds(20);
            ProtoHelper.SendHeart();
        }
    }
}
