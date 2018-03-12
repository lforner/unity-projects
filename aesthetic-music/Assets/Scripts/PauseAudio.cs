using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PauseAudio : MonoBehaviour
{
    public GameObject GenerationPlaceHolder;
    public PlayableDirector director;

    Button button;
    AudioSource audioSource;
    GenerateMeshes generationScript;

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { pauseAudio(); });
        audioSource = GenerationPlaceHolder.GetComponent<AudioSource>();
        generationScript = GenerationPlaceHolder.GetComponent<GenerateMeshes>();
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

        generationScript.isPlaying = !generationScript.isPlaying;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
