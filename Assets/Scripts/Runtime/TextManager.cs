using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
	public static TextManager instance { get; private set; }

	private Text text;
	private string requestedStr;

	private void Awake()
	{
		instance = this;
		text = GetComponent<Text>();
	}

	public void Show(string str)
	{
		requestedStr = str;
		str = LocalizationManager.instance.GetLocalizedString(str);

		if (text.color.a > 0f)
		{
			text.DOFade(0f, .3f)
				.OnComplete(() =>
				{
					text.text = str;
					text.DOFade(1f, .3f);
				});
			return;
		}

		text.text = str;
		text.DOFade(1f, .3f);
	}

	public void Hide(string str)
	{
		if (requestedStr == str)
		{
			text.DOFade(0f, .3f);
		}
	}

	public void ForceHide()
	{
		text.DOFade(0f, .3f)
			.OnComplete(() =>
			{
				requestedStr = "";
				text.text = "";
			});
	}
}
