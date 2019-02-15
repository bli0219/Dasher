using System.Collections;
using MyBehaviorTree;
using System;

namespace MyBehaviorTree {

    public class ConditionNode : ITreeNode {

        // Fn returns only success & failure, and finishes immediately
        Func<bool> Fn;
        float param;
        ITreeNode Child;
        bool condition;
        bool started;

        public ConditionNode(string name, Func<bool> fn, ITreeNode child, BehaviorTree tree) {
            Fn = fn;
            Name = name;
            BehaviorTree = tree;
            Child = child;
        }

        public ConditionNode(string name, Func<bool> fn, BehaviorTree tree) {
            Fn = fn;
            Name = name;
            BehaviorTree = tree;
        }

        public ConditionNode Build(ITreeNode child) {
            Child = child;
            return this;
        }

        // If condition met, push child
        // Return Success when child succeeds, Failure otherwise
        // Ticked only twice: when pushed and when poped
        public override void Tick() {
            if (!started) {
                started = true; 
                if (Fn()) {
                    BehaviorTree.path.Push(Child);
                } else {
                    BehaviorTree.Finish(NodeStatus.Failure);
                }
            } else {
                BehaviorTree.Finish(BehaviorTree.lastStatus);
            }
        }

    }
}