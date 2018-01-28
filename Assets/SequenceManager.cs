using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SequenceManager : MonoBehaviour
{
	static public SequenceManager instance { get; private set; }

	private void Awake()
	{
		instance = this;
	}

	private IEnumerator Show(SequenceElement element)
	{
		yield return new WaitForSeconds(element.fromTime);

		if (element.sprite != null)
		{
			ImageManager.instance.Show(element.sprite);
		}

		if (element.text != "")
		{
			TextManager.instance.Show(element.text);
		}

		yield return new WaitForSeconds(element.toTime - element.fromTime);

		if (element.sprite != null)
		{
			ImageManager.instance.Hide();
		}

		if (element.text != "")
		{
			TextManager.instance.Hide();
		}
	}

	private IEnumerator Complete(SequenceChapterData data)
	{
		yield return new WaitForSeconds(data.duration);

		StoryManager.instance.OpenChapter(data.nextChapter);
	}

	public void Play(SequenceChapterData data)
	{
		foreach (var element in data.elements)
		{
			StartCoroutine(Show(element));
		}

		StartCoroutine(Complete(data));
	}
}
