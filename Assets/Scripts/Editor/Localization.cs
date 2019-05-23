using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class Localization : EditorWindow
{
	[MenuItem("Tools/Localization")]
	public static void ShowWindow()
	{
		GetWindow(typeof(Localization), true, "Localization");
	}

	private TextAsset jsonAsset;
	private LocalizationData unityAsset;

	private void OnGUI()
	{
		GUILayout.BeginVertical();

		jsonAsset = EditorGUILayout.ObjectField("JSON Asset", jsonAsset, typeof(TextAsset), false) as TextAsset;
		unityAsset = EditorGUILayout.ObjectField("Unity Asset", unityAsset, typeof(LocalizationData), false) as LocalizationData;

		GUI.enabled = jsonAsset != null
			&& unityAsset != null
			&& AssetDatabase.GetAssetPath(jsonAsset).EndsWith(".json");

		if (GUILayout.Button("Process"))
		{
			Process();
		}

		GUILayout.EndVertical();
	}

	private void Process()
	{
		var localizationAssetPath = AssetDatabase.GetAssetPath(jsonAsset);

		var localizedTexts = JsonConvert.DeserializeObject<JsonLocalizationData>(jsonAsset.text);
		if (localizedTexts == null)
		{
			Debug.LogError("Asset is not a valid JSON file.");
			return;
		}

		var usedTexts = new HashSet<string>();

		var guids = AssetDatabase.FindAssets("t:" + typeof(SequenceChapterData).Name);
		foreach (var guid in guids)
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var sequenceChapterData = AssetDatabase.LoadAssetAtPath<SequenceChapterData>(assetPath);
			if (sequenceChapterData != null)
			{
				foreach (var sequenceElement in sequenceChapterData.elements)
				{
					if (sequenceElement.text != "")
					{
						usedTexts.Add(sequenceElement.text);
						if (!localizedTexts.texts.ContainsKey(sequenceElement.text))
						{
							localizedTexts.texts.Add(sequenceElement.text, new JsonLocalizedTextData());
						}
					}
				}
			}

		}

		foreach (var pair in localizedTexts.texts)
		{
			if (!usedTexts.Contains(pair.Key))
			{
				try
				{
					pair.Value.translations.Add("WARNING", "UNUSED");
				}
				catch (ArgumentException)
				{
				}
			}
		}

		var newContent = JsonConvert.SerializeObject(localizedTexts, Formatting.Indented);
		File.WriteAllText(localizationAssetPath, newContent);

		unityAsset.texts = usedTexts.Select(text =>
		{
			var textData = localizedTexts.texts[text];
			return new LocalizedTextData
			{
				text = text,
				translations = textData.translations.Select(pair =>
				{
					return new TranslationData
					{
						language = pair.Key,
						text = pair.Value,
					};
				}).ToArray(),
			};
		}).ToArray();

		EditorUtility.SetDirty(unityAsset);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}