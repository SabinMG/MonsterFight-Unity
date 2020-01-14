using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceOrigin.AI
{
    public class BehaviourTree
    {
        private BTNode _rootNode;
        private BTNodeStates _currentStatus;

        public BehaviourTree(BTNode root)
        {
            InitTree(root);
        }

        private void InitTree(BTNode root)
        {
            _rootNode = root;
        }

        public BTNodeStates Behave()
        {
            return _currentStatus = _rootNode.Execute();
        }
    }
}
