using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wintellect.PowerCollections;

public class NodeManager : MonoBehaviour {

	public static NodeManager Instance;
	//public static GameObject target;
	public static Node nearest;
	//    public GameObject enemyParent;
	public bool is3D;
	public bool isTest;
	public Vector2 bottomLeft;
	public Vector2 topRight;
	public float step = 2f;
	//public float radiat 
	float time = 0f;
	public List<List<Node>> nodeMap;
	//List<List<int>> activeMap;
	//public Node[,] nodeMap;
	List<Radiator> radiators;
	GameObject target;
	public GameObject nodePrefab;

	int xCount = 0;
	int yCount = 0;
	int count = 0;


	void Awake () {
		Instance = this;

		//time = Time.realtimeSinceStartup;
		CreateNodes();
		//Debug.Log("Creating " + count + " Nodes takes " + (Time.realtimeSinceStartup - time));
		//time = Time.realtimeSinceStartup;
		AssignNeighbors();
		radiators = new List<Radiator>(FindObjectsOfType<Radiator>());
		//Debug.Log("Assigning Neighbors takes " + (Time.realtimeSinceStartup - time));
	}

	void Start () {
		UpdateCost();
	}

	void FixedUpdate () {
		CostRadiation();
	}

	float SqrDist (Vector2 v2, Vector3 v3) {
		return (v2.x - v3.x) * (v2.x - v3.x) + (v2.y - v3.z) * (v2.y - v3.z);
	}

	public void CostRadiation () {
		bool moved = false;
		foreach (Radiator radiator in radiators) {
			if (radiator.Moved()) {
				moved = true;
				break;
			}
		}
		if (moved) {
			Debug.Log("moved");
			UpdateCost();
		}
		//foreach (Radiator radiator in radiators) {
		//	foreach (List<Node> row in nodeMap) {
		//		foreach (Node node in row) {
		//			if (node == null) continue;
		//			float dist = SqrDist(node.pos, radiator.transform.position);
		//			if (dist < 20) {
		//				node.r += radiator.cost / (1 + SqrDist(node.pos, radiator.transform.position));
		//			}
		//		}
		//	}
		//}
	}

	void UpdateCost () {
		if (radiators.Count == 0) {
			return;
		}
		float t = Time.realtimeSinceStartup;
		foreach (List<Node> row in nodeMap) {
			foreach (Node node in row) {
				if (node == null) continue;
				float r_cost = 0f;
				foreach (Radiator radiator in radiators) {
					float dist = SqrDist(node.pos, radiator.transform.position);
					if (dist < 10) {
						//r_cost += radiator.cost / (1 + SqrDist(node.pos, radiator.transform.position));
						r_cost += radiator.cost / (1 + LogDist(node.pos, radiator.transform.position));
						Debug.Log("r cost " + r_cost);
					}
				}
				node.r = r_cost;
			}
		}
		Debug.Log("took " + (Time.realtimeSinceStartup - t));
	}

	float LogDist (Vector2 a, Vector2 b) {
		float logDist = Mathf.Log10(Vector2.Distance(a, b));
		Debug.Log(logDist);
		return logDist;

	}

	public bool Contains (Node start, Node end) {
		bool found1 = false;
		bool found2 = false;
		while (!found1 || !found2) {
			foreach (List<Node> row in nodeMap) {
				foreach (Node node in row) {
					if (node == start) found1 = true;
					if (node == end) found2 = true;
				}
			}
		}
		return found1 && found2;
	}

	public Node NearestNode (GameObject go) {
		Vector3 goPos = go.transform.position;
		//Debug.Log("xC" + xCount + " yC" + yCount);
		int numX = (int)Mathf.Round((goPos.x - bottomLeft.x) / step);
		int numY = (int)Mathf.Round((goPos.z - bottomLeft.y) / step);

		if (numX < 0 || numY < 0 || numX >= xCount || numY >= yCount) {
			numX = Mathf.Clamp(numX, 0, xCount - 1);
			numY = Mathf.Clamp(numY, 0, yCount - 1);
		}
		nearest = nodeMap[numY][numX];

		//while (nearest == null) {
		//	nearest = 
		//}
		//Debug.Log("nearest node to " + goPos + " is " + numX + "," + numY + " " + nearest.pos);

		return nearest;
	}

	public void ChangeTarget (GameObject _target) {
		target = _target;
	}

	void CreateNodes () {
		yCount = Mathf.CeilToInt((topRight.y - bottomLeft.y) / step);
		xCount = Mathf.CeilToInt((topRight.x - bottomLeft.x) / step);
		Debug.Log("Max num of nodes = " + xCount * yCount);
		nodeMap = new List<List<Node>>();
		//activeMap = new List<List<int>>();

		for (int y = 0; y < yCount; y++) {
			List<Node> rowNode = new List<Node>();
			//List<int> rowActive = new List<int>();
			for (int x = 0; x < xCount; x++) {
				float _x = bottomLeft.x + x * step;
				float _y = bottomLeft.y + y * step;

				Collider[] colliders = Physics.OverlapBox(new Vector3(_x, 0f, _y), new Vector3(1f, 1f, 1f));
				bool block = false;
				for (int i = 0; i != colliders.Length; i++) {
					if (colliders[i].tag == "Block") {
						block = true;
						break;
					}
				}
				if (block) {
					rowNode.Add(null);
				} else {
					rowNode.Add(new Node(_x, _y));
					count++;
					GameObject nodeGO = Instantiate(nodePrefab, new Vector3(_x, 0, _y), Quaternion.identity);
					nodeGO.name = "Node " + _x + "," + _y;
					nodeGO.transform.parent = transform;
				}
			}
			nodeMap.Add(rowNode);
			//activeMap.Add(rowActive);
		}
		Debug.Log("Actual num of nodes = " + count);
	}

	/*
     * TODO: CAN WE JUST ASSUME NEIGHBORS ARE +-1 ON X AND Y WITHOUT STORING THEM?
     */
	void AssignNeighbors () {

		time = Time.realtimeSinceStartup;
		for (int y = 0; y < yCount; y++) {
			for (int x = 0; x < xCount; x++) {

				if (nodeMap[y][x] != null) {
					List<Node> neighbors = nodeMap[y][x].neighbors;
					//Debug.Log("node(" + y + "," + x + ") has neighbors");
					// Loop through 8 neighbors 1 index away
					for (int offsetY = -1; offsetY <= 1; offsetY++) {
						for (int offsetX = -1; offsetX <= 1; offsetX++) {
							// if not the node itself
							if (offsetX != 0 || offsetY != 0) {
								// actual x, y 
								int indexY = y + offsetY;
								int indexX = x + offsetX;
								// if within bounds
								if (indexY < yCount && indexY >= 0
									&& indexX < xCount && indexX >= 0) {
									// not checking null, add null if it's null
									neighbors.Add(nodeMap[indexY][indexX]);
									//Debug.Log("neighbor(" + indexY + "," + indexX + ")");

								}
							}
						}
					}
				}

			}
		}
		//Debug.Log("assigning neighbors for " + count + " nodes takes " + (Time.realtimeSinceStartup - time) + "seconds");
	}

}
