using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [SerializeField] private AudioClip accelClip;

    private AudioSource _accelSound;

    void Start()
    {
        _accelSound = SetUpAudioSource(accelClip);
    }

    void Update()
    {
        // 速度に応じてエンジン音のピッチを上げる
        _accelSound.pitch = 0.25f + CarController.GetSpeed() / 100.0f;
    }

    // AudioClip のセットアップ
    private AudioSource SetUpAudioSource(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 1;
        source.loop = true;
        source.Play();

        return source;
    }
}
