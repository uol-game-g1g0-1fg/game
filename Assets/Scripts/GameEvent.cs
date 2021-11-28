using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Event", fileName = "New Game Event")]
public class GameEvent : ScriptableObject {

    HashSet<GameEventListener> listners = new HashSet<GameEventListener>();

    public void Invoke() {
        foreach (var globalEventListener in listners) {
            globalEventListener.RaiseEvent();
        }
    }

    public void Register(GameEventListener gameEventListener) => listners.Add(gameEventListener);

    public void Deregister(GameEventListener gameEventListener) => listners.Remove(gameEventListener);
    
}
