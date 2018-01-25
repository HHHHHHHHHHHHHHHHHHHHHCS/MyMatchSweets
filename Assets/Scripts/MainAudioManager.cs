using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip clearSweetAudio;

    public MainAudioManager Init()
    {
        return this;
    }

    public void PlayClearSweetAudio()
    {
        AudioSource.PlayClipAtPoint(clearSweetAudio, Vector3.zero);
    }
}
