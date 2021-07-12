using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BeginEventProcess : MonoBehaviour
{
	[SerializeField] private EventCutscene eventCutscene;   // イベント終了時のカットシーン再生終了通知を受け取る用

	// イベントコリジョン内に触れていたら、イベント開始可能なことを示すテキスト
	[SerializeField] private GameObject[] onEventCollisionText;

	private bool _inCollison = false;	// イベント開始 UI を出現させるコリジョン内にいるか

	private readonly ReactiveProperty<bool> _beginEvent = new ReactiveProperty<bool>(false);    // イベント開始の通知を送る
	public IReadOnlyReactiveProperty<bool> BeginEvent => _beginEvent;                           // イベント購読用

	void Start()
    {
		ManageText(showFlag: false);

		// イベントコリジョンに触れている場合、プレイヤーに通知する
		this.OnTriggerEnterAsObservable()
			.Where(collider => !_inCollison)
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				_inCollison = true;
			});

		// イベントコリジョンを出たら UI を消去
		this.OnTriggerExitAsObservable()
			.Where(collider => _inCollison)
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				_inCollison = false;

				ManageText(showFlag: false);

				// 開始の是非に関わらず、イベント開始フラグを false に戻しておく
				_beginEvent.Value = false;

				Debug.Log("Exit to event collision");
			});

		// inCollision フラグが true ならイベント開始可能 UI を表示
		this.UpdateAsObservable()
			.Where(_ => _inCollison)
			.Subscribe(_ =>
			{
				ManageText(showFlag: true);
			});

		// イベントコリジョン内で E キーを押したらイベント開始
		this.UpdateAsObservable()
			.Where(_ => _inCollison && Input.GetKeyDown(KeyCode.E))
			.Subscribe(_ =>
			{
				_inCollison = false;
				_beginEvent.Value = true;

				ManageText(showFlag: false);

				Debug.Log("イベント開始");

				gameObject.SetActive(false);
			});

		eventCutscene.EndGoalEventCutscene
			.Where(endGoalEventCutscene => endGoalEventCutscene)
			.Subscribe(endGoalEventCutscene =>
			{
				_beginEvent.Value = false;

				gameObject.SetActive(true);
			});
    }

	// イベント開始可能なことを通知するテキストを制御する
	private void ManageText(bool showFlag)
	{
		foreach (var objects in onEventCollisionText)
		{
			objects.SetActive(showFlag);
		}
	}
}
