using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	void Start()
	{
		GetComponent<Image>().DOFade(1f, .3f);
	}

	public void Hide()
	{
		GetComponent<Image>().DOFade(0f, .3f)
			.OnComplete(() => Destroy(gameObject));
	}
}
