using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
	static public ImageManager instance { get; private set; }

	private Image image;

	private void Awake()
	{
		instance = this;
		image = GetComponent<Image>();
	}

	public void Show(Sprite sprite)
	{
		image.sprite = sprite;
		image.DOFade(1f, .3f);
	}

	public void Hide(Sprite sprite)
	{
		if (image.sprite == sprite)
		{
			image.DOFade(0f, .3f);
		}
	}
}
