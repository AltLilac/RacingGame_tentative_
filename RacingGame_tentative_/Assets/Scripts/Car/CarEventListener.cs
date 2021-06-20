using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CarEventListener : MonoBehaviour
{
	[SerializeField] private BeginEventProcess beginEventProcess;	// イベントカットシーン開始の通知を取得する用
	[SerializeField] private EventMainpart eventMainpart;           // イベント本体の開始地点情報を取得する用

	private const float WaitFadeOutTime = 3.0f;                     // イベントカットシーン開始時のフェードアウトが終わるまで待つ時間 

	void Start()
    {
		// イベント開始の通知を受け取ったら
		beginEventProcess.BeginEvent
			.Where(beginEventFlag => beginEventFlag)
			.Subscribe(BeginEventFlag =>
			{
				// イベント本体の開始地点へ遷移
				StartCoroutine(SetPlayerTransform(eventMainpart.EventStartPoint));
			});
    }

	private IEnumerator SetPlayerTransform(Transform moveTo)
	{
		yield return new WaitForSeconds(WaitFadeOutTime);

		transform.position = moveTo.position;
		transform.rotation = moveTo.rotation;

		Debug.Log("Transform complited!");
	}
}
