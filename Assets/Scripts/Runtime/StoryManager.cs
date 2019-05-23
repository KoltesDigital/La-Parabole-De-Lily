using UnityEngine;

public class StoryManager : MonoBehaviour
{
	static public StoryManager instance { get; private set; }

	[SerializeField]
	private ChapterData firstChapter;

	private ChapterData currentChapter;

	private void Awake()
	{
		instance = this;
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
			ShapeManager.instance.Initialize(data);
		}
	}

	void Start()
	{
		OpenChapter(firstChapter);
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
