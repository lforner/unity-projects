using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PauseAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public PlayableDirector director;

    private Button button;

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { pauseAudio(); });
    }

    private void pauseAudio()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
        else
            audioSource.UnPause();

        //if (director.state == PlayState.Playing)
        //    director.Pause();
        //else
        //    director.Play();

        Playable playable = director.playableGraph.GetRootPlayable(0);
        if (playable.GetSpeed() != 0)
            playable.SetSpeed(0);
        else
            playable.SetSpeed(1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
