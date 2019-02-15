using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree {
    public abstract class ITreeNode {

        public string Name;

        // Tick() modifies global variable LastStatus as message passing
        public abstract void Tick();
        protected BehaviorTree BehaviorTree;

        //public void AddToObserver() {
        //    TreeBehaviorTree.Path.Push (this);
        //}
        //public void RemoveFromObserver() {
        //    TreeBehaviorTree.Path.Pop();
        //}
    }
}
