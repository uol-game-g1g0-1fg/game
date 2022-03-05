using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour {
    public AudioMixerSnapshot idleSnapshot;
    public AudioMixerSnapshot battleSnapshot;

    EnemyManager enemyManager;
    
    bool canSwitchAudio;

    void Start() {
        enemyManager = gameObject.GetComponent<EnemyManager>();
        canSwitchAudio = true;
    }

    void Update() {
        if (enemyManager.AnyEnemyAttacking()) {
            battleSnapshot.TransitionTo(0.1f);
            StartCoroutine(nameof(AudioSnapshotSwitchCooldown));
            canSwitchAudio = false;
        } else if (canSwitchAudio) {
            idleSnapshot.TransitionTo(2f);
        }
    }
    
    public IEnumerator AudioSnapshotSwitchCooldown(){
        yield return new WaitForSeconds (5);
        canSwitchAudio = true;
    }
}