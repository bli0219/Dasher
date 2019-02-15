using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree {
	public abstract class ICompositeNode : ITreeNode {

		// inherit Name from ITreeNode
		protected ITreeNode[] Children;
		protected int activeChild = -1;

		protected ICompositeNode (string name, ITreeNode[] children, BehaviorTree behaviorTree) {
			Name = name;
			Children = children;
			BehaviorTree = behaviorTree;
		}

		protected ICompositeNode (string name, BehaviorTree behaviorTree) {
			Name = name;
			BehaviorTree = behaviorTree;
		}

		public ICompositeNode Build (params ITreeNode[] children) {
			Children = children;
			return this;
		}
	}
}
