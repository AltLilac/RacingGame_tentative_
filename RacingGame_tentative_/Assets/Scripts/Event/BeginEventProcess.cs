using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BeginEventProcess : MonoBehaviour
{
	private bool inCollison = false;

    void Start()
    {
		// イベントコリジョンに触れている場合、プレイヤーに通知する
		this.OnTriggerEnterAsObservable()
			.Where(collider => !inCollison)
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				inCollison = true;
				Debug.Log("You can begin this event!");
			});

		// イベントコリジョンを出たら UI を消去
		this.OnTriggerExitAsObservable()
			.Where(collider => inCollison)
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				inCollison = false;
				Debug.Log("Exit to event collision");
			});

		// inCollision フラグが true ならイベント開始可能 UI を表示
		this.UpdateAsObservable()
			.Where(_ => inCollison)
			.Subscribe(_ =>
			{
				Debug.Log("E キーでイベントを開始");
			});

		// イベントコリジョン内で E キーを押したらイベント開始
		this.UpdateAsObservable()
			.Where(_ => inCollison && Input.GetKeyDown(KeyCode.E))
			.Subscribe(_ =>
			{
				inCollison = false;
				Debug.Log("イベント開始");
			});
    }
}
