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

	[SerializeField] private Transform startPoint;                      // スタート地点
	public Transform EventStartPoint => startPoint;

	[SerializeField] private Transform goalPoint;                       // ゴール地点
	public Transform EventGoalPoint => goalPoint;

	[SerializeField] private TextMeshProUGUI countdownText;             // イベントスタート直前のカウントダウンに使用するテキスト
	[SerializeField] private TextMeshProUGUI notifyStartText;           // カウントダウンが終わったら表示するテキスト

	[SerializeField] private GameObject goalSign;						// ゴール地点に出現させる柱状の半透明オブジェクト

	private bool isInEvent = false;										// イベント中かどうか

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

		HideUI(countdownText, notifyStartText);

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

						isInEvent = true;

						Debug.Log("start!");
					});
			});

		// チェックポイント無し（スタート地点とゴール地点のみ）イベントの場合
		this.UpdateAsObservable()
			.Where(_ => isInEvent)
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

	// カウントダウン終了時のテキストを表示させる
	private IEnumerator DisplayStartText()
	{
		ShowUI(notifyStartText);

		yield return new WaitForSeconds(2.0f);

		HideUI(notifyStartText);
	}

	private void HideObjects<T>(params T[] gameObjects) where T : Object
	{
		foreach (var objects in gameObjects)
		{
			
		}
	}

	// UI を隠しておく
	private void HideUI(params TextMeshProUGUI[] texts)
	{
		foreach (var objects in texts)
		{
			objects.gameObject.SetActive(false);
		}
	}

	// UI を出現させる
	private void ShowUI(params TextMeshProUGUI[] texts)
	{
		foreach (var objects in texts)
		{
			objects.gameObject.SetActive(true);
		}
	}
}
