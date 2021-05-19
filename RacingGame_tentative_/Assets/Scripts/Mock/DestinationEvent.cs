using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DestinationEvent : MonoBehaviour
{
	[SerializeField]
	private GameObject goalPrefab;

	void Start()
	{
		this.OnTriggerEnterAsObservable()
			.Select(col => col.gameObject)
			.Where(go => go.GetComponent<CarController>())
			.First()
			.Subscribe(go =>
			{
				// ゴールオブジェクトの生成
				Instantiate(goalPrefab);
			});
	}
}
