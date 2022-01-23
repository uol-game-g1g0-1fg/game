using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehavioursFSM;

namespace EnemyBehaviour
{
    public class EnemyState : IState
    {
        #region Variables
        protected EnemyFSM m_EnemyFSM;
        protected EnemyFSM.StateTypes m_StateType;
        #endregion

        public EnemyFSM.StateTypes StateType => m_StateType;

        public EnemyState(MainFSM fsm, EnemyFSM.StateTypes type, EnemyFSM enemyFSM) : base(fsm)
        {
            m_EnemyFSM = enemyFSM;
            m_StateType = type;
        }

        public delegate void StateDelegate();
        public StateDelegate OnEnterDelegate { get; set; } = null;
        public StateDelegate OnExitDelegate { get; set; } = null;
        public StateDelegate OnUpdateDelegate { get; set; } = null;

        public override void Enter()
        {
            OnEnterDelegate?.Invoke();
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

    public class EnemyFSM : MonoBehaviour
    {
        #region Variables
        public MainFSM m_MainFSM; 
        public enum StateTypes { IDLE = 0, PATROL, ATTACK, DIE }
        private EnemyState m_State;
        #endregion

        #region Helpers
        private void SetState(StateTypes type)
        {
            m_MainFSM.SetCurrentState(m_MainFSM.GetState((int)type));
        }

        private EnemyState GetState(StateTypes type)
        {
            return (EnemyState)m_MainFSM.GetState((int)type);
        }

        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            m_MainFSM = new MainFSM();
            m_MainFSM.AddState((int)StateTypes.IDLE, new EnemyState(m_MainFSM, StateTypes.IDLE, this));
            m_MainFSM.AddState((int)StateTypes.PATROL, new EnemyState(m_MainFSM, StateTypes.PATROL, this));
            m_MainFSM.AddState((int)StateTypes.ATTACK, new EnemyState(m_MainFSM, StateTypes.ATTACK, this));
            m_MainFSM.AddState((int)StateTypes.DIE, new EnemyState(m_MainFSM, StateTypes.DIE, this));

            Init_IdleState();
            Init_PatrolState();
            Init_AttackState();
            Init_DieState();

            // Start in the idle state by default
            m_MainFSM.SetCurrentState(m_MainFSM.GetState((int)StateTypes.IDLE));
        }

        // Update is called once per frame
        private void Update()
        {
            m_MainFSM.Update();
        }

        private void Init_IdleState()
        {
            m_State = GetState(StateTypes.IDLE);

            m_State.OnEnterDelegate += delegate ()
            {
                // TODO: Implement idle OnEnter logic here
                Debug.Log("OnEnter - IDLE");
            };

            m_State.OnUpdateDelegate += delegate ()
            {
                // TODO: Implement idle OnUpdate logic here
                // Debug.Log("OnUpdate - IDLE");
            };

            m_State.OnExitDelegate += delegate ()
            {
                // TODO: Implement idle OnExit logic here
                //Debug.Log("OnExit - IDLE");
            };
        }
        private void Init_PatrolState()
        {
            m_State = GetState(StateTypes.PATROL);

            m_State.OnEnterDelegate += delegate ()
            {
                // TODO: Implement patrol OnEnter logic here
                Debug.Log("OnEnter - PATROL");
            };

            m_State.OnUpdateDelegate += delegate ()
            {
                // TODO: Implement patrol OnUpdate logic here
                //Debug.Log("OnUpdate - PATROL");
            };

            m_State.OnExitDelegate += delegate ()
            {
                // TODO: Implement patrol OnExit logic here
                //Debug.Log("OnExit - PATROL");
            };
        }
        private void Init_AttackState()
        {
            m_State = GetState(StateTypes.ATTACK);

            m_State.OnEnterDelegate += delegate ()
            {
                // TODO: Implement attack OnEnter logic here
                Debug.Log("OnEnter - ATTACK");
            };

            m_State.OnUpdateDelegate += delegate ()
            {
                // TODO: Implement attack OnUpdate logic here
                //Debug.Log("OnUpdate - ATTACK");
            };

            m_State.OnExitDelegate += delegate ()
            {
                // TODO: Implement attack OnExit logic here
                //Debug.Log("OnExit - ATTACK");
            };
        }
        private void Init_DieState()
        {
            m_State = GetState(StateTypes.DIE);

            m_State.OnEnterDelegate += delegate ()
            {
                // TODO: Implement die OnEnter logic here
                Debug.Log("OnEnter - DIE");
            };

            m_State.OnExitDelegate += delegate ()
            {
                // TODO: Implement die OnExit logic here
                //Debug.Log("OnExit - DIE");
            };

            m_State.OnUpdateDelegate += delegate ()
            {
                // TODO: Implement die OnUpdate logic here
                //Debug.Log("OnUpdate - DIE");
            };
        }
    }
}
