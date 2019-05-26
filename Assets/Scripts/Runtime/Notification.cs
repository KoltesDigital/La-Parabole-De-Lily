using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Notification : MonoBehaviour
{
	public string text
	{
		set
		{
			GetComponent<Text>().text = value;
		}
	}

	private void Start()
	{
		GetComponent<Text>().DOFade(1f, .3f);
	}

	public void Hide()
	{
		GetComponent<Text>().DOFade(0f, .3f)
			.OnComplete(() => Destroy(gameObject));
	}
}
