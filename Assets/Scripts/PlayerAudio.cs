using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour {

    public AudioMixerSnapshot idleSnapshot;
    public AudioMixerSnapshot battleSnapshot;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("DangerZone")) {
            battleSnapshot.TransitionTo(0.5f);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("DangerZone")) {
            idleSnapshot.TransitionTo(2f);
        }
    }
}