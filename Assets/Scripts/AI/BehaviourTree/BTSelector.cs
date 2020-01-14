using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.AI
{
    public class BTSelector : BTNode
    {
        protected List<BTNode> _nodes = new List<BTNode>();

        public BTSelector(List<BTNode> nodes)
        {
            _nodes = nodes;
        }

        //If return first succes node
        public override BTNodeStates Execute()
        {
            foreach (BTNode node in _nodes)
            {
                switch (node.Execute())
                {
                    case BTNodeStates.FAILURE:
                        continue;
                    case BTNodeStates.SUCCESS:
                        return _nodeState = BTNodeStates.SUCCESS;
                    case BTNodeStates.RUNNING:
                        return _nodeState = BTNodeStates.RUNNING;
                    default:
                        continue;
                }
            }
            return _nodeState = BTNodeStates.FAILURE;
        }
    }
}
