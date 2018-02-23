using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTime : MonoBehaviour
{
    public AudioSource audioSource;
    
    private Text time;

    // Use this for initialization
    void Start()
    {
        time = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float minutes = Mathf.FloorToInt(audioSource.time / 60f);
        float seconds = Mathf.FloorToInt(audioSource.time % 60f);
        float milliseconds = (audioSource.time % 60f - seconds) * 1000f;
        time.text = string.Format("{0}:{1}:{2}", minutes.ToString("00"), seconds.ToString("00"), milliseconds.ToString("000"));
    }
}
