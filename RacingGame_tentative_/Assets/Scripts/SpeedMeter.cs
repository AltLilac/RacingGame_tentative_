using System;
using TMPro;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class SpeedMeter : MonoBehaviour
{
	void Start()
	{
		var speedText = GetComponentInChildren<TextMeshProUGUI>();

		// スピードメーターの数値を更新する
		this.UpdateAsObservable()
			.Select(_ => CarController.GetSpeed())
			.Select(spd => Math.Floor(spd))
			.Select(spd => spd > 1000 ? 999 : spd)
			.Subscribe(spd =>
			{
				speedText.text = spd.ToString();
			});
	}
}
