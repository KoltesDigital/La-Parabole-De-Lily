using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class LocalizationManager : MonoBehaviour
{
	public static LocalizationManager instance { get; private set; }

	public LocalizationData localizationData;

	public string language;

	private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		Assert.IsNotNull(localizationData);
	}

	public string GetLocalizedString(string originalText)
	{
		var localizedTextData = localizationData.texts.First(textData => textData.text == originalText);
		var translationData = localizedTextData.translations.FirstOrDefault(data => data.language == language);
		return translationData != null ? translationData.text : originalText;
	}
}
