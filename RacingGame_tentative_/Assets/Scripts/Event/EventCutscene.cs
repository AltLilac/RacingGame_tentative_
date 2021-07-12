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
	[SerializeField] private BeginEventProcess beginEventProcess;   // イベント開始の通知を受け取る用
	[SerializeField] private EventGoal eventGoal;                   // イベントゴールの通知を受け取る用

	[SerializeField] private GameObject[] mainGameObjects;          // カットシーン再生中非表示にするゲームオブジェクト
	[SerializeField] private GameObject[] cutsceneObjects;          // カットシーン再生中表示するゲームオブジェクト

	// アニメーションを再生するタイムラインを管理
	[SerializeField] private PlayableDirector beginEventCutsceneDirector;   
	[SerializeField] private PlayableDirector endEventCutsceneDirector;		

	private readonly ReactiveProperty<bool> _endStartEventCutscene = new ReactiveProperty<bool>(false); // イベント開始時のカットシーン再生終了を通知する
	public IReadOnlyReactiveProperty<bool> EndStartEventCutscene => _endStartEventCutscene;

	private readonly ReactiveProperty<bool> _endGoalEventCutscene = new ReactiveProperty<bool>(false);   // イベント終了時のカットシーン再生終了を通知する
	public IReadOnlyReactiveProperty<bool> EndGoalEventCutscene => _endGoalEventCutscene;

	void Start()
	{
		ManageActiveSelf(cutsceneObjects, activeState: false);

		beginEventProcess.BeginEvent
			.Where(beginEventFlag => beginEventFlag)
			.Subscribe(beginEventFlag =>
			{
				beginEventCutsceneDirector.played += PlayedDirectorProcessOnBeginEvent;
				beginEventCutsceneDirector.stopped += StoppedDirectorProcessOnBeginEvent;

				beginEventCutsceneDirector.Play();
			});

		eventGoal
			.OnTriggerEnterAsObservable()
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				beginEventCutsceneDirector.played -= PlayedDirectorProcessOnBeginEvent;
				beginEventCutsceneDirector.stopped -= StoppedDirectorProcessOnBeginEvent;

				// 車の Input をロックする
				CarManager.IsCarInputEnabled = false;

				endEventCutsceneDirector.played += PlayedDirectorProcessOnEndEvent;
				endEventCutsceneDirector.stopped += StoppedDirectorProcessOnEndEvent;

				Observable
					.FromCoroutine(DelayCoroutine)
					.Publish()
					.RefCount()
					.Subscribe(_ =>
					{
						endEventCutsceneDirector.Play();
					});
			});

		_endGoalEventCutscene
			.Where(endGoalEventCutscene => endGoalEventCutscene)
			.Subscribe(endGoalEventCutscene =>
			{
				endEventCutsceneDirector.played -= PlayedDirectorProcessOnEndEvent;
				endEventCutsceneDirector.stopped -= StoppedDirectorProcessOnEndEvent;
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
	private void StoppedDirectorProcessOnBeginEvent(PlayableDirector obj)
	{
		ManageActiveSelf(mainGameObjects, activeState: true);
		ManageActiveSelf(cutsceneObjects, activeState: false);

		// HUD シーンを戻す
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);

		// カットシーン終了を通知
		_endStartEventCutscene.Value = true;

		Debug.Log("StopedDirectorProcessOnBeginEvent");
	}

	// イベント終了時におけるタイムライン終了の処理
	private void StoppedDirectorProcessOnEndEvent(PlayableDirector obj)
	{
		// HUD シーンを戻す
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);

		// 車の Input のロックを解除
		CarManager.IsCarInputEnabled = true;


		_endStartEventCutscene.Value = false;

		// カットシーン終了を通知
		_endGoalEventCutscene.Value = true;

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
