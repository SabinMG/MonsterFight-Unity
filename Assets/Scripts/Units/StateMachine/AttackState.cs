using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.MonsterFight
{
    public class AttackState : IState
    {
        private Unit _stateOwner;
        public AttackState(Unit owner) { this._stateOwner = owner; }
        private string name = "attack";
        public string Name { get { return name; } set { name = value; } }
        private float _attackStartTime;

        public void Enter()
        {
            _stateOwner.attackFinished = false;
            _stateOwner.animator.SetBool("attack", true);
            _attackStartTime = Time.time;
        }

        public void Execute()
        {
            // quickly trurning towards the target
            var targetRotation = Quaternion.LookRotation(_stateOwner.attackTarget.transform.position - _stateOwner.transform.position);
            _stateOwner.transform.rotation = Quaternion.RotateTowards(_stateOwner.transform.rotation, targetRotation, 5.0f);

            CheckForAttackIsOver();
        }

        public void Exit()
        {
            _stateOwner.animator.SetBool("attack", false);
        }

        void CheckForAttackIsOver()
        {
            if (_attackStartTime + _stateOwner.attackTime <= Time.time)
            {
                _stateOwner.attackFinished = true;
                _stateOwner.AttackFinished();
            } 
        } 
    }
}
