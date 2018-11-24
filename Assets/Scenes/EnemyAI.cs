using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    PlayerControl player;
    Rigidbody rb;

    public float followDrag = 0.9f;
    public float moveSpeed = 5f;
    public float bounceUp = 2000f;
    public bool up= false;
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    void Start () {
        player = PlayerControl.instance;		
	}
	
	// Update is called once per frame
	void Update () {
        NaiveFollow();
	}

    void NaiveFollow() {
        Vector3 newVelocity = (player.transform.position - transform.position).normalized * moveSpeed;
        rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;

    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player" && !up) {
            if (player.dashing && player.ult) {
                up = true;
                Debug.Log("enemy up");
                rb.AddForce(new Vector3(0, bounceUp, 0));
            }
        }

        if (collision.collider.tag == "Ground") {
            up = false;
        }
    }
}
