using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree {
	public class SelectorNode : ICompositeNode {

		// activeChild = -1 on declaration
		public SelectorNode (string name, ITreeNode[] children, BehaviorTree bt) : base(name, children, bt) { }
		public SelectorNode (string name, BehaviorTree bt) : base(name, bt) { }

		// use global variable to pass status
		public override void Tick () {

			if (activeChild == -1) {
				// no child ticked, ignore LastStatus, tick first child
				// assuming Children.Length > 0
				try {
					activeChild = 0;
					BehaviorTree.path.Push(Children[activeChild]);
				} catch {
					Debug.LogError("Empty Children[]");
				}

			} else {
				// some child ticked
				if (BehaviorTree.lastStatus == NodeStatus.Success) {
					// succeed if any success
					activeChild = -1;
					BehaviorTree.Finish(NodeStatus.Success);
				} else {
					// no success yet
					if (activeChild < Children.Length - 1) {
						// if last activeChild is not the last
						activeChild++;
						BehaviorTree.path.Push(Children[activeChild]);
					} else {
						// reached the last, still no success
						activeChild = -1;
						BehaviorTree.Finish(NodeStatus.Failure);
					}
				}

			}
		}
	}
}