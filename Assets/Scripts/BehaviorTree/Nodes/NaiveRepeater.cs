using System.Collections;
using System.Collections.Generic;

namespace MyBehaviorTree  {

    public class NaiveRepeater : IDecoratorNode  {

        public NaiveRepeater(string name, ITreeNode child, BehaviorTree BehaviorTree) : base (name, child, BehaviorTree) { }
        public NaiveRepeater(string name, BehaviorTree BehaviorTree) : base(name, BehaviorTree) { }

        public override void Tick() {
            BehaviorTree.path.Push(Child);
        }
    }

}