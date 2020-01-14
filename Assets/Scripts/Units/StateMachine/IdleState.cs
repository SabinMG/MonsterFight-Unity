using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.MonsterFight
{
    public class IdleState : IState
    {
        private Unit _stateOwner;
        public IdleState(Unit owner) { this._stateOwner = owner; }
        private string name = "idle";
        public string Name { get { return name; } set { name = value; } }

        public void Enter()
        {
          // _stateOwner.animator
        }

        public void Execute()
        {
           
        }

        public void Exit()
        {

        } 
    }
}