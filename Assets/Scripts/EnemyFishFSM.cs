using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehavioursFSM;

namespace EnemyBehaviour {
    public class EnemyFishState : IState {
        #region Variables

        EnemyFishFSM m_EnemyFSM;
        readonly Enemy.StateTypes m_StateType;
        #endregion

        public Enemy.StateTypes StateType => m_StateType;

        public EnemyFishState(MainFSM fsm, Enemy.StateTypes type, EnemyFishFSM enemyFSM) : base(fsm) {
            m_EnemyFSM = enemyFSM;
            m_StateType = type;
        }

        public delegate void StateDelegate();
        public StateDelegate OnEnterDelegate { get; set; } = null;
        public StateDelegate OnExitDelegate { get; set; } = null;
        public StateDelegate OnFixedUpdateDelegate { get; set; } = null;
        public StateDelegate OnUpdateDelegate { get; set; } = null;

        public override void Enter() => OnEnterDelegate?.Invoke();

        public override void FixedUpdate() => OnFixedUpdateDelegate?.Invoke();

        public override void Update() => OnUpdateDelegate?.Invoke();

        public override void Exit() => OnExitDelegate?.Invoke();
    }

    public class EnemyFishFSM : Enemy {
        #region Property Inspector Variables
        [Header("Enemy Settings")]
        [SerializeField] GameObject m_EnemyFishGameObject;
        Rigidbody rb;
        [SerializeField] float m_TimeBetweenAttacks;
        [SerializeField] float m_EnemyFishLOSRadius;
        [SerializeField] float m_EnemyFishBreakOffRadius;
        #endregion

        #region Variables
        public Animator animator;
        public MainFSM m_MainFSM = null;
        public EnemyFishState m_State = null;
        public StateTypes m_StateType;
        [SerializeField] GameEvent OnBite;
        EnemyHealth enemyHealth;

        float m_MaxLOSDot = 0.2f;
        float m_TimeSinceLastAttack = 0;
        Rigidbody rbProjectile;

        GameObject m_TargetEntity;
        GameObject m_PlantProjectile;

        string m_ClipName;
        AnimatorClipInfo[] m_CurrentClipInfo;
        GameObjectPoolManager m_ObjectPoolMgr;
        GameObjectSpawner m_ObjectSpawner;

        //path
        [SerializeField] public GameObject[] patrolPoints;
        int patrolPointIndex;
        GameObject currentPoint;
        [SerializeField, Tooltip("Movement speed while patrolling")] float movementSpeed;
        float movementVelocity;
        [SerializeField, Tooltip("Rotation speed of the shark at patrol points")] float rotationSpeed = 50f;
        #endregion

        #region Helpers
        void SetState(StateTypes type) {
            m_MainFSM.SetCurrentState(m_MainFSM.GetState((int)type));
        }

        EnemyFishState GetState(StateTypes type) {
            return (EnemyFishState)m_MainFSM.GetState((int)type);
        }

        public override StateTypes State => m_StateType;

        bool IsWithinRange(float range) => (Vector3.Distance(m_TargetEntity.transform.position, m_EnemyFishGameObject.transform.position) < range);

        float GetAngleToTarget() {
            var up = m_TargetEntity.transform.TransformDirection(Vector3.up);  // what is happening here?
            var toEnemy = Vector3.zero;
            return Vector3.Dot(up, toEnemy);
        }

        #endregion

        void Start() {
            animator = GetComponent<Animator>();
            m_MainFSM = new MainFSM();
            m_MainFSM.AddState((int)StateTypes.IDLE, new EnemyFishState(m_MainFSM, StateTypes.IDLE, this));
            m_MainFSM.AddState((int)StateTypes.ATTACK, new EnemyFishState(m_MainFSM, StateTypes.ATTACK, this));
            m_MainFSM.AddState((int)StateTypes.HURT, new EnemyFishState(m_MainFSM, StateTypes.HURT, this));
            m_MainFSM.AddState((int)StateTypes.DEAD, new EnemyFishState(m_MainFSM, StateTypes.DEAD, this));

            rb = gameObject.GetComponent<Rigidbody>();

            Init_IdleState();
            Init_AttackState();
            Init_HurtState();
            Init_DieState();

            m_ObjectPoolMgr = GameObjectPoolManager.Instance;
            m_ObjectSpawner = GameObjectSpawner.Instance;

            // Start in the idle state by default
            SetState(StateTypes.IDLE);

            m_TargetEntity = GameObject.FindGameObjectWithTag("Player");
            enemyHealth = gameObject.GetComponent<EnemyHealth>();

            patrolPointIndex = 0;
            movementVelocity = 0;
            
            // Add the fish to the Enemy Manager
            enemyManager = m_TargetEntity.GetComponent<EnemyManager>();
            enemyManager.Add(this);
        }

