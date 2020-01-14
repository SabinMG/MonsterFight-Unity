using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.AI
{
    public enum BTNodeStates
    {
        SUCCESS,
        FAILURE,
        RUNNING,
    }

    public abstract class BTNode
    {
        protected BTNodeStates _nodeState;

        public BTNodeStates nodeState
        {
            get { return _nodeState; }
        }

        public BTNode() { }
        public abstract BTNodeStates Execute();
    }
}
