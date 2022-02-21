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

  private bool hasShownPlantMessage = false;
  private bool hasShownPickupCoreMessage = false;
  private bool hasShownTreasureMessage = false;

  void Start()
  {
    var initial = "Your Submarine is damaged and cannot move. Find the Fusion Core to fix it. Use WSAD keys to move, G to pick up items and F to fire your harpoon";

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
  }

  public void activate(string value, float hideAfterSeconds)
  {
    setOpacity(1);
    text.text = value;
    hideAtTime = Time.unscaledTime + hideAfterSeconds;
  }

  public void OnPlantProjectileFired()
  {
    if (hasShownPlantMessage) return;
    hasShownPlantMessage = true;

    activate("Watch out! Some entities are unfriendly and can cause damage to your vessel", 10);
  }

  public void OnEnableCore()
  {
    if (hasShownPickupCoreMessage) return;
    hasShownPickupCoreMessage = true;

    activate("Your Submarine now has a Fusion Core installed and is able to ascend to the surface!", 10);
  }

  public void OnPickupTreasure()
  {
    if (hasShownTreasureMessage) return;
    hasShownTreasureMessage = true;

    activate("Nice find! Looks like you found some Treasure. Pick these up to increase your Game score", 10);
  }
}
