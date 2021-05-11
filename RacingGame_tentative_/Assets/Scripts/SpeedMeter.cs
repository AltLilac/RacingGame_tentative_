using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedMeter : MonoBehaviour {
	[SerializeField]
	private float _speed = 0.0f;

	// [SerializeField]
	private TextMeshProUGUI _speedText;

	// Start is called before the first frame update
	void Start() {
		_speedText = GetComponentInChildren<TextMeshProUGUI>();
	}

	// Update is called once per frame
	void Update() {
		if (_speed > 1000) {
			_speed = 999;
		}
		// スピードメーターの数値を更新する
		_speedText.text = Math.Floor(_speed).ToString();
	}
}
