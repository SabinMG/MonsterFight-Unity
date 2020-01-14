using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.AI
{
    public class BTActionNode : BTNode
    {
        public delegate BTNodeStates ActionNodeDelegate();
        private ActionNodeDelegate _action;

        public BTActionNode(ActionNodeDelegate action)
        {
            _action = action;
        }

        public override BTNodeStates Execute()
        {
            switch (_action())
            {
                case BTNodeStates.SUCCESS:
                    return _nodeState = BTNodeStates.SUCCESS;
                case BTNodeStates.FAILURE:
                   return _nodeState = BTNodeStates.FAILURE;
                case BTNodeStates.RUNNING:
                    return _nodeState = BTNodeStates.RUNNING;
                default:
                    return _nodeState = BTNodeStates.FAILURE;
            }
        }
    }
}
