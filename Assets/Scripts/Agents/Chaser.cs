using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviorTree;

public class Chaser : MonoBehaviour {

	public static List<Chaser> chasers = new List<Chaser>();
	Player player;
	Escaper escaper;
	Rigidbody rb;
	//BehaviorTree bt;

	Stage stage;
	public float followDrag = 0.9f;
	public float moveSpeed = 5f;
	public float bounceUp = 2000f;
	public bool up = false;
	//public bool usingBT;
	Vector3 offset;
	Vector3 startPos;
	float dashSpeed = 50f;
	//int hitCount = 0;
	//bool dashing = false;

	public bool pathfinding = true;
	PathFinder pf;
	Node start;
	Node goal;
	List<Node> path;
	public GameObject target;
	NodeManager nm;


	float alpha = 10f; //Degree to move when NaiveProtect();

	void Awake () {
		rb = GetComponent<Rigidbody>();
	}

	void Start () {
		chasers.Add(this);
		//player = Player.Instance;
		//escaper = Escaper.Instance;
		//offset = transform.position - escaper.transform.position;
		//startPos = transform.position;
		//stage = Stage.Idle;
		if (pathfinding) {
			pf = new PathFinder();
			target = Player.Instance.gameObject;
			nm = NodeManager.Instance;
		}
	}

	void Update () {
		if (Time.timeScale < 0.2f) {
			return;
		}
		if (pathfinding) {
			PathFollow();
		}
		//if (stage == Stage.Chase) NaiveFollow();
		//if (stage == Stage.Protect) HorizontalProtect();
	}



	void PathFollow () {
		//if (path == null) {
		FindPath();
		//}


		if (path.Count == 0) {
			Debug.Log("Path length of 0");
			return;
		}
		if (path[0] == nm.NearestNode(gameObject)) {
			path.RemoveAt(0);
		} else {
			Vector3 dir = (new Vector3(path[0].pos.x, 0, path[0].pos.y) - transform.position);
			dir.y = 0;
			Vector3 vel = dir.normalized * moveSpeed / 5f;
			rb.velocity = vel;
			//rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;
		}
	}

	void FindPath () {
		start = nm.NearestNode(this.gameObject);
		goal = nm.NearestNode(target.gameObject);
		pf.Initialize(start, goal);
		path = pf.AStarPath(start, goal);
	}

	public void ChaseStage () {
		rb.isKinematic = false;
		stage = Stage.Chase;
	}
	void NaiveFollow () {
		Vector3 newVelocity = (player.transform.position - transform.position).normalized * moveSpeed;
		rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;
	}

	//void NaiveFollowWrap () {
	//	StartCoroutine("NaiveFollowCR");
	//}

	//IEnumerator NaiveFollowCR () {
	//	float dist = Vector3.Distance(player.transform.position, transform.position);
	//	while (dist > 5f) {
	//		Vector3 newVelocity = (player.transform.position - transform.position).normalized * moveSpeed;
	//		rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;
	//		dist = Vector3.Distance(player.transform.position, transform.position);
	//		yield return new WaitForEndOfFrame();
	//	}
	//	bt.Finish(NodeStatus.Success);
	//}


	//void DashWrap () {
	//	StartCoroutine("DashCR");
	//}

	//IEnumerator DashCR () {
	//	int lastHitCount = hitCount;
	//	Vector3 dir = (player.transform.position - transform.position).normalized;
	//	rb.velocity = dir * dashSpeed;
	//	yield return new WaitForSecondsRealtime(1f);

	//	if (hitCount > lastHitCount) {
	//		bt.Finish(NodeStatus.Success);
	//	} else {
	//		bt.Finish(NodeStatus.Failure);
	//	}

	//}

	float SqDistTo (Vector3 pos) {
		return (transform.position.x - pos.x) * (transform.position.x - pos.x) + (transform.position.z - pos.z) * (transform.position.z - pos.z);
	}

	//void RadiusProtect() {
	//Vector3 playerDir = (player.transform.position - escaper.transform.position).normalized;
	//Vector3 curDir = (transform.position - escaper.transform.position).normalized;
	//float beta = Vector3.Angle(Vector3.right, curDir);
	//float angle = alpha + beta;
	//}

	void HorizontalProtect () {
		if (SqDistTo(player.transform.position) < 400f) {
			Vector3 targetPos = new Vector3(player.transform.position.x, 0.75f, escaper.transform.position.z + offset.z);
			Vector3 dir = targetPos - transform.position;
			Vector3 newVelocity = dir.normalized * moveSpeed;
			rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;
		} else {
			Vector3 targetPos = escaper.transform.position + offset;
			Vector3 dir = targetPos - transform.position;
			Vector3 newVelocity = dir.normalized * moveSpeed;
			rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;
		}
	}

	//void OnCollisionEnter (Collision collision) {
	//	if (collision.collider.tag == "Player" && !up) {
	//		if (player.dashing) {
	//			up = true;
	//			Debug.Log("enemy up");
	//			rb.AddForce(new Vector3(0, bounceUp, 0));
	//		}

	//		//if (dashing) {
	//		//	hitCount++;
	//		//}
	//	}

	//	if (collision.collider.tag == "Ground") {
	//		up = false;
	//	}
	//}
}
