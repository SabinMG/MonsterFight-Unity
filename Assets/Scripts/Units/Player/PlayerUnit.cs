using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SpaceOrigin.Data;

namespace SpaceOrigin.MonsterFight
{
    public class PlayerUnit : Unit,IDamageable, IKillable
    {
        public IntSO alivePlayersUnitsSO;
        public Color attackPossibleClr;
        public Color attackNotPossibleClr;

        private int _maxMoveBfrAttack = 1000000; // offering max steps for now, limited move within limited sqaure around the player is also a good choice
        private int _maxAttacks = 1;
        private int _numberOfMovesLeft;
        private int _numberOfAttacksLeft;

        void Start()
        {
            Idle();
        }

        public void NewTurn()
        {
            _numberOfMovesLeft = _maxMoveBfrAttack; // not using for now, the idea we can have min nuber of moves before attack
            _numberOfAttacksLeft = _maxAttacks;
            SetActiveIndicatorColor(attackPossibleClr);
        }

        public void Attack()
        {
            _numberOfAttacksLeft--;
            if (attackTarget != null) stateMachine.ChangeState(new AttackState(this));
        }

        public bool CanMove() // if walkiing, atttackiing  
        {
            bool isWalking = stateMachine.IsCurrentStateName("walk");
            bool isAttacking = stateMachine.IsCurrentStateName("attack");
            return !isWalking && !isAttacking;
        }

        public bool CanDoAnotherAttack()
        {
            return _numberOfAttacksLeft > 0;
        }

        public override void Execute()
        {
            stateMachine.Update();
           
        }

        public override void Idle()
        {
            stateMachine.ChangeState(new IdleState(this));
        }

        public override void Move(Vector3 position)
        {
            _numberOfMovesLeft--;
            targetPosition = position;
            stateMachine.ChangeState(new WalkingState(this));
        }

        public override void Damage(int damageTaken)
        {
            base.Damage(damageTaken);
        }

        public override void Kill()
        {
            base.Kill();
            alivePlayersUnitsSO.Value = alivePlayersUnitsSO.Value - 1;
            GameAbstractFactory gameAbtractFactory = GameFactoryProducer.GetFactory("PlayerUnitsFactory");
            gameAbtractFactory.RecyclePlayerUnit(this);
        }

        public override void ReachedTarget()
        {
            base.ReachedTarget();
            Idle();
        }

        public override void AttackFinished()
        {
            if (_numberOfAttacksLeft == 0) SetActiveIndicatorColor(attackNotPossibleClr);   // TODO: Rewrite not right way
            base.AttackFinished();
            attackTarget.Damage(attackPower);
            Idle();
        }
    }
}
