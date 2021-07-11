using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CarEventListener : MonoBehaviour
{
	[SerializeField] private BeginEventProcess beginEventProcess;   // イベントカットシーン開始の通知を取得する用
	[SerializeField] private EventGoal eventGoal;                   // イベントのゴール通知を取得する用
	[SerializeField] private EventMainpart eventMainpart;           // イベント本体の開始地点情報を取得する用

	private CarController carController;

	private const float WaitFadeOutTime = 2.0f;                     // イベントカットシーン開始時のフェードアウトが終わるまで待つ時間 

	void Start()
    {
		carController = GetComponent<CarController>();

		beginEventProcess.BeginEvent
			.Where(beginEventFlag => beginEventFlag)
			.Subscribe(BeginEventFlag =>
			{
				StopPlayer();

				// イベント本体の開始地点へ遷移
				StartCoroutine(SetPlayerTransform(eventMainpart.EventStartPoint));
			});

		eventGoal
			.OnTriggerEnterAsObservable()
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				StopPlayer();
			});

    }

	// プレイヤーの速度を 0 にする
	private void StopPlayer()
	{
		carController.GetCarRigidbody.isKinematic = true;

		carController.FrontLeftWheelBreakTorque = Mathf.Infinity;
		carController.FrontRightWheelBreakTorque = Mathf.Infinity;

		carController.GetCarRigidbody.isKinematic = false;
	}

	private IEnumerator SetPlayerTransform(Transform moveTo)
	{
		yield return new WaitForSeconds(WaitFadeOutTime);

		transform.position = moveTo.position;
		transform.rotation = moveTo.rotation;

		Debug.Log("Transform complited!");
	}
}
