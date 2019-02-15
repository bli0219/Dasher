using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree {
    public class SuccessRepeater : IDecoratorNode {

        bool started = false;

        public SuccessRepeater(string name, ITreeNode child, BehaviorTree BehaviorTree) : base(name, child, BehaviorTree) { }
        public SuccessRepeater(string name, BehaviorTree BehaviorTree) : base(name, BehaviorTree) { }

        public override void Tick() {
            // this condition is most frequently used, so put it first

            if (!started) {
                started = true;
                BehaviorTree.path.Push(Child);
            } else {
                if (BehaviorTree.lastStatus == NodeStatus.Success) {
                    BehaviorTree.path.Push(Child);
                } else {
                    BehaviorTree.Finish(NodeStatus.Failure);
                }
            }
        }
    }
}