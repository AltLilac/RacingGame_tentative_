using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [SerializeField] private AudioClip accelSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = accelSound;
    }

    void Update()
    {

    }
}
