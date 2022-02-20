using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehavioursFSM;

namespace EnemyBehaviour
{
    public class EnemyPlantState : IState
    {
        #region Variables
        protected EnemyPlantFSM m_EnemyFSM;
        protected EnemyPlantFSM.StateTypes m_StateType;
        #endregion

        public EnemyPlantFSM.StateTypes StateType => m_StateType;

        public EnemyPlantState(MainFSM fsm, EnemyPlantFSM.StateTypes type, EnemyPlantFSM enemyFSM) : base(fsm)
        {
            m_EnemyFSM = enemyFSM;
            m_StateType = type;
        }

        public delegate void StateDelegate();
        public StateDelegate OnEnterDelegate { get; set; } = null;
        public StateDelegate OnExitDelegate { get; set; } = null;
        public StateDelegate OnFixedUpdateDelegate { get; set; } = null;
        public StateDelegate OnUpdateDelegate { get; set; } = null;

        public override void Enter()
        {
            OnEnterDelegate?.Invoke();
        }

        public override void FixedUpdate()
        {
            OnFixedUpdateDelegate?.Invoke();
        }

        public override void Update()
        {
            OnUpdateDelegate?.Invoke();
        }

        public override void Exit()
        {
            OnExitDelegate?.Invoke();
        }
    }

    public class EnemyPlantFSM : Enemy
    {
        #region Property Inspector Variables
        [Header("Enemy Settings")]
        [SerializeField] private GameObject m_EnemyPlantGameObject;
        [SerializeField] private GameObject m_EnemyPlantProjectile;
        [SerializeField] private float m_EnemyPlantProjectileSpeed;
        [SerializeField] private float m_EnemyPlantLOSRadius;
        [SerializeField] private float m_TimeBetweenAttacks;

        [Header("Events")]
        [SerializeField] GameEvent OnFire;
        [SerializeField] GameEvent OnPlantDeath;
        #endregion

        #region Variables
        public MainFSM m_MainFSM = null;
        public Animator m_EnemyPlantAnimator;

        private EnemyPlantState m_State = null;
        private StateTypes m_StateType;
        private EnemyPlantHealth m_EnemyPlantHealth;

        private float m_MaxLOSDot = 0.2f;
        private float m_TimeSinceLastAttack = 0;
        private Rigidbody rbProjectile;

        private GameObject m_Player;
        private PlayerHealth m_PlayerHealth;
        private GameObject m_PlantProjectile;

        private string m_ClipName;
        private AnimatorClipInfo[] m_CurrentClipInfo;
        private GameObjectPoolManager m_ObjectPoolMgr;
        private GameObjectSpawner m_ObjectSpawner;
        #endregion

        #region Helpers
        private void SetState(StateTypes type)
        {
            m_MainFSM.SetCurrentState(m_MainFSM.GetState((int)type));
        }

        private EnemyPlantState GetState(StateTypes type)
        {
            return (EnemyPlantState)m_MainFSM.GetState((int)type);
        }

        public override StateTypes State => m_StateType;

        private bool ShouldAttack()
        {
            if (m_PlayerHealth.IsDead())
            {
                return false;
            }

            return (IsWithinRange() && m_TimeSinceLastAttack > m_TimeBetweenAttacks);
        }

        private bool IsWithinRange()
        {
            float dist = Vector3.Distance(m_Player.transform.position,m_EnemyPlantGameObject.transform.position);
            if (dist < m_EnemyPlantLOSRadius && GetAngleToTarget() <= m_MaxLOSDot)
            {
                return true;
            }

            return false;
        }

        private float GetAngleToTarget()
        {
            Vector3 up = m_Player.transform.TransformDirection(Vector3.up);
            Vector3 toEnemy = (m_EnemyPlantGameObject.transform.position - m_Player.transform.position).normalized;
            return Vector3.Dot(up, toEnemy);
        }

        #endregion

        private void Start()
        {
            m_MainFSM = new MainFSM();
            m_MainFSM.AddState((int)StateTypes.IDLE, new EnemyPlantState(m_MainFSM, StateTypes.IDLE, this));
            m_MainFSM.AddState((int)StateTypes.ATTACK, new EnemyPlantState(m_MainFSM, StateTypes.ATTACK, this));
            m_MainFSM.AddState((int)StateTypes.DEAD, new EnemyPlantState(m_MainFSM, StateTypes.DEAD, this));

            Init_IdleState();
            Init_AttackState();
            Init_DeadState();

            m_ObjectPoolMgr = GameObjectPoolManager.Instance;
            m_ObjectSpawner = GameObjectSpawner.Instance;

            // Start in the idle state by default
            SetState(StateTypes.IDLE);

            m_Player = GameObject.FindGameObjectWithTag("Player");
            m_PlayerHealth = m_Player.GetComponent<PlayerHealth>();
            m_EnemyPlantHealth = m_EnemyPlantGameObject.GetComponent<EnemyPlantHealth>();
            
            // Add the plant to the Enemy Manager
            enemyManager = m_Player.GetComponent<EnemyManager>();
            enemyManager.Add(this);
        }

