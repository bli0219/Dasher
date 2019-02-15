using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree {
    public abstract class IDecoratorNode : ITreeNode  {
        // inherit Name
        public ITreeNode Child;

        protected IDecoratorNode (string name, ITreeNode child, BehaviorTree behaviorTree) {
            Name = name;
            Child = child;
            BehaviorTree = behaviorTree;
        }
        protected IDecoratorNode(string name, BehaviorTree behaviorTree) {
            Name = name;
            BehaviorTree = behaviorTree;
        }
        public IDecoratorNode Build(ITreeNode child) {
            Child = child;
            return this;
        }
    }
}