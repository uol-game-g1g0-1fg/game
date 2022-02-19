using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Camera Settings")] 
    public GameObject[] virtualCameras;
    readonly CameraZone.CameraDistance baseDistance = CameraZone.CameraDistance.Mid;
    
    EnemyManager enemyManager;
    
    bool canSwitchCamera;
    
    void Start() {
        enemyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<EnemyManager>();
        HandleCameraZoom(baseDistance);
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("CameraZone")) {
            HandleCameraZoom(other.GetComponent<CameraZone>().distance);
        }
    }

    void OnTriggerExit(Collider other) {
        HandleCameraZoom(baseDistance);
    }
    
    void Update() {
        Debug.Log(canSwitchCamera);
        if (enemyManager.AnyEnemyAttacking()) {
            Debug.Log("Enemy Attacking");
            HandleCameraZoom(CameraZone.CameraDistance.Near);
            StartCoroutine(nameof(CameraSwitchCooldown));
            canSwitchCamera = false;
        } else if (canSwitchCamera) {
            Debug.Log("Zoom out");
            HandleCameraZoom(baseDistance);
        }
    }
    
    public IEnumerator CameraSwitchCooldown(){
        yield return new WaitForSeconds (5);
        canSwitchCamera = true;
    }

    void HandleCameraZoom(CameraZone.CameraDistance distance) {
        var index = 1;
            
        switch(distance)  {
            case CameraZone.CameraDistance.Near:
                Debug.Log("ZOOM 1");
                index = 1;
                break;
            case CameraZone.CameraDistance.Mid:
                Debug.Log("ZOOM 2");
                index = 2;
                break;
            case CameraZone.CameraDistance.Far:
                Debug.Log("ZOOM 3");
                index = 3;
                break;
            default:
                Debug.Log("NO ZOOM");
                break;
        }

        SetVirtualCamera(index - 1);
    }

    void SetVirtualCamera(int zoom) {
        for (int i = 0; i < virtualCameras.Length; i++) {
            // Debug.Log("Setting camera element " + (zoom));
            virtualCameras[i].SetActive(i == (zoom));
        }
    }
}