        void Update() {
            m_MainFSM.Update();
        }
        
        void FixedUpdate() {
            m_MainFSM.FixedUpdate();

            m_TimeSinceLastAttack += Time.deltaTime;
            
            if (enemyHealth.GetHealth() <= 40) {
                // FIXME could make the shark flee?
                // SetState(StateTypes.HURT);
            }
            
            if (enemyHealth.GetHealth() <= 0) {
                SetState(StateTypes.DEAD);
                
                // Remove the fish from the Enemy Manager
                enemyManager.Remove(this);
                
                // Death Animation
                animator.enabled = false;
                enemyHealth.SetIsDead();
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                rb.isKinematic = false;
                rb.mass = 500;
                rb.rotation = Quaternion.Slerp(rb.transform.rotation, Quaternion.Euler(0, 0,-180), 0.5f * Time.deltaTime);
            }
        }

        void OnTriggerEnter(Collider other) {
            if (other.CompareTag("PatrolPoint")) {
                Debug.Log("Trigger Patrol Point: " + other.name);
                if (patrolPointIndex < patrolPoints.Length - 1)
                    patrolPointIndex += 1;
                else
                    patrolPointIndex = 0;

                currentPoint = patrolPoints[patrolPointIndex];
                movementVelocity = 0.0f;
            }
           
        }

        void Init_IdleState() {
            m_State = GetState(StateTypes.IDLE);

            m_State.OnEnterDelegate += delegate () {
                m_StateType = StateTypes.IDLE;
                Debug.Log(patrolPoints);
                currentPoint = patrolPoints[patrolPointIndex];
            };

            m_State.OnUpdateDelegate += delegate () {
                if (IsWithinRange(m_EnemyFishLOSRadius) && m_TimeSinceLastAttack > m_TimeBetweenAttacks) {
                    SetState(StateTypes.ATTACK);
                }

                movementVelocity += 0.1f * movementSpeed * Time.deltaTime;
                transform.position = Vector3.Slerp(transform.position, currentPoint.transform.position, movementVelocity);
                
                // Create the rotation we need to be in to look at the target
                var lookRotation = Quaternion.LookRotation((currentPoint.transform.position - transform.position).normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            };
            
            m_State.OnExitDelegate += delegate () {};
        }

        void Bite() {
            if (IsWithinRange(4)) {
                OnBite?.Invoke();
            }
        }

        void Init_AttackState() {
            m_State = GetState(StateTypes.ATTACK);

            m_State.OnEnterDelegate += delegate () {
                m_StateType = StateTypes.ATTACK;

                Debug.Log("SHARK ATTACK!");
                animator.SetBool("isAttack", true);
            };

            m_State.OnFixedUpdateDelegate += delegate () {};
            m_State.OnUpdateDelegate += delegate() {
                if (!IsWithinRange(m_EnemyFishBreakOffRadius)) {
                    SetState(StateTypes.IDLE);
                    return;
                }
                
                transform.position = Vector3.Slerp(transform.position, m_TargetEntity.transform.position, movementVelocity);
                
                var lookRotation = Quaternion.LookRotation((m_TargetEntity.transform.position - transform.position).normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            };

            m_State.OnExitDelegate += delegate () {
                m_TimeSinceLastAttack = 0;
                animator.SetBool("isAttack", false);
            };
        }
        void Init_HurtState() {
            m_State = GetState(StateTypes.HURT);

            m_State.OnEnterDelegate += delegate() {
                m_StateType = StateTypes.HURT;
            };

            m_State.OnExitDelegate += delegate () {};
            m_State.OnUpdateDelegate += delegate () {};
        }

        void Init_DieState() {
            m_State = GetState(StateTypes.DEAD);

            m_State.OnEnterDelegate += delegate() {
                m_StateType = StateTypes.DEAD;
            };

            m_State.OnExitDelegate += delegate () {};
            m_State.OnUpdateDelegate += delegate () {};
        }
    }
}
