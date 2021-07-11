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
	[SerializeField] private EventGoal eventGoal;					// イベントゴールの通知を受け取る用

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

		eventGoal
			.OnTriggerEnterAsObservable()
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				// 車の Input をロックする
				CarManager.IsCarInputEnabled = false;

				director.played += PlayedDirectorProcessOnEndEvent;
				director.stopped += StopedDirectorProcessOnEndEvent;

				Observable
					.FromCoroutine(DelayCoroutine)
					.Publish()
					.RefCount()
					.Subscribe(_ =>
					{
						director.Play();
					});
		});
	}

	// イベント開始時におけるタイムライン開始の処理
	private void PlayedDirectorProcessOnBeginEvent(PlayableDirector obj)
	{
		ManageActiveSelf(mainGameObjects, activeState: false);
		ManageActiveSelf(cutsceneObjects, activeState: true);

		// HUD シーンを無効化
		SceneManager.UnloadSceneAsync("HUD");

		// 車の Input をロックする
		CarManager.IsCarInputEnabled = false;

		Debug.Log("PlayedDirectorProcessOnBeginEvent");
	}

	// イベント終了時におけるタイムライン開始の処理
	private void PlayedDirectorProcessOnEndEvent(PlayableDirector obj)
	{
		// HUD シーンを無効化
		SceneManager.UnloadSceneAsync("HUD");

		Debug.Log("PlayedDirectorProcessOnEndEvent");
	}

	// イベント開始時におけるタイムライン終了の処理
	private void StopedDirectorProcessOnBeginEvent(PlayableDirector obj)
	{
		ManageActiveSelf(mainGameObjects, activeState: true);
		ManageActiveSelf(cutsceneObjects, activeState: false);

		// HUD シーンを戻す
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);

		// カットシーン終了を通知
		_endBeginEventCutscene.Value = true;

		Debug.Log("StopedDirectorProcessOnBeginEvent");
	}

	// イベント終了時におけるタイムライン終了の処理
	private void StopedDirectorProcessOnEndEvent(PlayableDirector obj)
	{
		// HUD シーンを戻す
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);

		// 車の Input のロックを解除
		CarManager.IsCarInputEnabled = true;

		Debug.Log("StopedDirectorProcessOnEndEvent");
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

	// リザルト UI の表示を待つ
	private IEnumerator DelayCoroutine()
	{
		yield return new WaitForSeconds(4.0f);
	}
}
