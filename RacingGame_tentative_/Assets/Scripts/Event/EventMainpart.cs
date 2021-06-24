using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class EventMainpart : MonoBehaviour
{
	[SerializeField] private EventCutscene eventCutscene;				// イベントカットシーン終了通知を取得する用
	[SerializeField] private EventType eventType = EventType.Default;   // イベントの種類

	[SerializeField] private int timelimit = 180;                       // イベントの制限時間
	public int Timelimit => timelimit;

	[SerializeField] private Transform startPoint;                      // スタート地点
	public Transform EventStartPoint => startPoint;

	[SerializeField] private Transform goalPoint;                       // ゴール地点
	public Transform EventGoalPoint => goalPoint;

	[SerializeField] private TextMeshProUGUI countdownText;             // イベントスタート直前のカウントダウンに使用するテキスト
	[SerializeField] private TextMeshProUGUI notifyStartText;           // カウントダウンが終わったら表示するテキスト
	[SerializeField] private TextMeshProUGUI eventTimelimit;			// イベント中の制限時間を表示する UI
	[SerializeField] private TextMeshProUGUI eventTimelimitInfo;		// 制限時間の上に表示するテキスト

	[SerializeField] private GameObject goalSign;						// ゴール地点に出現させる柱状の半透明オブジェクト


	// カウントダウンが終了し、イベント本体がスタートしたら通知する RP
	private readonly ReactiveProperty<bool> _isEndCountdown = new ReactiveProperty<bool>(false);
	public IReadOnlyReactiveProperty<bool> EndCountdown => _isEndCountdown;

	// イベントの種類
	enum EventType
	{
		Default,
		CheckPointTimeAttack,
		NoCheckPointTimeAttack
	};

    void Start()
    {
		goalSign.SetActive(false);

		HideUI(countdownText, notifyStartText, eventTimelimit, eventTimelimitInfo);

		// カットシーンが終わったら
		eventCutscene.EndCutscene
			.Where(endCutsceneFlag => endCutsceneFlag)
			.Subscribe(endCutsceneFlag =>
			{
				// カウントダウンコルーチンが終了したら
				Observable
					.FromCoroutine(BeginCountdown)
					.Publish()	// Hot 変換してコルーチンの複製を防ぐ
					.RefCount()
					.Subscribe(_ =>
					{
						// 車の Input のロックを解除
						CarManager.IsCarInputEnabled = true;

						StartCoroutine(DisplayStartText());

						ShowUI(eventTimelimit, eventTimelimitInfo);

						_isEndCountdown.Value = true;

						Debug.Log("start!");
					});
			});

		EndCountdown
			.Where(isEndCountdown => isEndCountdown)
			.Subscribe(isEndcountdown =>
			{
				//while (isEndcountdown)
				//{
				//	DecreaseTime(timelimit);
				//}
			});

		// チェックポイント無し（スタート地点とゴール地点のみ）イベントの場合
		this.UpdateAsObservable()
			.Where(_ => _isEndCountdown.Value)
			.Where(_ => eventType == EventType.NoCheckPointTimeAttack)
			.Subscribe(_ =>
			{
				SpawnGoalSign();
			});

		// ゴール地点のコリジョンに触れたら
    }

	private void SpawnGoalSign()
	{
		// ゴール地点に大きな目印を出現させる
		goalSign.SetActive(true);
	}

	private IEnumerator DecreaseTime(int time, float span = 1.0f)
	{
		while (time <= 0)
		{
			yield return new WaitForSeconds(span);

			time--;
		}
	}

	// イベントスタート直前のカウントダウン
	private IEnumerator BeginCountdown()
	{
		if (!countdownText.gameObject.activeSelf)
		{
			ShowUI(countdownText);
		}

		int count = 3;

		// カウントダウンでの、1 秒待った後値をデクリメントする機能を別関数にしたい
		countdownText.text = DecreaseTime(count).ToString();

		if (count <= 0)
		{
			HideUI(countdownText);

			yield return null;
		}
	}

	// カウントダウン終了時のテキストを表示させる
	private IEnumerator DisplayStartText()
	{
		ShowUI(notifyStartText);

		yield return new WaitForSeconds(2.0f);

		HideUI(notifyStartText);
	}

	// UI を隠しておく
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

	// UI を出現させる
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
