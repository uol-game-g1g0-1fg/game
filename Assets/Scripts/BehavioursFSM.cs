using System.Collections;
using System.Collections.Generic;

namespace BehavioursFSM
{
    public abstract class IState
    {
        #region ClassConstructor
        protected IState(MainFSM fsm)
        {
            m_MainFSM = fsm;
        }
        #endregion

        #region VirtualMethods
        public virtual void Enter() {}
        public virtual void Update() {}
        public virtual void Exit() {}
        #endregion

        #region Variables
        protected MainFSM m_MainFSM;
        #endregion
    }

    public class MainFSM
    {
        #region ClassConstructor
        public MainFSM() {}
        #endregion

        #region Variables
        protected Dictionary<int, IState> m_States = new Dictionary<int, IState>();
        protected IState m_CurrentState;
        #endregion

        public void AddState(int index, IState state)
        {
            m_States.Add(index, state);
        }

        public IState GetState(int index)
        {
            if (index < m_States.Count)
            {
                return m_States[index];
            }

            return null;
        }

        public void SetCurrentState(IState state)
        {
            // If we have a current state, exit before transitioning into a new one.
            m_CurrentState?.Exit();

            // Set the current state to the new state.
            m_CurrentState = state;

            // If the current state is not null, enter the state.
            m_CurrentState?.Enter();
        }

        public void Update()
        {
            // If the current state is not null, update the state.
            m_CurrentState?.Update();
        }
    }
}
