using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public Image image;

	public string language;

	public void OnPointerEnter(PointerEventData eventData)
	{
		image.DOFade(1f, .3f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		image.DOFade(0f, .3f);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		LocalizationManager.instance.language = language;
		StoryManager.instance.StartStory();
	}
}
