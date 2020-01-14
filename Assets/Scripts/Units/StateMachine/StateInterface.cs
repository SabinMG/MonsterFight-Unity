using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.MonsterFight
{
    public interface IState
    {
        string Name { get; set; }
        void Enter();
        void Execute();
        void Exit();
    }
}
