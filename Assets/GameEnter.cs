using UnityEngine;
using UnityEngine.UI;

public class GameEnter : MonoBehaviour
{
    public InputField Input;
    public static byte[] test;
    private string ip = "127.0.0.1";
    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.Singleton.Init();
        //CardFactory.Init();
        Input.onEndEdit.AddListener((s) =>
        {
            ip = s;
        });
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        NetWork.Dispose();
    }

    public void OnClickStart()
    {
        NetWork.Init(ip);
    }

}
