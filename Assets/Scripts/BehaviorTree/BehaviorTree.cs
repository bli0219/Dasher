using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree {
	public class BehaviorTree : MonoBehaviour {

		public Stack<ITreeNode> path;
		public ITreeNode activeNode;
		public NodeStatus lastStatus;
		public NodeStatus actionStatus;
		public bool actionTaken;
		public ITreeNode root;


		public void Build (ITreeNode _root) {
			path = new Stack<ITreeNode>();
			root = _root;
			path.Push(root);
		}

		public void Tick () {
			while (!actionTaken) {
				Debug.Log("Ticking " + path.Peek().Name);
				path.Peek().Tick();
			}
		}

		//public void OneTick() {
		//    Debug.Log("Ticking " + path.Peek().Name);
		//    path.Peek().Tick();
		//}

		public void PrintPath () {
			string str = "";
			foreach (ITreeNode node in path) {
				str = str + node.Name + "(" + node.GetType() + ")" + " -> ";
			}
			Debug.Log(str);
		}

		IEnumerator Traverse () {
			while (path.Count > 0) {
				path.Peek().Tick();
				yield return null;
			}
			yield return null;
			Debug.Log("Traversal Ended.");
		}

		public void Run () {
			actionTaken = true;
			lastStatus = NodeStatus.Running;
		}

		public void Finish (NodeStatus status) {
			if (path.Peek().GetType() == typeof(ActionNode)) {
				actionTaken = false;
			}
			Debug.Log("popping " + path.Peek().Name + " as " + status);
			path.Pop();
			lastStatus = status;
		}


	}
}
