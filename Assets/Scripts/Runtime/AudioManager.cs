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

	private Tween Stop()
	{
		return source.DOFade(0f, 5f);
	}

	private void Play(AudioClip clip)
	{
		source.clip = clip;
		source.Play();
		source.DOFade(1f, 1f);
	}

	public void Process(ChapterData data)
	{
		if (data.playMusic != null)
		{
			if (source.isPlaying)
			{
				Stop()
					.OnComplete(() => Play(data.playMusic));
			}
			else
			{
				Play(data.playMusic);
			}
		}
		else if (data.stopMusic)
		{
			Stop()
				.OnComplete(() =>
				{
					source.Stop();
					source.clip = null;
				});
		}
	}
}
