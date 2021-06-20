using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UniRx;
using UniRx.Triggers;

public class EventCutscene : MonoBehaviour
{
	[SerializeField] private BeginEventProcess beginEventProcess;

	[SerializeField] private GameObject[] mainGameObjects;			// カットシーン再生中非表示にするゲームオブジェクト
	[SerializeField] private GameObject[] cutsceneObjects;			// カットシーン再生中表示するゲームオブジェクト

	private PlayableDirector _director;

	private readonly ReactiveProperty<bool> _endCutscene = new ReactiveProperty<bool>(false);	// カットシーンの再生終了を通知する
	public IReadOnlyReactiveProperty<bool> EndCutscene => _endCutscene;

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

		// イベント開始の通知を受け取ったら
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

		// HUD シーンを無効化
		SceneManager.UnloadSceneAsync("HUD");

		// 車の Input をロックする
		CarManager.IsCarInputEnabled = false;
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

		// HUD シーンを戻す
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);

		// カットシーン終了を通知
		_endCutscene.Value = true;
	}
}
