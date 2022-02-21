using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NotificationType
{
  Trigger, Time
}

public class Notification : MonoBehaviour
{
  public NotificationType type;
  public string text;
  [Tooltip("For Time type")]
  public float timeDelaySeconds;

  public float hideAfterSeconds = 10;

  bool hasActivated = false;
  private NotificationManager notificationManager;

  void Start()
  {
    notificationManager = GameObject.Find("/UI Manager").GetComponent<NotificationManager>();

    if (type == NotificationType.Time)
    {
      StartCoroutine(ActivateAfterDelay());
    }
  }

  IEnumerator ActivateAfterDelay()
  {
    yield return new WaitForSeconds(timeDelaySeconds);

    notificationManager.activate(text, hideAfterSeconds);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (hasActivated || other.tag != "Player") return;
    hasActivated = true;

    notificationManager.activate(text, hideAfterSeconds);
  }
}