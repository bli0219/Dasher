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
    float dist = 2f;

    float time = 0f;
    public List<List<Node>> nodeMap;
    //List<List<int>> activeMap;
    //public Node[,] nodeMap;
    List<CostObject> costObjects;
        GameObject target;

    int xCount = 0;
    int yCount = 0;
    int count = 0;


    void Awake() {
        Instance = this;

        //time = Time.realtimeSinceStartup;
        CreateNodes();
        //Debug.Log("Creating " + count + " Nodes takes " + (Time.realtimeSinceStartup - time));
        //time = Time.realtimeSinceStartup;
        AssignNeighbors();
        costObjects = new List<CostObject>(FindObjectsOfType<CostObject>());
        //Debug.Log("Assigning Neighbors takes " + (Time.realtimeSinceStartup - time));
    }

    void FixedUpdate() {
        UpdateThreatCosts();
    }

    float SqrDist(Vector2 v2, Vector3 v3) {
        return (v2.x - v3.x) * (v2.x - v3.x) + (v2.y - v3.z) * (v2.y - v3.z);
    }

    public void UpdateThreatCosts() {
        foreach (List<Node> row in nodeMap) {
            foreach (Node node in row) {
                float t_cost = 0f;
                foreach (CostObject costObject in costObjects) {
                    t_cost += costObject.cost / SqrDist(node.pos, costObject.transform.position);
                }
                node.t = t_cost;
                //Debug.Log("Node " + node.pos + " has cost " + node.t);
            }
        }
    }

    public bool Contains(Node start, Node end) {
        bool found1 = false;
        bool found2 = false;
        while (!found1 || !found2) {
            foreach(List<Node> row in nodeMap) {
                foreach (Node node in row) {
                    if (node == start) found1 = true;
                    if (node == end) found2 = true;
                }
            }
        }
        return found1 && found2;
    }

    public Node NearestNode(GameObject go) {
        Vector3 goPos = go.transform.position;
        Debug.Log("xC" + xCount + " yC" + yCount);
        int numX = (int)Mathf.Round((goPos.x - bottomLeft.x) / dist);
        int numY = (int)Mathf.Round((goPos.z - bottomLeft.y) / dist);

        if (numX < 0 || numY < 0 || numX >= xCount || numY >= yCount) {
            numX = Mathf.Clamp(numX, 0, xCount - 1);
            numY = Mathf.Clamp(numY, 0, yCount - 1);
        }
        nearest = nodeMap[numY][numX];
        Debug.Log("nearest node to " + goPos + " is " + numX + "," + numY + " " + nearest.pos);

        return nearest;
    }

    public void ChangeTarget(GameObject _target) {
        target = _target;
    }

    void CreateNodes () {
        yCount = Mathf.CeilToInt((topRight.y - bottomLeft.y) / dist);
        xCount = Mathf.CeilToInt((topRight.x - bottomLeft.x) / dist);
        Debug.Log("Max num of nodes = " + xCount * yCount);
        nodeMap = new List<List<Node>>();
        //activeMap = new List<List<int>>();

	    for (int y = 0; y < yCount; y ++) {
            List<Node> rowNode = new List<Node>();
            //List<int> rowActive = new List<int>();
            for (int x = 0; x < xCount; x++) {
                float _x = bottomLeft.x + x * dist;
                float _y = bottomLeft.y + y * dist;

                Collider[] colliders = Physics.OverlapBox(Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f));

                bool block = false;
                for (int i = 0; i != colliders.Length; i++) {
                    if (colliders[i].tag == "Block") {
                        block = true;
                        break;
                    }
                }
                if (block) {
                    rowNode.Add(null);
                    //Debug.Log("null node(" + y + "," + x + ") at " + "(" + _y + "," + _x + ")");
                } else {
                    rowNode.Add(new Node(_x, _y));
                    //Debug.Log("node(" + y + "," + x + ") at " + "(" + _y + "," + _x + ")");
                    count++;
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
    void AssignNeighbors() { 

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
