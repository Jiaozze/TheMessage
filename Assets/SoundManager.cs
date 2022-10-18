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

    public static void PlaySound(CardNameEnum cardNameEnum, bool isWoman = false)
    {
        string sex = isWoman ? "woman" : "man";
        string path = "Music/Sound/" + cardNameEnum.ToString() + "_" + sex;
        soundManager.soundAudio.clip = Resources.Load<AudioClip>(path);
        soundManager.soundAudio.time = 2;
        soundManager.soundAudio.Play();
        //soundManager.soundAudio.PlayScheduled(1);
    }
}
