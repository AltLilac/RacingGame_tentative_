using UnityEngine;
using UniRx;

[RequireComponent(typeof(CollisionEventTrigger))]
public class DestinationEvent : MonoBehaviour
{
	[SerializeField]
	private GameObject goalPrefab;

	void Start()
	{
		var trigger = GetComponent<CollisionEventTrigger>();

		trigger.OnBeginEvent
			.First()
			.Subscribe(col =>
			{
				// ゴールオブジェクトの生成
				Instantiate(goalPrefab);
			});
	}
}
