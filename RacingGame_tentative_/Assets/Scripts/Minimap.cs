using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		// 追従する
		var trans = Camera.main.transform;

		transform.position = new Vector3(trans.position.x, trans.position.y + 300.0f, trans.position.z);
		var rot = trans.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(90.0f, rot.y, rot.z);
	}
}
