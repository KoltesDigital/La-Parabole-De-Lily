using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
	public static SequenceManager instance { get; private set; }

	private List<Coroutine> coroutines = new List<Coroutine>();

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

		if (element.toTime >= 0f)
		{
			yield return new WaitForSeconds(element.toTime - element.fromTime);

			if (element.sprite != null)
			{
				ImageManager.instance.Hide(element.sprite);
			}

			if (element.text != "")
			{
				TextManager.instance.Hide(element.text);
			}
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
			coroutines.Add(StartCoroutine(Show(element)));
		}

		coroutines.Add(StartCoroutine(Complete(data)));
	}

	public void Stop()
	{
		foreach (var coroutine in coroutines)
		{
			StopCoroutine(coroutine);
		}

		coroutines.Clear();

		ImageManager.instance.ForceHide();
		TextManager.instance.ForceHide();
	}
}
