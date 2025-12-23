using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public SerializedDictionary<string, AudioSource> audioSource;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAudio(string soundName)
    {
        if (audioSource[soundName].isPlaying) return;

        audioSource[soundName].Play();
        Debug.Log("Play Sound: " + soundName);
    }

    public void PlayAudioOnce(string soundName)
    {
        if (audioSource[soundName].isPlaying) return;

        audioSource[soundName].PlayOneShot(audioSource[soundName].clip);
        Debug.Log("Play Sound: " + soundName);
    }

    public void PlayAudioForce(string soundName)
    {
        audioSource[soundName].Play();
    }

    public void StopAudio(string soundName)
    {
        if (!audioSource[soundName].isPlaying) return;
        audioSource[soundName].Stop();
    }

    public void AllAudioStop()
    {
        foreach(var source in audioSource)
        {
            source.Value.Stop();
        }
    }
}
