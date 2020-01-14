using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SpaceOrigin.Utilities;

namespace SpaceOrigin.MonsterFight
{
    public struct DistanceData
    {
        public Unit unit;
        public float distanceFromUnit;
    }

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class Unit : MonoBehaviour,IDamageable, IKillable
    {
        public int maxHealth = 10;
        public int attackPower = 1; // how much an attak will do on opponent
        public float attackTime = 2.0f;
        public float targetReachThreshold;
        public GameObject activeIndicator;
        public HealthBarUI healthBar;

        public bool reachedDestination { get; set;}
        public bool attackFinished { get; set; }
        public StateMachine stateMachine { get; set;}
        public NavMeshAgent navMeshAgent { get; set; }
        public Animator animator { get; set;}
        public Vector3 targetPosition { get; set;}
        public bool alive { get; set; }
        public Unit attackTarget { get; set; }
        public int currentHealth { get; set; }
        public LevelGrid levelGrid { get; set; }

        public Action OnReachTarget; // will update code later to events
        public Action OnAttackFinished;

        public delegate void UnitLifeDelegate(Unit unit);
        public UnitLifeDelegate onUnitLostLifeEvent;  // binds to unit controllers

        public virtual void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            stateMachine = new StateMachine();
            UnSelected();
            currentHealth = maxHealth;
        }

        public void Selected()
        {
            activeIndicator.SetActive(true);
        }

        public void UnSelected()
        {
            activeIndicator.SetActive(false);
        }

        protected void SetActiveIndicatorColor(Color color)
        {
            activeIndicator.GetComponent<SpriteRenderer>().color = color; //avoid get component TODO: rewrite
        }

        public virtual void Execute()
        {
        }

        public virtual void Idle()
        {

        }

        public virtual void Move(Vector3 position)
        {
     
        }

        public virtual void ReachedTarget()
        {
            if (OnReachTarget != null) OnReachTarget.Invoke();
        }

        public virtual void AttackFinished()
        {
            if (OnAttackFinished != null) OnAttackFinished.Invoke();  
        }

        public virtual void Damage(int damageTaken)
        {
            currentHealth -= damageTaken;
            healthBar.SetValue((float)currentHealth/ (float)maxHealth);
            if (currentHealth <= 0) Kill();
        }

        public virtual void Kill()
        {
            alive = false;
            if (onUnitLostLifeEvent != null) onUnitLostLifeEvent.Invoke(this);
        }
    }
}

