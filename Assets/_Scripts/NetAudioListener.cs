using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class NetAudioListener : MonoBehaviour
{

    void Awake()
    {
        _audioListener = GetComponent<AudioListener>();
    }


    private AudioListener _audioListener = null;
}
