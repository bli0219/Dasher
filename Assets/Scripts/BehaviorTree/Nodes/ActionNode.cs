using System.Collections;
using MyBehaviorTree;
using System;

namespace MyBehaviorTree {

    public class ActionNode : ITreeNode {

        // The action to perform, passed by user when initialized
        // Needs to be defined wiht clear feedback (success, failure, running)
        Action Task;
        
        public ActionNode(string name, Action task, BehaviorTree behaviorTree) {
            Task = task;
            Name = name;
            BehaviorTree = behaviorTree;
        }

        // Task() will call Finish(status) to pop this node
        public override void Tick() {
            BehaviorTree.Run();
            Task();
        }
    }
}