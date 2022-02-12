using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Camera Settings")] 
    [SerializeField, Range(1, 5)] float cameraSphereCastRadius = 3f;
    int numCollidersInModel;
    Camera mainCamera;
    public GameObject[] virtualCameras;
    
    void Awake() {
        mainCamera = Camera.main;
        numCollidersInModel = gameObject.GetComponentsInChildren<Collider>().Length;
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("CameraZone")) {
            var distance = other.GetComponent<CameraZone>().distance;
            var zoomModifier = 1;
            
            switch(distance)  {
                case CameraZone.CameraDistance.Near:
                    Debug.Log("ZOOM 1");
                    zoomModifier = 1;
                    break;
                case CameraZone.CameraDistance.Mid:
                    Debug.Log("ZOOM 2");
                    zoomModifier = 2;
                    break;
                case CameraZone.CameraDistance.Far:
                    Debug.Log("ZOOM 3");
                    zoomModifier = 3;
                    break;
                default:
                    Debug.Log("NO ZOOM");
                    break;
            }

            SetVirtualCamera(zoomModifier);
        }
    }

    void SetVirtualCamera(int zoom) {
        for (int i = 0; i < virtualCameras.Length; i++) {
            Debug.Log("Setting camera element " + (zoom - 1));
            virtualCameras[i].SetActive(i == (zoom - 1));
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cameraSphereCastRadius);
    }
}
