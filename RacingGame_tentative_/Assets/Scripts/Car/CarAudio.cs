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

    void Start()
    {
        var accelSound = SetUpAudioSource(accelClip);

		// 車の Input が有効だったら
		this.UpdateAsObservable()
			.Where(_ => CarManager.IsCarInputEnabled)
			.Select(_ => accelSound.volume = 1)
			.Subscribe(_ =>
			{
				// 速度に応じてエンジン音のピッチを上げる
				accelSound.pitch = defaultPitch + CarController.GetSpeed() / pitchMultiplier;
			});

		// 車の Input が無効だったら
		this.UpdateAsObservable()
			.Where(_ => !CarManager.IsCarInputEnabled)
			.Select(_ => accelSound.volume = 0)	// エンジン音をミュート
			.Subscribe(_ =>
			{

			});
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
