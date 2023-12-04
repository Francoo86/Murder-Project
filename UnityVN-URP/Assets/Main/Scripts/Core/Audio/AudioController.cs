using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    //Como es un controlador entonces usamos singleton.
    public static AudioController Instance {  get; private set; }

    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;


    private void Awake()
    {
        if (Instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else {
            DestroyImmediate(gameObject);
            return;
        }
    }
}
