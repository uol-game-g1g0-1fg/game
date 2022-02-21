using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullableItem : MonoBehaviour
{
  public Rigidbody[] bodies;
  public GameObject rockToPull;
  private bool isPulling = false;
  private Collider arm;
  private Vector3 armOffset;

  // Start is called before the first frame update
  void Start()
  {
    bodies = GetComponentsInChildren<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    if (arm && isPulling)
      rockToPull.transform.position = arm.transform.position + armOffset;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "MechArm")
    {
      Debug.Log("Pulling!...");

      isPulling = true;
      arm = other;
      armOffset = rockToPull.transform.position - arm.transform.position;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.tag == "MechArm")
    {
      Debug.Log("Pulling!...");

      isPulling = false;

      foreach (Rigidbody body in bodies)
      {
        body.isKinematic = false;
      }
    }
  }
}
