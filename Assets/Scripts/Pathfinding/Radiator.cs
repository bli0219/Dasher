using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiator : MonoBehaviour {

	public float cost;
	Node node;
	NodeManager nm;

	void Start () {
		nm = NodeManager.Instance;
		node = nm.NearestNode(gameObject);
	}
	public bool Moved () {
		Node current = nm.NearestNode(gameObject);
		if (current != node) {
			node = current;
			return true;
		} else {
			return false;
		}
	}
}
