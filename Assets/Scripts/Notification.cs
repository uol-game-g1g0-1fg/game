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

  private NotificationManager notificationManager;

  void Start()
  {
    print("Registered notification");
    notificationManager = GameObject.Find("/UI Manager").GetComponent<NotificationManager>();

    if (type == NotificationType.Time)
    {
      print("time based");
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
    if (other.tag != "Player") return;
    print("triggered" + other.tag);

    notificationManager.activate(text, hideAfterSeconds);
  }
}