        private void Update()
        {
            m_MainFSM.Update();
        }

        private void FixedUpdate()
        {
            m_MainFSM.FixedUpdate();

            m_TimeSinceLastAttack += Time.deltaTime;
            m_CurrentClipInfo = m_EnemyPlantAnimator.GetCurrentAnimatorClipInfo(0);
            m_ClipName = m_CurrentClipInfo[0].clip.name;

            if (m_EnemyPlantHealth.GetHealth() <= 0.0f && m_StateType != StateTypes.DEAD)
            {
                SetState(StateTypes.DEAD);
                CleanUp();
            }
        }

        private void Init_IdleState()
        {
            m_State = GetState(StateTypes.IDLE);

            m_State.OnEnterDelegate += delegate ()
            {
                m_StateType = StateTypes.IDLE;

                m_EnemyPlantAnimator.SetTrigger("Idle");
            };

            m_State.OnFixedUpdateDelegate += delegate ()
            {
                if (ShouldAttack())
                {
                    SetState(StateTypes.ATTACK);
                }
            };

            m_State.OnExitDelegate += delegate () {};
        }

        private void Init_AttackState()
        {
            m_State = GetState(StateTypes.ATTACK);

            m_State.OnEnterDelegate += delegate ()
            {
                m_StateType = StateTypes.ATTACK;

                if (m_EnemyPlantAnimator.HasState(0, Animator.StringToHash("RangeAttack")))
                {
                    m_EnemyPlantAnimator.SetTrigger("RangeAttack");
                }
                else if (m_EnemyPlantAnimator.HasState(0, Animator.StringToHash("Spell")))
                {
                    m_EnemyPlantAnimator.SetTrigger("Spell");
                }
                m_TimeSinceLastAttack = 0;
            };

            m_State.OnFixedUpdateDelegate += delegate ()
            {
                if (!ShouldAttack())
                {
                    SetState(StateTypes.IDLE);
                }
            };

            m_State.OnExitDelegate += delegate ()
            {
                m_TimeSinceLastAttack = 0;
            };
        }

        private void Init_DeadState()
        {
            m_State = GetState(StateTypes.DEAD);

            m_State.OnEnterDelegate += delegate ()
            {
                m_StateType = StateTypes.DEAD;
                m_EnemyPlantAnimator.SetTrigger("Death");
                OnPlantDeath.Invoke();
            };

            m_State.OnUpdateDelegate += delegate () {};

            m_State.OnExitDelegate += delegate () {};
        }

        // Called when the animation hits the event tag.
        private void FireProjectile()
        {
            if (!m_ClipName.Equals("RangeAttack")) { return; }

            const string tag = "ProjectileSpawnPoint";
            Vector3 projectilePos = m_ObjectSpawner.FindTransformObjectWithTag(tag, m_EnemyPlantGameObject.transform).position;
            m_PlantProjectile = m_ObjectPoolMgr.SpawnFromPool("PlantProjectile", projectilePos, Quaternion.identity);

            if (!m_PlantProjectile) { return; }

            rbProjectile = m_PlantProjectile.GetComponent<Rigidbody>();
            m_PlantProjectile.transform.LookAt(m_Player.transform.position);
            rbProjectile.AddForce(m_PlantProjectile.transform.forward * m_EnemyPlantProjectileSpeed);
            OnFire.Invoke();
        }

        private void CleanUp()
        {
            // Remove the plant from the Enemy Manager
            enemyManager.Remove(this);

            // Disable the plant particles
            ParticleSystem particles = m_EnemyPlantGameObject.GetComponentInChildren<ParticleSystem>();
            if (particles)
            {
                particles.Stop();
            }

            // Swap betweeen alive/dead collider positions
            BoxCollider[] boxColliders = m_EnemyPlantGameObject.GetComponents<BoxCollider>();
            if (boxColliders.Length > 1)
            {
                foreach (BoxCollider boxCollider in boxColliders)
                {
                    boxCollider.enabled = !boxCollider.enabled;
                }
            }
        }
    }
}