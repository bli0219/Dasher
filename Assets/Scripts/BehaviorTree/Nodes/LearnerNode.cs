using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyBehaviorTree {
	public class LearnerNode : ICompositeNode {

		List<float> scores;
		bool ticked = false;
		int lastTicked = -1;
		float scoreSum = 0f;
		public float threshold = 1f;

		public LearnerNode (string name, BehaviorTree bt) : base(name, bt) { }

		public new LearnerNode Build (params ITreeNode[] children) {
			Children = children;
			scores = new List<float>();
			for (int i = 0; i != children.Length; i++) {
				scores.Add(threshold);
			}
			return this;
		}

		public override void Tick () {
			if (scores == null) {
				Debug.LogWarning("Scores not initialized");
				scores = new List<float>(new float[Children.Length]);
			}

			//for (int i = 0; i != Children.Length; i++) {
			//	Debug.Log(Children[i].Name + " has score " + scores[i]);
			//}

			if (!ticked) {
				ticked = true;
				int rand = WeightedRandom();
				lastTicked = rand;
				BehaviorTree.path.Push(Children[rand]);
			} else {
				if (BehaviorTree.lastStatus == NodeStatus.Success)
					UpdateScore(lastTicked, 1f);
				if (BehaviorTree.lastStatus == NodeStatus.Failure)
					UpdateScore(lastTicked, -1f);
				ticked = false;
				BehaviorTree.Finish(BehaviorTree.lastStatus);
			}
		}
		void UpdateScore (int index, float delta) {
			scores[index] += delta;
			scoreSum += delta;
			if (scores[index] < threshold) {
				for (int i = 0; i != scores.Count; i++) {
					scores[i] += 1f;
					scoreSum += 1f;
				}
			}
		}

		int WeightedRandom () {
			int index = 0;
			float r = Random.Range(0f, scoreSum);
			//Debug.Log("r is " + r);
			r -= scores[0];
			//Debug.Log("r minus scores[0] is " + r);
			while (r >= 0) {
				index++;
				r -= scores[index];
				//Debug.Log("r minus scores[" + index + "] is " + r);
			}

			return index;
		}
	}
}