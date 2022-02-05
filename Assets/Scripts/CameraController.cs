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

    void Update() {
        HandleCameraSphereCollider();
    }

    void HandleCameraSphereCollider() {
        int zoomModifier = 1 - numCollidersInModel;
        Collider[] hitColliders = new Collider[virtualCameras.Length + numCollidersInModel];
        
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, cameraSphereCastRadius, hitColliders);

        for (int i = 0; i < numColliders - numCollidersInModel; i++) {
            zoomModifier++;
        }
        //Debug.Log("Camera Zoom: " + zoomModifier);

        for (int i = 0; i < virtualCameras.Length - 1; i++) {
            virtualCameras[i].SetActive(i == zoomModifier);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cameraSphereCastRadius);
    }
}
