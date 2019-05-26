using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Shape : MonoBehaviour
{
	public int type;

	public Vector2 position
	{
		get
		{
			return GetComponent<RectTransform>().anchoredPosition;
		}
		set
		{
			GetComponent<RectTransform>().anchoredPosition = value;
		}
	}

	public Sprite sprite
	{
		set
		{
			GetComponent<Image>().sprite = value;
		}
	}

	private void Start()
	{
		GetComponent<Image>().DOFade(1f, .3f);
	}

	public void Hide()
	{
		GetComponent<Image>().DOFade(0f, .3f)
			.OnComplete(() => Destroy(gameObject));
	}
}
