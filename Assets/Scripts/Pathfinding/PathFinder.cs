using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wintellect.PowerCollections;

public class PathFinder {

	// Use this for initialization
	OrderedSet<Node> open;
	HashSet<Node> closed;
	List<List<Node>> map;
	Node start;
	Node goal;
	Node current;

	public PathFinder () {
		map = NodeManager.Instance.nodeMap;
		open = new OrderedSet<Node>();
		closed = new HashSet<Node>();
	}

	public void UpdateMap () {
		map = NodeManager.Instance.nodeMap;
	}

	public bool Initialize (Node _start, Node _goal) {
		if (!NodeManager.Instance.Contains(_start, _goal)) {
			return false;
		}
		start = _start;
		goal = _goal;
		open.Clear();
		closed.Clear();
		foreach (List<Node> list in map) {
			foreach (Node node in list) {
				if (node != null) {
					node.g = float.MaxValue;
					node.h = Heuristic(node, goal);
					node.parent = null;
				}
			}
		}
		start.g = start.r;
		start.CalcF();
		open.Add(start);
		return true;
	}

	public List<Node> RecoverPath (Node goal) {
		List<Node> path = new List<Node>();
		path.Add(goal);

		Node cur = current;
		while (cur != start) {
			path.Add(cur);
			cur = cur.parent;
		}
		path.Reverse();
		return path;
	}

	public List<Node> AStarPath (Node _start, Node _goal) {
		float t = Time.realtimeSinceStartup;
		start = _start;
		goal = _goal;
		while (open.Count != 0) {
			current = open.GetFirst();
			if (current == goal) {
				Debug.Log("A* runtime " + (Time.realtimeSinceStartup - t));
				return RecoverPath(current);
			}
			open.RemoveFirst();
			closed.Add(current);

			foreach (Node neighbor in current.neighbors) {
				if (neighbor == null)
					continue;
				if (closed.Contains(neighbor)) //evaluated
					continue;
				float new_g = current.g + DistCost(current, neighbor) + current.r;
				if (new_g < neighbor.g) {
					neighbor.parent = current;
					neighbor.g = new_g;
					neighbor.CalcF();
				}
				if (!open.Contains(neighbor)) {
					//if (neighbor == goal) {
					//	Debug.Log(neighbor.parent);
					//}
					open.Add(neighbor);
				}
			}
		}
		Debug.LogError("No path found. " + (Time.realtimeSinceStartup - t));
		//Debug.Log()
		return null;
	}

	public float h_sqrDist (Node node) {
		return Vector2.SqrMagnitude(goal.pos - node.pos);
	}

	float Heuristic (Node a, Node b) {
		//return Vector2.SqrMagnitude(a.pos - b.pos);
		try {
			return Vector2.Distance(a.pos, b.pos);
		} catch {

			//Debug.Log(a);
			//Debug.Log(b);
			//Debug.Log(a.pos);
			//Debug.Log(b.pos);
			return 0;
		}
		//return Mathf.Min(Mathf.Abs(a.pos.x - b.pos.x), Mathf.Abs(a.pos.y - b.pos.y));
	}

	float DistCost (Node a, Node b) {
		//return Mathf.Min(Mathf.Abs(a.pos.x - b.pos.x), Mathf.Abs(a.pos.y - b.pos.y));
		//return Vector2.Distance(a.pos, b.pos);
		return Vector2.SqrMagnitude(a.pos - b.pos);
	}

}
