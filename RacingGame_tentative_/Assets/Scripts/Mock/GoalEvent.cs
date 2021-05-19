using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GoalEvent : MonoBehaviour
{
	void Start()
	{
		this.OnTriggerEnterAsObservable()
			.Where(col => col.gameObject.GetComponent<CarController>())
			.First()
			.Subscribe(_ =>
			{
				Debug.Log("ゴール！");
			});
	}
}
