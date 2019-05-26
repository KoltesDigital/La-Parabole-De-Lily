using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance { get; private set; }

	private AudioSource source;

	private void Awake()
	{
		instance = this;
		source = GetComponent<AudioSource>();
	}

	private void FadeIn(AudioClip clip)
	{
		source.clip = clip;
		source.Play();
		source.DOFade(1f, 1f);
	}

	private Tween FadeOut(float duration)
	{
		return source.DOFade(0f, duration);
	}

	public void Stop(float duration = .3f)
	{
		FadeOut(duration)
			.OnComplete(() =>
			{
				source.Stop();
				source.clip = null;
			});
	}

	public void Process(ChapterData data)
	{
		if (data.playMusic != null)
		{
			if (source.isPlaying)
			{
				FadeOut(.3f)
					.OnComplete(() => FadeIn(data.playMusic));
			}
			else
			{
				FadeIn(data.playMusic);
			}
		}
		else if (data.stopMusic)
		{
			Stop(5f);
		}
	}
}
