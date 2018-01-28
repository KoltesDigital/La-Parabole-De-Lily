using System;
using UnityEngine;

[Serializable]
public class SequenceElement
{
	public float fromTime;
	public float toTime;
	public Sprite sprite;
	public string text;
}

[CreateAssetMenu()]
public class SequenceChapterData : ChapterData
{
	public SequenceElement[] elements;
	public float duration;
	public ChapterData nextChapter;
}
