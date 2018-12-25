using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser: MonoBehaviour {

    public static List<Chaser> chasers = new List<Chaser>();
    Player player;
    Escaper escaper;
    Rigidbody rb;
    public bool chasing = false;

    public float followDrag = 0.9f;
    public float moveSpeed = 5f;
    public float bounceUp = 2000f;
    public bool up = false;
    Vector3 offset;
    Vector3 startPos;

    float alpha = 10f; //Degree to move when NaiveProtect();

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    void Start () {
        chasers.Add(this);
        player = Player.Instance;
        escaper = Escaper.Instance;
        offset = transform.position - escaper.transform.position;
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeScale <0.2f) {
            return;
        }
        if (chasing) {
            NaiveFollow();
        } else {
            HorizontalProtect();
        }
    }

    void NaiveFollow() {
        Vector3 newVelocity = (player.transform.position - transform.position).normalized * moveSpeed;
        rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;
    }

    float SqDistTo(Vector3 pos) {
        return (transform.position.x - pos.x) * (transform.position.x - pos.x) + (transform.position.z - pos.z) * (transform.position.z - pos.z);
    }

    //void RadiusProtect() {
        //Vector3 playerDir = (player.transform.position - escaper.transform.position).normalized;
        //Vector3 curDir = (transform.position - escaper.transform.position).normalized;
        //float beta = Vector3.Angle(Vector3.right, curDir);
        //float angle = alpha + beta;
    //}

    void HorizontalProtect() {
        if (SqDistTo(player.transform.position) < 400f) {
            Vector3 targetPos = new Vector3(player.transform.position.x, 0.75f, escaper.transform.position.z + offset.z);
            Vector3 dir = targetPos - transform.position;
            Vector3 newVelocity = dir.normalized * moveSpeed;
            rb.velocity = followDrag * rb.velocity + (1 - followDrag) * newVelocity;
        } else {
            rb.velocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player" && !up) {
            if (player.dashing ) {
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
