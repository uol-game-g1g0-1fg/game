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

    public class EnemyPlantFSM : MonoBehaviour
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
        #endregion

        #region Variables
        public MainFSM m_MainFSM = null;
        public Animator m_EnemyPlantAnimator;
        public enum StateTypes { IDLE = 0, ATTACK, DIE }

        private EnemyPlantState m_State = null;
        private float m_MaxLOSDot = 0.2f;
        private float m_TimeSinceLastAttack = 0;
        private Rigidbody rbProjectile;

        private GameObject m_TargetEntity;
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

        private bool IsWithinRange()
        {
            float dist = Vector3.Distance(m_TargetEntity.transform.position,m_EnemyPlantGameObject.transform.position);
            if (dist < m_EnemyPlantLOSRadius && GetAngleToTarget() <= m_MaxLOSDot)
            {
                return true;
            }

            return false;
        }

        private float GetAngleToTarget()
        {
            Vector3 up = m_TargetEntity.transform.TransformDirection(Vector3.up);
            Vector3 toEnemy = (m_EnemyPlantGameObject.transform.position - m_TargetEntity.transform.position).normalized;
            return Vector3.Dot(up, toEnemy);
        }

        #endregion

        private void Start()
        {
            m_MainFSM = new MainFSM();
            m_MainFSM.AddState((int)StateTypes.IDLE, new EnemyPlantState(m_MainFSM, StateTypes.IDLE, this));
            m_MainFSM.AddState((int)StateTypes.ATTACK, new EnemyPlantState(m_MainFSM, StateTypes.ATTACK, this));
            m_MainFSM.AddState((int)StateTypes.DIE, new EnemyPlantState(m_MainFSM, StateTypes.DIE, this));

            Init_IdleState();
            Init_AttackState();
            Init_DieState();

            m_ObjectPoolMgr = GameObjectPoolManager.Instance;
            m_ObjectSpawner = GameObjectSpawner.Instance;

            // Start in the idle state by default
            m_MainFSM.SetCurrentState(m_MainFSM.GetState((int)StateTypes.IDLE));

            m_TargetEntity = GameObject.FindGameObjectWithTag("Player");
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
        }

        private void Init_IdleState()
        {
            m_State = GetState(StateTypes.IDLE);

            m_State.OnEnterDelegate += delegate ()
            {
                // TODO: Potentially Implement idle OnEnter logic here
            };

            m_State.OnUpdateDelegate += delegate ()
            {
                if (IsWithinRange() && m_TimeSinceLastAttack > m_TimeBetweenAttacks)
                {
                    SetState(StateTypes.ATTACK);
                }
            };

            m_State.OnExitDelegate += delegate ()
            {
                // TODO: Potentially Implement idle OnExit logic here
            };
        }

        private void Init_AttackState()
        {
            m_State = GetState(StateTypes.ATTACK);

            m_State.OnEnterDelegate += delegate ()
            {
                m_EnemyPlantAnimator.SetTrigger("RangeAttack");
            };

            m_State.OnFixedUpdateDelegate += delegate ()
            {
                SetState(StateTypes.IDLE);
            };

            m_State.OnUpdateDelegate += delegate()
            {
            };

            m_State.OnExitDelegate += delegate ()
            {
                m_TimeSinceLastAttack = 0;
            };
        }
        private void Init_DieState()
        {
            m_State = GetState(StateTypes.DIE);

            m_State.OnEnterDelegate += delegate ()
            {
                m_EnemyPlantAnimator.SetTrigger("Die");
            };

            m_State.OnExitDelegate += delegate ()
            {
                // TODO: Potentially Implement die OnExit logic here
                //Debug.Log("OnExit - DIE");
            };

            m_State.OnUpdateDelegate += delegate ()
            {
                // TODO: Potentially Implement die OnUpdate logic here
                //Debug.Log("OnUpdate - DIE");
            };
        }

        // Called when the animation hits the event tag.
        private void FireProjectile()
        {
            if (m_ClipName.Equals("RangeAttack"))
            {
                const string tag = "ProjectileSpawnPoint";
                Vector3 projectilePos = m_ObjectSpawner.FindTransformObjectWithTag(tag, m_EnemyPlantGameObject.transform).position;
                m_PlantProjectile = m_ObjectPoolMgr.SpawnFromPool("PlantProjectile", projectilePos, Quaternion.identity);
                rbProjectile = m_PlantProjectile.GetComponent<Rigidbody>();

                if (m_PlantProjectile)
                {
                    m_PlantProjectile.transform.LookAt(m_TargetEntity.transform.position);
                    rbProjectile.AddForce(m_PlantProjectile.transform.forward * m_EnemyPlantProjectileSpeed);
                    OnFire?.Invoke();
                }
            }
        }
    }
}
