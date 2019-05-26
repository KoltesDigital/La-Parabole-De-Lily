using UnityEngine;
using UnityEngine.Assertions;

public class NotificationManager : MonoBehaviour
{
	public static NotificationManager instance { get; private set; }

	public GameObject notificationPrefab;

	private void Awake()
	{
		instance = this;
	}

	public Notification Show(string str)
	{
		str = LocalizationManager.instance.GetLocalizedString(str);

		var instance = Instantiate(notificationPrefab, transform);

		var notification = instance.GetComponentInChildren<Notification>();
		Assert.IsNotNull(notification);
		notification.text = str;

		return notification;
	}

	public static void Hide(ref Notification notification)
	{
		if (notification != null)
		{
			notification.Hide();
			notification = null;
		}
	}
}
