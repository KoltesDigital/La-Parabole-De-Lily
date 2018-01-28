using System;
using UnityEngine;

[Serializable]
public struct SequenceElement
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
}
