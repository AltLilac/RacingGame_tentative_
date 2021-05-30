using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CarAudio : MonoBehaviour
{
    [SerializeField] private AudioClip accelClip;

	[SerializeField] private float defaultPitch = 0.25f;		// 初期のピッチ値
	[SerializeField] private float pitchMultiplier = 100.0f;	// ピッチシフト倍率

    private AudioSource _accelSound;


    void Start()
    {
        _accelSound = SetUpAudioSource(accelClip);

    }

    void Update()
    {
        // 速度に応じてエンジン音のピッチを上げる
        _accelSound.pitch = defaultPitch + CarController.GetSpeed() / pitchMultiplier;
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
