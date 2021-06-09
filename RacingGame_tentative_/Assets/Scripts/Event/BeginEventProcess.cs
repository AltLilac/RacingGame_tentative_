using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BeginEventProcess : MonoBehaviour
{
	private bool _inCollison = false;	// イベント開始 UI を出現させるコリジョン内にいるか

	private readonly ReactiveProperty<bool> _beginEvent = new ReactiveProperty<bool>(false);    // イベント開始の通知を送る
	public IReadOnlyReactiveProperty<bool> BeginEvent => _beginEvent;							// イベント購読用


	void Start()
    {
		// イベントコリジョンに触れている場合、プレイヤーに通知する
		this.OnTriggerEnterAsObservable()
			.Where(collider => !_inCollison)
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				_inCollison = true;
				Debug.Log("You can begin this event!");
			});

		// イベントコリジョンを出たら UI を消去
		this.OnTriggerExitAsObservable()
			.Where(collider => _inCollison)
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				_inCollison = false;

				// 開始の是非に関わらず、イベント開始フラグを false に戻しておく
				// TODO: イベント開始の通知を送るプロパティは作成したので、カットシーンを呼ぶ方法を考える
				_beginEvent.Value = false;

				Debug.Log("Exit to event collision");
			});

		// inCollision フラグが true ならイベント開始可能 UI を表示
		this.UpdateAsObservable()
			.Where(_ => _inCollison)
			.Subscribe(_ =>
			{
				Debug.Log("E キーでイベントを開始");
			});

		// イベントコリジョン内で E キーを押したらイベント開始
		this.UpdateAsObservable()
			.Where(_ => _inCollison && Input.GetKeyDown(KeyCode.E))
			.Subscribe(_ =>
			{
				_inCollison = false;
				_beginEvent.Value = true;

				Debug.Log("イベント開始");
			});
    }
}
