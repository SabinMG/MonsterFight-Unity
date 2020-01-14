using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.AI
{
    public class BTSequence : BTNode
    {
        protected List<BTNode> _nodes = new List<BTNode>();
        protected Queue<BTNode> _nodeQueue = new Queue<BTNode>();

        protected BTNode _currentNode;

        public BTSequence(List<BTNode> nodes)
        {
            _nodes = nodes;
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodeQueue.Enqueue(_nodes[i]);
            }
        }

        //If any child fails retunrs failure
        public override BTNodeStates Execute()
        {
            if(_currentNode == null && _nodeQueue.Count >0)
            {
                _currentNode = _nodeQueue.Dequeue();
            }

            BTNodeStates nodeStatus = _currentNode.Execute();
            switch (nodeStatus)
            {
                case BTNodeStates.FAILURE:
                    return _nodeState = BTNodeStates.FAILURE;
                case BTNodeStates.SUCCESS:
                    if (_nodeQueue.Count > 0) _currentNode = _nodeQueue.Dequeue();
                    else return _nodeState = BTNodeStates.SUCCESS;
                    break;
                case BTNodeStates.RUNNING:
                    _nodeState = BTNodeStates.RUNNING;
                    break;
                default:
                    return _nodeState = BTNodeStates.SUCCESS;
            }

            return BTNodeStates.RUNNING;
        }
    }
}
