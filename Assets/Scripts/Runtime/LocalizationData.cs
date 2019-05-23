using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonLocalizedTextData
{
	public Dictionary<string, string> translations = new Dictionary<string, string>();
}

public class JsonLocalizationData
{
	public Dictionary<string, JsonLocalizedTextData> texts = new Dictionary<string, JsonLocalizedTextData>();
}

[Serializable]
public class TranslationData
{
	public string language;
	public string text;
}

[Serializable]
public class LocalizedTextData
{
	public string text;
	public TranslationData[] translations;
}

[CreateAssetMenu]
public class LocalizationData : ScriptableObject
{
	public LocalizedTextData[] texts;
}
