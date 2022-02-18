using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehavioursFSM;

namespace EnemyBehaviour
{
    public class EnemyFishState : IState
    {
        #region Variables
        protected EnemyFishFSM m_EnemyFSM;
        public EnemyFishFSM.StateTypes m_StateType;
        #endregion

        public EnemyFishFSM.StateTypes StateType => m_StateType;

        public EnemyFishState(MainFSM fsm, EnemyFishFSM.StateTypes type, EnemyFishFSM enemyFSM) : base(fsm)
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

    public class EnemyFishFSM : Enemy
    {
        #region Property Inspector Variables
        [Header("Enemy Settings")]
        //[SerializeField] private GameObject m_EnemyPlantGameObject;
        //[SerializeField] private GameObject m_EnemyPlantProjectile;
        //[SerializeField] private float m_EnemyPlantProjectileSpeed;
        //[SerializeField] private float m_EnemyPlantLOSRadius;
        [SerializeField] private float m_TimeBetweenAttacks;
        #endregion

        #region Variables
        public MainFSM m_MainFSM = null;
        private EnemyFishState m_State = null;

        private float m_MaxLOSDot = 0.2f;
        private float m_TimeSinceLastAttack = 0;
        private Rigidbody rbProjectile;

        private GameObject m_TargetEntity;
        private GameObject m_PlantProjectile;

        private string m_ClipName;
        private AnimatorClipInfo[] m_CurrentClipInfo;
        private GameObjectPoolManager m_ObjectPoolMgr;
        private GameObjectSpawner m_ObjectSpawner;

        //path
        [SerializeField] public GameObject[] patrolPoints;
        private int patrolPointIndex;
        private float pathPosition;
        private GameObject currentPoint;
        [SerializeField] public float speed;
        private float currentRotation;
        [SerializeField] public float rotationSpeed;
        private bool turn;
        #endregion

        #region Helpers
        private void SetState(StateTypes type)
        {
            m_MainFSM.SetCurrentState(m_MainFSM.GetState((int)type));
        }

        private EnemyFishState GetState(StateTypes type)
        {
            return (EnemyFishState)m_MainFSM.GetState((int)type);
        }

        public override StateTypes State {
            get => m_State.m_StateType;
        }

        private bool IsWithinRange()
        {
            //float dist = Vector3.Distance(m_TargetEntity.transform.position,m_EnemyPlantGameObject.transform.position);
            //if (dist < m_EnemyPlantLOSRadius && GetAngleToTarget() <= m_MaxLOSDot)
            //{
            //     return true;
            //}

            return false;
        }

        private float GetAngleToTarget()
        {
            Vector3 up = m_TargetEntity.transform.TransformDirection(Vector3.up);
            Vector3 toEnemy = Vector3.zero;//(m_EnemyPlantGameObject.transform.position - m_TargetEntity.transform.position).normalized;
            return Vector3.Dot(up, toEnemy);
        }

        #endregion

        private void Start()
        {
            m_MainFSM = new MainFSM();
            m_MainFSM.AddState((int)StateTypes.IDLE, new EnemyFishState(m_MainFSM, StateTypes.IDLE, this));
            m_MainFSM.AddState((int)StateTypes.ATTACK, new EnemyFishState(m_MainFSM, StateTypes.ATTACK, this));
            m_MainFSM.AddState((int)StateTypes.DEAD, new EnemyFishState(m_MainFSM, StateTypes.DEAD, this));

            Init_IdleState();
            Init_AttackState();
            Init_DieState();

            m_ObjectPoolMgr = GameObjectPoolManager.Instance;
            m_ObjectSpawner = GameObjectSpawner.Instance;

            // Start in the idle state by default
            m_MainFSM.SetCurrentState(m_MainFSM.GetState((int)StateTypes.IDLE));

            m_TargetEntity = GameObject.FindGameObjectWithTag("Player");

            patrolPointIndex = 0;
            pathPosition = 0;
            turn = false;
            
            // Add the fish to the Enemy Manager
            enemyManager = m_TargetEntity.GetComponent<EnemyManager>();
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
            
            // FIXME If the shark dies
            // Remove the fish from the Enemy Manager
            // enemyManager.Remove(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "PatrolPoint")
            {
                //Debug.Log(other.name);
                if (patrolPointIndex < patrolPoints.Length - 1)
                    patrolPointIndex += 1;
                else
                    patrolPointIndex = 0;

                currentPoint = patrolPoints[patrolPointIndex];
                pathPosition = 0.0f;

                // add turn animation ?
                currentRotation = 0.0f;
                turn = true;
            }
           
        }

        private void SlowTurn()
        {
            //pathPosition = 0;
            currentRotation += rotationSpeed * Time.deltaTime;

            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            if(currentRotation >= 180)
            {
                currentRotation = 0;
                turn = false;
            }
        }

        private void Init_IdleState()
        {
            m_State = GetState(StateTypes.IDLE);

            m_State.OnEnterDelegate += delegate ()
            {
                // TODO: Potentially Implement idle OnEnter logic here
                currentPoint = patrolPoints[patrolPointIndex];
            };

            m_State.OnUpdateDelegate += delegate ()
            {
                if (IsWithinRange() && m_TimeSinceLastAttack > m_TimeBetweenAttacks)
                {
                    SetState(StateTypes.ATTACK);
                }

                pathPosition += 0.001f * speed * Time.deltaTime;
                //Debug.Log(pathPosition);
                transform.position = Vector3.Lerp(transform.position, currentPoint.transform.position, pathPosition);
                if (turn)
                    SlowTurn();
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
            m_State = GetState(StateTypes.DEAD);

            m_State.OnEnterDelegate += delegate ()
            {

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

        }
    }
}
