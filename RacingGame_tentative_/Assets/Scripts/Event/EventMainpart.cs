using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

/*
	TODO: イベントを何回もプレイできるようにする。ゴールだけまだ出てこない。
*/

public class EventMainpart : MonoBehaviour
{
	[SerializeField] private EventCutscene eventCutscene;				// イベントカットシーン終了通知を取得する用
	[SerializeField] private EventGoal eventGoal;						// イベントのゴール通知を取得する用

	[SerializeField] private int defaultTimelimit;						// イベントの制限時間

	private ReactiveProperty<int> _currentTimelimit;
	public int GetCurrentTimelimit => _currentTimelimit.Value;

	[SerializeField] private Transform startPoint;                      // スタート地点
	public Transform EventStartPoint => startPoint;

	[SerializeField] private Transform goalPoint;                       // ゴール地点
	public Transform EventGoalPoint => goalPoint;

	[SerializeField] private TextMeshProUGUI countdownText;             // イベントスタート直前のカウントダウンに使用するテキスト
	[SerializeField] private TextMeshProUGUI notifyStartText;           // カウントダウンが終わったら表示するテキスト
	[SerializeField] private TextMeshProUGUI eventTimelimit;			// イベント中の制限時間を表示する UI
	[SerializeField] private TextMeshProUGUI eventTimelimitInfo;        // 制限時間の上に表示するテキスト
	[SerializeField] private TextMeshProUGUI timeOverText;              // 制限時間超過時に表示するテキスト
	[SerializeField] private TextMeshProUGUI winText;                   // 完走時に表示するテキスト
	[SerializeField] private TextMeshProUGUI clearTimeText;             // クリアタイムを表示するテキスト
	[SerializeField] private TextMeshProUGUI clearTimeInfo;				// クリアタイムの上に表示するテキスト

	// カウントダウンが終了し、イベント本体がスタートしたら通知する RP
	private readonly ReactiveProperty<bool> _isEndCountdown = new ReactiveProperty<bool>(false);
	public IReadOnlyReactiveProperty<bool> EndCountdown => _isEndCountdown;

	private bool _isEndEvent = false;    // イベントが終わっているか

    void Start()
    {
		_currentTimelimit = new ReactiveProperty<int>(defaultTimelimit);

		eventGoal.gameObject.SetActive(false);

		HideUI(countdownText, notifyStartText, eventTimelimit, eventTimelimitInfo, timeOverText, winText, clearTimeText, clearTimeInfo);

		// カットシーンが終わったら
		eventCutscene.EndStartEventCutscene
			.Where(endCutsceneFlag => endCutsceneFlag)
			.Subscribe(endCutsceneFlag =>
			{
				// カウントダウンコルーチンが終了したら
				Observable
					.FromCoroutine(BeginCountdown)
					.Publish()
					.RefCount()
					.Subscribe(_ =>
					{
						// 車の Input のロックを解除
						CarManager.IsCarInputEnabled = true;

						StartCoroutine(DisplayTemporaryText(span: 2.0f, notifyStartText));
						ShowUI(eventTimelimit, eventTimelimitInfo);

						_isEndCountdown.Value = true;

						Debug.Log("start!");
					});
			});

		EndCountdown
			.Where(isEndCountdown => isEndCountdown)
			.Subscribe(isEndcountdown =>
			{
				eventGoal.gameObject.SetActive(true);

				StartCoroutine(DecreaseTimelimit());
			});

		// ゴール地点のコリジョンに触れたら
		eventGoal
			.OnTriggerEnterAsObservable()
			.Where(collider => collider.gameObject.CompareTag("Player"))
			.Subscribe(collider =>
			{
				_isEndCountdown.Value = false;
				_isEndEvent = true;

				// クリアタイムを計算
				clearTimeText.text
					= $"{EventTimelimit.GetMinutesToString(defaultTimelimit - GetCurrentTimelimit)} : {EventTimelimit.GetSecondsToString(defaultTimelimit - GetCurrentTimelimit)}";

				Observable
					.FromCoroutine(EventFinished)
					.Publish()
					.RefCount()
					.Subscribe(_ =>
					{
						Debug.Log("ゴールした");
					});
			});

		// 時間切れになったら
		_currentTimelimit
			.Where(time => time < 0)
			.Subscribe(_ =>
			{
				_isEndEvent = true;

				Observable
					.FromCoroutine(EventTimeOvered)
					.Publish()
					.RefCount()
					.Subscribe(_ =>
					{
						Debug.Log("時間切れ");
					});
			});
	}

	// イベントスタート直前のカウントダウン
	private IEnumerator BeginCountdown()
	{
		if (!countdownText.gameObject.activeSelf)
		{
			ShowUI(countdownText);
		}

		int count = 3;

		while (count >= 0)
		{
			countdownText.text = count.ToString();

			yield return new WaitForSeconds(1.0f);

			count--;
		}

		HideUI(countdownText);
	}

	private IEnumerator DecreaseTimelimit()
	{
		while (_currentTimelimit.Value >= 0 && !_isEndEvent)
		{
			yield return new WaitForSeconds(1.0f);

			_currentTimelimit.Value--;
		}

		// UI の残り時間が -1 になるのを防止する
		if (_currentTimelimit.Value < 0)
		{
			_currentTimelimit.Value = 0;
		}
	}

	private IEnumerator EventTimeOvered()
	{
		float span = 2.0f;
		CarManager.IsCarInputEnabled = false;

		StartCoroutine(DisplayTemporaryText(span: span, timeOverText));

		eventGoal.gameObject.SetActive(false);
		HideUI(eventTimelimit, eventTimelimitInfo);

		// DisplayTemporaryText コルーチンの終了を待つ
		yield return new WaitForSeconds(span);
	}

	private IEnumerator EventFinished()
	{
		float span = 2.0f;
		StartCoroutine(DisplayTemporaryText(span: span, winText, clearTimeText, clearTimeInfo));

		eventGoal.gameObject.SetActive(false);
		HideUI(eventTimelimit, eventTimelimitInfo);

		// DisplayTemporaryText コルーチン終了を待つ
		yield return new WaitForSeconds(span);
	}

	private IEnumerator DisplayTemporaryText(float span, params TextMeshProUGUI[] texts)
	{
		ShowUI(texts);

		yield return new WaitForSeconds(span);

		HideUI(texts);
	}

	private void HideUI(params TextMeshProUGUI[] texts)
	{
		foreach (var objects in texts)
		{
			if (objects.gameObject.activeSelf)
			{
				objects.gameObject.SetActive(false);
			}
		}
	}

	private void ShowUI(params TextMeshProUGUI[] texts)
	{
		foreach (var objects in texts)
		{
			if (!objects.gameObject.activeSelf)
			{
				objects.gameObject.SetActive(true);
			}
		}
	}
}
