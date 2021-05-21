using UnityEngine;
using UniRx;

[RequireComponent(typeof(CollisionEventTrigger))]
public class GoalEvent : MonoBehaviour
{
	void Start()
	{
		var trigger = GetComponent<CollisionEventTrigger>();

		trigger.OnBeginEvent
			.First()
			.Subscribe(_ =>
			{
				Debug.Log("ゴール！");
			});
	}
}
