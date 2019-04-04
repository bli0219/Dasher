using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node> {

	public Vector2 pos;
	public float f; // total cost
	public float g; // path cost from start to this
	public float h; // heuristic cost (straight-line distance)
	public float r; // radiation cost
	public float gr; // g + r
	public Node parent; //parent has the lowest cost from start
	public List<Node> neighbors;

	public Node (float posX, float posY) {
		pos = new Vector2(posX, posY);
		neighbors = new List<Node>();
		f = UnityEngine.Random.Range(0f, 10f);
	}

	public int CompareTo (Node other) {
		if (f < other.f) return -1;
		if (f > other.f) return 1;
		return 0;
	}

	public void CalcF () {
		f = g + h + r;
	}


	//float SqrDist(float x, float y) {
	//    return x * x + y * y;
	//}

	//public void UpdateCosts(List<Enemy> enemies, Enemy target) {
	//    float cost = 0f;
	//    foreach(Enemy e in enemies) {
	//        float sqrDist = SqrDist(e.transform.position.x - pos.x, e.transform.position.y - pos.y);
	//        float scaledThreat = e.threat / sqrDist;
	//        cost += scaledThreat;
	//    }

	//    Vector3 targetPos = target.transform.position;
	//    float scaledAttraction = target.attraction / SqrDist(targetPos.x - pos.x, targetPos.y - pos.y);
	//    cost -= scaledAttraction;
	//}


}
