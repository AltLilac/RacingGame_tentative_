using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CollisionEventTrigger : MonoBehaviour, IEventTrigger<Collider>
{
	// イベント開始
	private Subject<Collider> _onBeginEvent = new Subject<Collider>();
	public IObservable<Collider> OnBeginEvent
	{
		get { return _onBeginEvent.AsObservable(); }
	}

	void Start()
	{
		// 接触したオブジェクトがCarControllerを保持していたら通知する
		this.OnTriggerEnterAsObservable()
			.Where(col => col.gameObject.GetComponent<CarController>())
			.Subscribe(_onBeginEvent);
	}
}
