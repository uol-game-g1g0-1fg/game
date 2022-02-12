using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour {
    
    [Serializable]
    public enum CameraDistance {
        Near,
        Mid,
        Far
    };
    
    [SerializeField, Tooltip("Camera Distance for this Zone, set upon entry.")]
    public CameraDistance distance;

    void Start() {
        
    }

    void Update() {
        
    }
}
