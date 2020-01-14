using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.MonsterFight
{
    public class WalkingState : IState
    {
        private Unit _stateOwner;
        public WalkingState(Unit owner) { this._stateOwner = owner; }
        private string name = "walk";
        public string Name { get{ return name; }  set { name = value;} }

        public void Enter()
        {
            _stateOwner.reachedDestination = false;
            _stateOwner.navMeshAgent.SetDestination(_stateOwner.targetPosition);
            _stateOwner.animator.SetBool("walk", true);
        }

        public void Execute()
        {
            if(!_stateOwner.reachedDestination) CheckDestinationReached();
        }

        public void Exit()
        {
            _stateOwner.animator.SetBool("walk", false);
        }

        void CheckDestinationReached()
        {
            float distanceToTarget = Vector3.Distance(_stateOwner.transform.position, _stateOwner.targetPosition);
            if (distanceToTarget < _stateOwner.targetReachThreshold)
            {
                _stateOwner.reachedDestination = true;
                _stateOwner.ReachedTarget();
            }
        }
    }
}
