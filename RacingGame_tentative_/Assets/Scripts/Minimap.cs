using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		// 追従する
		var trans = Camera.main.transform;

		transform.position = trans.position;
		var rot = trans.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(90.0f, rot.y, rot.z);
	}
}
