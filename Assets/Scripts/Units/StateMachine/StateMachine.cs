using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.MonsterFight
{
    public class StateMachine
    {
        IState _currentState;

        public void ChangeState(IState newState)
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }
            _currentState = newState;
            _currentState.Enter();
        }

        public void Update()
        {
            if (_currentState != null) _currentState.Execute();
        }

        public bool IsCurrentStateName(string name)
        {
            return _currentState.Name == name;
        }
    }
}
