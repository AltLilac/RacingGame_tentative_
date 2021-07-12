using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using UniRx.Triggers;

public class EventTimelimit : MonoBehaviour
{
	[SerializeField] private EventCutscene eventCutscene;	// カットシーン終了時の通知を購読する用
	[SerializeField] private EventMainpart eventMainpart;   // カウントダウン終了時の通知を購読する用
	
    void Start()
    {
		var timelimitUI = GetComponent<TextMeshProUGUI>();

		eventCutscene.EndStartEventCutscene
			.Where(isEndCutscene => isEndCutscene)
			.Subscribe(isEndcutscene =>
			{
				timelimitUI.text = $"{GetMinutesToString(eventMainpart.GetCurrentTimelimit)}:{GetSecondsToString(eventMainpart.GetCurrentTimelimit)}";
			});

		// 取得した時間を更新し続ける
		this.UpdateAsObservable()
			.Where(_ => eventMainpart.EndCountdown.Value)
			.Subscribe(_ =>
			{
				timelimitUI.text = $"{GetMinutesToString(eventMainpart.GetCurrentTimelimit)}:{GetSecondsToString(eventMainpart.GetCurrentTimelimit)}";
			});
    }

	static public string GetMinutesToString(int time)
	{
		// 10 分以下だったら 0 を先頭に挿入して返す
		if ((time / 60) >= 10)
		{
			return (time / 60).ToString();
		}
		else
		{
			return "0" + (time / 60).ToString();
		}
	}

	static public string GetSecondsToString(int time)
	{
		// 10 秒以下だったら 0 を先頭に挿入して返す
		if ((time % 60) >= 10)
		{
			return (time % 60).ToString();
		}
		else
		{
			return "0" + (time % 60).ToString();
		}
	}
}
