using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class LocalizationWindow : EditorWindow
{
	[MenuItem("Tools/Localization")]
	public static void ShowWindow()
	{
		GetWindow(typeof(LocalizationWindow), true, "Localization");
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

	private IEnumerable<TAsset> GetAllAssets<TAsset>() where TAsset : UnityEngine.Object
	{
		return AssetDatabase.FindAssets("t:" + typeof(TAsset).Name)
			.Select(guid =>
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				return AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);
			})
			.Where(asset => asset != null);
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

		void UseText(string text)
		{
			usedTexts.Add(text);
			if (!localizedTexts.texts.ContainsKey(text))
			{
				localizedTexts.texts.Add(text, new JsonLocalizedTextData());
			}
		}

		foreach (var sequenceChapterData in GetAllAssets<SequenceChapterData>())
		{
			foreach (var sequenceElement in sequenceChapterData.elements)
			{
				if (sequenceElement.text != "")
				{
					UseText(sequenceElement.text);
				}
			}
		}

		foreach (var extraTextData in GetAllAssets<ExtraTextData>())
		{
			foreach (var text in extraTextData.texts)
			{
				UseText(text);
			}
		}

		foreach (var pair in localizedTexts.texts)
		{
			if (!usedTexts.Contains(pair.Key))
			{
				try
				{
					Debug.LogErrorFormat("Unused text: {0}", pair.Key);
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
