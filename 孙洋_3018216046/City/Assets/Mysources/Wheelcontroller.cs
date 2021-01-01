using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Wheelcontroller : MonoBehaviour
{ 
    public bool PlayingAudio { get; private set; }


    private AudioSource m_AudioSource;
   
    private WheelCollider m_WheelCollider;
    // Start is called before the first frame update
    void Start()
    {
        

        //m_WheelCollider = GetComponent<WheelCollider>();
        m_AudioSource = GetComponent<AudioSource>();
        PlayingAudio = false;

    }

    public void PlayAudio()
    {
        m_AudioSource.Play();
        PlayingAudio = true;
    }


    public void StopAudio()
    {
        m_AudioSource.Stop();
        PlayingAudio = false;
    }
}
