using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage {
    Idle,
    Escape,
    Chase,
    End
}
public class Escaper: MonoBehaviour {

    static public Escaper Instance;
    public Rigidbody rb;
    BoxCollider col;
    Player player;
    Vector2 move;
    public float shieldScale = 4.285714f;
    Vector3 shieldOffset = new Vector3(0f, 1.5f, 0f);
    Vector3 endPos = new Vector3(0, 0.35f, 48f);

    public Stage stage = Stage.Escape;
    public bool pause = false;
    bool bouncing = false;
    int touchCount = 0;
    public float dodgeDistance = 10f;
    public float knockForce = 50f;
    public float escapeDrag = 0.9f;
    public float chaseDrag = 0.95f;
    public float rotateDrag = 0.99f;
    public float moveSpeed = 10f;
    public float bounceUp = 2000f;
    public float knockOffset = 0.5f;
    int dashCount = -1;
    float diag = 1/Mathf.Sqrt(2);
    List<Vector2> nexts;
    List<Vector2> posList = new List<Vector2>{
        Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero,
    };
    float bound = 40f;

    void Awake() {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        nexts = new List<Vector2> {
            new Vector2(1,0),
            new Vector2(-1,0),
            new Vector2(0,1),
            new Vector2(0,-1),
            new Vector2(diag,diag),
            new Vector2(-diag,-diag),
            new Vector2(diag,-diag),
            new Vector2(-diag,diag),
        };
    }

    void Start() {
        player = Player.Instance;
    }

    void Update() {
        if (Time.timeScale < 0.2f) return;
        if (stage == Stage.Idle) NaiveDodge();        
        if (stage == Stage.Escape) ThreatEscape();
        if (stage == Stage.Chase) NaiveChase();
        if (stage == Stage.End) {
            rb.velocity = Vector3.zero;
            transform.position = endPos;
            return;
        }
        Vector3 face = rb.velocity;
        face.y = 0f;
        if (!bouncing) {
            transform.forward = rotateDrag * transform.forward + (1 - rotateDrag) * face;
        }
    }
    public void EscapeStage() {
        moveSpeed = 8f;
        stage = Stage.Escape;
    }

    public void ChaseStage() {
        stage = Stage.Chase;
        tag = "Shield";
        transform.GetChild(1).gameObject.SetActive(true);
        col.size = new Vector3(shieldScale, shieldScale, shieldScale);
        col.center = shieldOffset;
        rb.velocity = Vector3.zero;
        moveSpeed = 30f;
        knockOffset = 0.9f;
    }

    public void EndStage() {
        stage = Stage.End;
        rb.velocity = Vector3.zero;
        transform.position = endPos;
        transform.GetChild(1).gameObject.SetActive(false);
    }

    float SqDist(Vector2 v2, Vector3 pos) {
        return (v2.x - pos.x)*  (v2.x - pos.x) +(v2.y - pos.z) * (v2.y - pos.z);
    }

    float SqDistTo(Vector3 pos) {
        return (transform.position.x - pos.x) * (transform.position.x - pos.x) + (transform.position.z - pos.z) * (transform.position.z - pos.z);
    }

    void NaiveDodge() {
        float distToPlayer = SqDistTo(player.gameObject.transform.position);
        if (distToPlayer < dodgeDistance) {
            Vector3 dir = (transform.position- player.transform.position).normalized;
            Vector3 newVelocity = dir.normalized * moveSpeed;
            rb.velocity = escapeDrag * rb.velocity + (1 - escapeDrag) * newVelocity;
        } else {
            rb.velocity = escapeDrag * rb.velocity; 
        }
    }
    void ThreatEscape() {
        float distToPlayer = SqDistTo(player.gameObject.transform.position);
        if (distToPlayer < 400f) {
            for (int i = 0; i != 8; i++) {
                posList[i] = new Vector2(transform.position.x + nexts[i][0], transform.position.z + nexts[i][1]);
            }
            float largest = 0f;
            for (int i = 0; i != posList.Count; i++) {
                if (posList[i].x > bound || posList[i].x < -bound || posList[i].y > bound || posList[i].y < -bound) {
                    continue;
                }
                float dist = SqDist(posList[i], player.transform.position);
                if (dist > largest) {
                    largest = dist;
                    move = posList[i];
                }
            }
            Vector3 dir = new Vector3(move.x - transform.position.x, 0, move.y - transform.position.z);
            Vector3 newVelocity = dir.normalized * moveSpeed;
            rb.velocity = escapeDrag * rb.velocity + (1 - escapeDrag) * newVelocity;
        } 
    }


    void NaiveChase() {
        Vector3 dir =  (player.transform.position - transform.position).normalized;
        dir.y = 0f;
        for (int i = 0; i != 1; i++) {
            if (!Physics.Raycast(transform.position, dir, 2f)) {
                break;
            }
            if (Random.value > 0.5f)
                dir += transform.right;
            else
                dir -= transform.right;
        }
        Vector3 newVelocity = dir.normalized * moveSpeed;
        rb.velocity = chaseDrag * rb.velocity + (1 - chaseDrag) * newVelocity;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player") {
            if (player.dashing) {
                if (dashCount != player.dashCount) {
                    dashCount = player.dashCount;
                    Vector3 dashDir = player.rb.velocity.normalized;
                    Vector3 distanceDir = (transform.position- player.transform.position).normalized;
                    Vector3 knock = ((1-knockOffset)* dashDir + knockOffset* distanceDir).normalized * knockForce;
                    rb.AddForce(knock);
                    bouncing = true;
                    Invoke("StopBouncing", 0.2f);
                    StageManager.Instance.EscaperHit();
                }
            }
        }
    }

    void StopBouncing() {
        bouncing = false;
    }
}
