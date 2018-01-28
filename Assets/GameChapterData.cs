using System;
using UnityEngine;

[Serializable]
public struct GameSettings
{
	public int shapeRemainder;
	public int shapeCount;
}

[CreateAssetMenu()]
public class GameChapterData : ChapterData
{
	public GameSettings settings;
	public ChapterData nextChapterOnFailure;
	public ChapterData nextChapterOnMiddle;
	public ChapterData nextChapterOnSuccess;
}
