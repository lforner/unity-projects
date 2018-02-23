using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class SeekToAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public PlayableDirector director;

    private MySlider slider;

    // Use this for initialization
    void Start()
    {
        slider = GetComponent<MySlider>();
        slider.maxValue = audioSource.clip.samples;
        slider.onValueChanged.AddListener(delegate { SeekToTime(slider.value); });
    }

    private void SeekToTime(float time)
    {
        audioSource.timeSamples = Mathf.FloorToInt(time) % audioSource.clip.samples;
        //float ratio = time / audioSource.clip.samples;
        //director.time = ratio * director.duration % director.duration;
        director.time = time / audioSource.clip.frequency % audioSource.clip.samples;
    }

    // Update is called once per frame
    void Update()
    {
        slider.MySet(audioSource.timeSamples, false);
    }
}
