using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Minimap : MonoBehaviour
{
	// メインカメラから離すyの距離
	private readonly float CAMERA_OFFSET = 300.0f;

	void Start()
	{
		// メインカメラから一定距離離れた角度に追従させる
		this.UpdateAsObservable()
			.Select(_ => Camera.main.transform)
			.Subscribe(trans =>
			{
				transform.position = new Vector3(trans.position.x, trans.position.y + CAMERA_OFFSET, trans.position.z);
				var rot = trans.rotation.eulerAngles;
				transform.rotation = Quaternion.Euler(90.0f, rot.y, rot.z);
			});
	}
}
