using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicAudio;
    public AudioSource soundAudio;
    public Toggle toggleMusic;
    private static SoundManager soundManager;

    private void Awake()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        int isOn = PlayerPrefs.GetInt("music", 1);
        musicAudio.enabled = isOn == 1;
        toggleMusic.isOn = isOn == 1;
    }

    public void SaveState(bool isOn)
    {
        PlayerPrefs.SetInt("music", isOn ? 1 : 0);
    }

    public static void Play()
    {
        soundManager.musicAudio.Play();
    }

    public static void PlaySoundCard(CardNameEnum cardNameEnum, bool isWoman = false)
    {
        string sex = isWoman ? "woman" : "man";
        string path = "Music/Sound/" + cardNameEnum.ToString() + "_" + sex;
        soundManager.soundAudio.clip = Resources.Load<AudioClip>(path);
        soundManager.soundAudio.time = 2;
        soundManager.soundAudio.Play();
        //soundManager.soundAudio.PlayScheduled(1);
    }

    public static void PlaySound(string path)
    {
        soundManager.soundAudio.clip = Resources.Load<AudioClip>(path);
        soundManager.soundAudio.time = 0;
        soundManager.soundAudio.Play();
    }

    public static void PlaySoundMessage(bool real, bool isWoman)
    {
        List<string> sounds = null;
        if(real && isWoman)
        {
            sounds = SoundPath.message_woman_real;
        }
        else if(real && !isWoman)
        {
            sounds = SoundPath.message_man_real;
        }
        else if(!real && isWoman)
        {
            sounds = SoundPath.message_woman_unreal;
        }
        else if(!real && !isWoman)
        {
            sounds = SoundPath.message_man_unreal;
        }
        int index = Random.Range(0, sounds.Count);
        soundManager.soundAudio.clip = Resources.Load<AudioClip>(sounds[index]);
        soundManager.soundAudio.time = 0;
        soundManager.soundAudio.Play();
    }

    //int a = 0;
    //public void Test()
    //{        
    //    a++;
    //    switch(a)
    //    {
    //        case 1:
    //            PlaySound(SoundPath.cardout);
    //            break;
    //        case 2:
    //            PlaySound(SoundPath.chooserole);
    //            break;
    //        case 3:
    //            PlaySound(SoundPath.dead);
    //            break;
    //        case 4:
    //            PlaySound(SoundPath.deal);
    //            break;
    //        case 5:
    //            PlaySound(SoundPath.lost);
    //            break;
    //        case 6:
    //            a = 0;
    //            break;
    //    }
    //}

    //public void Test1()
    //{
    //    PlaySoundMessage(true, true);
    //}
}

public static class SoundPath
{
    public static string cardout = "Music/Sound/cardout"; //牌打出去
    public static string chooserole = "Music/Sound/chooserole"; //选择角色
    public static string dead = "Music/Sound/dead"; //死亡
    public static string deal = "Music/Sound/deal"; //发牌
    public static string lost = "Music/Sound/lost"; //游戏失败
    public static string trunrole = "Music/Sound/trunrole"; //翻开角色
    public static string win = "Music/Sound/win"; //游戏胜利

    public static List<string> message_man_real = new List<string>()
    {
        "Music/Sound/info/111",
        "Music/Sound/info/112",
        "Music/Sound/info/113",
    };
    public static List<string> message_man_unreal = new List<string>()
    {
        "Music/Sound/info/131",
        "Music/Sound/info/132",
        "Music/Sound/info/133",
    };
    public static List<string> message_woman_real = new List<string>()
    {
        "Music/Sound/info/011",
        "Music/Sound/info/012",
        "Music/Sound/info/013",
    };
    public static List<string> message_woman_unreal = new List<string>()
    {
        "Music/Sound/info/031",
        "Music/Sound/info/032",
        "Music/Sound/info/033",
    };
}
