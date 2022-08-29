using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    private static SoundManager soundManager;

    private void Awake()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void Play()
    {
        soundManager.audioSource.Play();
    }
}
