using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NotificationManager : MonoBehaviour
{
  [Tooltip("Notification Prefab")]
  public GameObject notificationBox;
  public Text text;

  private float hideAtTime = 0;
  float hideDurationSeconds = 1f;

  void Start()
  {
    var initial = "Your \"Explorer\" Submarine is damaged! Main engines are non-functional. Find the \"Fusion Core\" to fix it! To move use WSAD. Once it is done, you will be able to progress to the surface. Be aware of hazards! Good Luck!";

    activate(initial, 10);
  }

  void Update()
  {
    if (hideAtTime > 0 && Time.unscaledTime > hideAtTime)
    {
      StartCoroutine(hideNotification());
      hideAtTime = 0;
    }
  }

  IEnumerator hideNotification()
  {
    float t = 0f;

    while (t < hideDurationSeconds)
    {
      t += Time.unscaledDeltaTime;
      setOpacity(Mathf.Lerp(1f, 0f, t / hideDurationSeconds));
      yield return null;
    }
  }

  void setOpacity(float opacity)
  {
    notificationBox.GetComponent<CanvasGroup>().alpha = opacity;
    // var renderer = notificationBox.transform.Find("Speaker").GetComponent<RawImage>();
    // var c = renderer.material.color;
    // c.a = opacity;
    // renderer.material.color = c;
  }

  public void activate(string value, float hideAfterSeconds)
  {
    setOpacity(1);
    text.text = value;
    hideAtTime = Time.unscaledTime + hideAfterSeconds;
  }
}
