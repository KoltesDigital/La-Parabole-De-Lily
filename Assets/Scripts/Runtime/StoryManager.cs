using DG.Tweening;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
	public static StoryManager instance { get; private set; }

	public ChapterData firstChapter;
	private ChapterData currentChapter;

	public bool inHome = true;
	public CanvasGroup homeGroup;

	public float idleDuration = 60f;
	public float idleNotificationDuration = 50f;
	private float activityNotificationTime = 0f;
	private float activityThresholdTime = 0f;
	private Vector2 previousMousePosition;
	private Notification activityNotification = null;

	public float escPressDuration = 3f;
	private float escPressThresholdTime;
	private Notification escPressNotification = null;

	private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		previousMousePosition = Input.mousePosition;

		homeGroup.DOFade(1f, .3f);
	}

	public void OpenChapter(ChapterData chapter)
	{
		currentChapter = chapter;

		if (currentChapter == null)
		{
			return;
		}

		AudioManager.instance.Process(currentChapter);

		if (currentChapter is SequenceChapterData)
		{
			var data = currentChapter as SequenceChapterData;
			SequenceManager.instance.Play(data);
		}

		if (currentChapter is GameChapterData)
		{
			var data = currentChapter as GameChapterData;
			ShapeManager.instance.Play(data);
		}
	}

	public void StartStory()
	{
		homeGroup.DOFade(0f, .3f)
			.OnComplete(() =>
			{
				inHome = false;
				homeGroup.gameObject.SetActive(false);
				OpenChapter(firstChapter);
			});
	}

	public void StopStory()
	{
		if (!inHome)
		{
			inHome = true;

			AudioManager.instance.Stop();
			SequenceManager.instance.Stop();
			ShapeManager.instance.Stop();

			homeGroup.gameObject.SetActive(true);
			homeGroup.DOFade(1f, .3f);

			NotificationManager.Hide(ref activityNotification);
			NotificationManager.Hide(ref escPressNotification);
		}
	}

	private void Update()
	{
		if (!Mathf.Approximately(Vector2.Distance(previousMousePosition, Input.mousePosition), 0f) || !(currentChapter is GameChapterData))
		{
			activityNotificationTime = Time.time + idleNotificationDuration;
			activityThresholdTime = Time.time + idleDuration;

			NotificationManager.Hide(ref activityNotification);
		}
		previousMousePosition = Input.mousePosition;

		if (!inHome)
		{
			if (activityNotification == null && Time.time >= activityNotificationTime)
			{
				activityNotification = NotificationManager.instance.Show("Move The Mouse");
			}

			if (Time.time >= activityThresholdTime)
			{
				StopStory();
			}

			if (Input.GetKeyDown(KeyCode.Escape) && escPressNotification == null)
			{
				escPressNotification = NotificationManager.instance.Show("Keep Pressing Esc");
				escPressThresholdTime = Time.time + escPressDuration;
			}

			if (Input.GetKey(KeyCode.Escape))
			{
				if (Time.time >= escPressThresholdTime)
				{
					StopStory();
				}
			}
			else
			{
				NotificationManager.Hide(ref escPressNotification);
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			}
		}
	}
}
