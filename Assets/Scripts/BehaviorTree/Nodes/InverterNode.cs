using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree {
    public class InverterNode : IDecoratorNode {

        bool started = false;
        public InverterNode(string name, ITreeNode child, BehaviorTree BehaviorTree) : base(name, child, BehaviorTree) { }
        public InverterNode(string name, BehaviorTree BehaviorTree) : base(name, BehaviorTree) { }

        public override void Tick() {

            if (!started) {
                BehaviorTree.path.Push(Child);
            } else {
                NodeStatus inverted = BehaviorTree.lastStatus == NodeStatus.Success ? NodeStatus.Failure : NodeStatus.Success;
                BehaviorTree.Finish(inverted);
            }
        }

    }
}