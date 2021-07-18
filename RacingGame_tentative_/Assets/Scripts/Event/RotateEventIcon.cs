using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class RotateEventIcon : MonoBehaviour
{
	[SerializeField] private float heightRange = 20.0f;		// 変動する縦の値
	[SerializeField] private float rotAngleY = 20.0f;		// 変動する角度

    void Start()
    {
		this.UpdateAsObservable()
			.Subscribe(_ => RotateObject());
    }

	private void RotateObject()
	{
		// y 軸で滑らかに上下移動させる
		{ 
			Vector3 newLocation = transform.position;

			float runningTime = Time.realtimeSinceStartup;
			float deltaHeight = (Mathf.Sin(runningTime + Time.deltaTime) - Mathf.Sin(runningTime));

			newLocation.y += deltaHeight * heightRange;
			transform.position = newLocation;
		}

		// y 軸で回転させ続ける
		{
			Vector3 newRotation = new Vector3(0.0f, rotAngleY, 0.0f);

			transform.Rotate(newRotation);
		}
	}
}
