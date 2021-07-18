using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EventGoal : MonoBehaviour
{
	public IObservable<Collider> OnTriggerEnterAsObservable()
	{
		return ObservableTriggerExtensions.OnTriggerEnterAsObservable(this);
	}
}
