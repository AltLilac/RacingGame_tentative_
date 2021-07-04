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
	[SerializeField] private BeginEventProcess beginEventProcess;	// イベント開始の通知を受け取る用
	[SerializeField] private EventMainpart eventMainpart;			// イベント終了の通知を受け取る用

	[SerializeField] private GameObject[] mainGameObjects;			// カットシーン再生中非表示にするゲームオブジェクト
	[SerializeField] private GameObject[] cutsceneObjects;			// カットシーン再生中表示するゲームオブジェクト

	[SerializeField] private PlayableDirector director;				// アニメーションを再生するタイムラインを管理

	private readonly ReactiveProperty<bool> _endBeginEventCutscene = new ReactiveProperty<bool>(false);	// イベント開始時のカットシーン再生終了を通知する
	public IReadOnlyReactiveProperty<bool> EndBeginEventCutscene => _endBeginEventCutscene;

	void Start()
    {
		ManageActiveSelf(cutsceneObjects, activeState: false);

		beginEventProcess.BeginEvent
			.Where(beginEventFlag => beginEventFlag)
			.Subscribe(beginEventFlag =>
			{
				director.played += PlayedDirectorProcessOnBeginEvent;
				director.stopped += StopedDirectorProcessOnBeginEvent;

				director.Play();
			});

		eventMainpart.EndEvent
			.Where(endEventFlag => endEventFlag)
			.Subscribe(endEventFlag =>
			{
				director.played += PlayedDirectorProcessOnEndEvent;
				director.stopped += StopedDirectorProcessOnEndEvent;

				director.Play();
			});
    }

	// タイムラインの再生時の処理
	private void PlayedDirectorProcessOnBeginEvent(PlayableDirector obj)
	{
		ManageActiveSelf(mainGameObjects, activeState: false);
		ManageActiveSelf(cutsceneObjects, activeState: true);

		// HUD シーンを無効化
		SceneManager.UnloadSceneAsync("HUD");

		// 車の Input をロックする
		CarManager.IsCarInputEnabled = false;
	}

	private void PlayedDirectorProcessOnEndEvent(PlayableDirector obj)
	{
		// HUD シーンを無効化
		SceneManager.UnloadSceneAsync("HUD");

		// 車の Input をロックする
		CarManager.IsCarInputEnabled = false;
	}

	// タイムライン終了時の処理
	private void StopedDirectorProcessOnBeginEvent(PlayableDirector obj)
	{
		ManageActiveSelf(mainGameObjects, activeState: true);
		ManageActiveSelf(cutsceneObjects, activeState: false);

		// HUD シーンを戻す
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);

		// カットシーン終了を通知
		_endBeginEventCutscene.Value = true;
	}

	private void StopedDirectorProcessOnEndEvent(PlayableDirector obj)
	{
		// HUD シーンを戻す
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);

		// 車の Input のロックを解除
		CarManager.IsCarInputEnabled = true;
	}

	private void ManageActiveSelf(GameObject[] gameObjects, bool activeState)
	{
		if (gameObjects != null)
		{
			foreach (var objects in gameObjects)
			{
				objects.SetActive(activeState);
			}
		}
	}
}
