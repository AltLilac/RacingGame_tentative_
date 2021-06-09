using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;
using UniRx.Triggers;

public class EventCutscene : MonoBehaviour
{
	[SerializeField] private BeginEventProcess beginEventProcess;
	[SerializeField] private GameObject[] mainGameObjects;			// カットシーン再生中非表示にするゲームオブジェクト
	[SerializeField] private GameObject[] cutsceneObjects;			// カットシーン再生中表示するゲームオブジェクト

	private PlayableDirector _director;

	void Start()
    {
		// カットシーン用のオブジェクトを非表示にしておく
		foreach (var objects in cutsceneObjects)
		{
			objects.SetActive(false);
		}

		_director = GetComponent<PlayableDirector>();
		_director.played += PlayedDirectorProcess;
		_director.stopped += StopedDirectorProcess;

		// BeginEventProcess クラスの _beginEvent が true になったら
		beginEventProcess.BeginEvent
			.Where(beginEventFlag => beginEventFlag)
			.Subscribe(beginEventFlag =>
			{
				// タイムラインスタート
				Debug.Log("Start timeline!");

				_director.Play();
			});
    }

	// タイムラインの再生時の処理
	private void PlayedDirectorProcess(PlayableDirector obj)
	{
		// メインゲームのオブジェクトを全て非表示
		foreach (var objects in mainGameObjects)
		{
			objects.SetActive(false);
		}

		// カットシーン用のオブジェクトを全て表示する
		foreach (var objects in cutsceneObjects)
		{
			objects.SetActive(true);
		}
	}

	// タイムライン終了時の処理
	private void StopedDirectorProcess(PlayableDirector obj)
	{
		// インスペクタービューでセットした UI の非表示を解除する
		foreach (var objects in mainGameObjects)
		{
			objects.SetActive(true);
		}

		// カットシーン用のオブジェクトを全て非表示
		foreach(var objects in cutsceneObjects)
		{
			objects.SetActive(false);
		}
	}
}
