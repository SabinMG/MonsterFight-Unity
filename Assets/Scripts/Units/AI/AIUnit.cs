using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceOrigin.Data;
using SpaceOrigin.AI;

namespace SpaceOrigin.MonsterFight
{
    public class AIUnit : Unit
    {
        public IntSO aliveAIUnitsSO;

        public bool finishedTurn { get; set;}
        public List<DistanceData> closetPlayerUnitsData { get; set; } // stores the units and its distance ordererd by distance // set from AIunits controller

        private BehaviourTree _behvourTree;
        public delegate void AIUnitEvents(AIUnit unit);
        public AIUnitEvents onUnitFinishTurn;

        public void NewTurn()
        {
            closetPlayerUnitsData = PlayerUnitsController.Instance.GetClosetPlayerUnitsData(this);
            Idle(); // set state to idle
            finishedTurn = false;
            reachedDestination = false;
            attackFinished = false;
            BuildBehaviourTree(); 
        }

        private void BuildBehaviourTree()
        {
            BTActionNode checkAIUnitsCountCriticalNode = new BTActionNode(CheckAIUnitsCountCritical);
            BTActionNode findWeakestPlayerUnitNode = new BTActionNode(FindWeakestPlayerUnit);
            BTActionNode findClosetPlayerUnitNode = new BTActionNode(FindClosestPlayerUnit);
            BTActionNode moveTowardsThePlayerUnitNode = new BTActionNode(MoveTowardsThePlayerUnit);
            BTActionNode attackPlayerUnitNode = new BTActionNode(AttackPlayerUnit);

            BTSequence MoveTowardsAndAttackSequence = new BTSequence(new List<BTNode>()
            {
                moveTowardsThePlayerUnitNode,
                attackPlayerUnitNode
            });

            BTSequence WeakestPlayerAttackSequence = new BTSequence(new List<BTNode>()
            {
                findWeakestPlayerUnitNode,
                MoveTowardsAndAttackSequence
            });

            BTSequence aiWeakSequence = new BTSequence(new List<BTNode>()  // check if the AI count is less than or eaqual to player count, al
            {
                  checkAIUnitsCountCriticalNode,
                  WeakestPlayerAttackSequence,
            });

            BTSequence aiStrongSequence = new BTSequence(new List<BTNode>() //moment I am checking againt unit, also can have centralized AI score, => health of all unts + unt count
            {
                  findClosetPlayerUnitNode,
                  MoveTowardsAndAttackSequence,
            });


            BTSelector selectorRoot = new BTSelector(new List<BTNode>()
            {
                aiWeakSequence,aiStrongSequence
            });

            _behvourTree = new BehaviourTree(selectorRoot);
        }

        public override void Idle()
        {
            stateMachine.ChangeState(new IdleState(this));
        }

        public override void Execute()
        {
            stateMachine.Update();

            if (_behvourTree != null)
            {
                BTNodeStates state = _behvourTree.Behave();
                if (state == BTNodeStates.SUCCESS)
                {
                    if (onUnitFinishTurn != null) onUnitFinishTurn.Invoke(this);
                }

                if (state == BTNodeStates.FAILURE) // not using at the moment
                {
                    if (onUnitFinishTurn != null) onUnitFinishTurn.Invoke(this);
                }
            }
        }

        public override void Move(Vector3 position)
        {
            levelGrid.OpenCellAthisPosition(transform.position);
            targetPosition = position;
            stateMachine.ChangeState(new WalkingState(this));
            levelGrid.CloseCellAthisPosition(position);
        }

        public override void Damage(int damageTaken)
        {
            base.Damage(damageTaken);
        }

        public override void Kill()
        {
            base.Kill();
            aliveAIUnitsSO.Value = aliveAIUnitsSO.Value - 1;
            GameAbstractFactory gameAbtractFactory = GameFactoryProducer.GetFactory("AIUnitsFactory");
            gameAbtractFactory.RecycleAIUnit(this);
        }

        public override void ReachedTarget()
        {
            base.ReachedTarget();
            Idle();
        }

        public override void AttackFinished()
        {
            base.AttackFinished();
            attackTarget.Damage(attackPower);
            Idle();
        }

        public void Attack()
        {
            if (attackTarget != null) stateMachine.ChangeState(new AttackState(this));
        }

        private BTNodeStates CheckAIUnitsCountCritical() // if ai unit count is less or equal to player count, AI will attack the weak player
        {
            if (aliveAIUnitsSO.Value <= closetPlayerUnitsData.Count)
            {
                return BTNodeStates.SUCCESS;
            }
            else
            {
                return BTNodeStates.FAILURE;
            }
        }

        private BTNodeStates FindClosestPlayerUnit()
        {
            PlayerUnit closestPlayer = (PlayerUnit)closetPlayerUnitsData[0].unit;
            attackTarget = closestPlayer; 

            if (closestPlayer != null)
            {
                return BTNodeStates.SUCCESS;
            }
            else
            {
                return BTNodeStates.FAILURE;
            }
        }

        private BTNodeStates FindWeakestPlayerUnit()
        {
            PlayerUnit weakestPlayer = (PlayerUnit)closetPlayerUnitsData[0].unit; 
            for (int i = 0; i < closetPlayerUnitsData.Count; i++)
            {
                if (closetPlayerUnitsData[i].unit.currentHealth < weakestPlayer.currentHealth) weakestPlayer = (PlayerUnit)closetPlayerUnitsData[i].unit;
            }

            attackTarget = weakestPlayer; // setting attack target as weakestPlayer

            if (weakestPlayer != null)
            {
                return BTNodeStates.SUCCESS;
            }
            else
            {
                return BTNodeStates.FAILURE;
            }
        }

        private BTNodeStates MoveTowardsThePlayerUnit() // rewriter
        {
            if (stateMachine.IsCurrentStateName("idle") && !reachedDestination)
            {
                Vector3 ClosetCellPosToTarget = levelGrid.GetClosetOpenPositionAtTarget(transform.position, attackTarget.transform.position);
                Move(ClosetCellPosToTarget);
            }

            if (stateMachine.IsCurrentStateName("walk"))
            {
                return BTNodeStates.RUNNING;
            }
           
            if (stateMachine.IsCurrentStateName("idle"))
            {
                return BTNodeStates.SUCCESS;
            }
            else
            {
                return BTNodeStates.FAILURE;
            }
        }

        private BTNodeStates AttackPlayerUnit() // rewriter
        {
            if (stateMachine.IsCurrentStateName("idle") && !attackFinished )
            {
                Attack();
            }

            if (stateMachine.IsCurrentStateName("attack"))
            {
                return BTNodeStates.RUNNING;
            }
            if (stateMachine.IsCurrentStateName("idle"))
            {
                return BTNodeStates.SUCCESS;
            }
            else
            {
                return BTNodeStates.FAILURE;
            }
        }
    }
}
