using UnityEngine;

public abstract class Enemy : MonoBehaviour {
    public enum StateTypes { IDLE, ATTACK, HURT, DEAD }

    public EnemyManager enemyManager;

    // Must be overridden in all children.  Must return the current state of the enemy FSM.
    public abstract StateTypes State { get; }
}
