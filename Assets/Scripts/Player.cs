using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance;
    public float moveSpeed = 10f;
    public float moveForce = 5f;
    public float moveDrag = 0.5f;
    public float dashForce = 10f;
    public float dashSpeed = 60f;
    public float dashLength = 0.2f;
    public float bounceUp = 1f;
    public float bounceOff = 300f;
    public int dashCount = 0;

    public bool bouncing = false;
    public bool mouseMove = false;
    public bool up = false;
    public bool dashing = false;
    public bool ult = false;
    public bool charging = false;
    public bool canDash = true;
    public bool pause = false;
    public Rigidbody rb;
    Vector3 middleOfScreen;
    Vector3 faceDir;
    CameraScript cam;

    void Awake() {
        Instance = this;
        cam = Camera.main.GetComponent<CameraScript>();
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        middleOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        rb.velocity = Vector3.zero;
    }

    void Update() {

        if (up || pause || Time.timeScale < 0.2f ) {
            rb.velocity = Vector3.zero;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            rb.AddForce(new Vector3(0, bounceUp, 0));
            up = true;
        }
        Face();
        if (mouseMove) {
            Move2();
        } else {
            Move();
        }
        if (rb.velocity.magnitude < 0.01f) {
            rb.velocity = Vector3.zero;
        }
        Dash();
    }

    public void Pause() {
        pause = true;
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(0, 0.5f, -40f);
        CancelInvoke();
        dashing = false;
        up = false;
        charging = false;
    }

    void Face() {
        if (dashing) {
            return;
        }
        Vector3 camVec = Input.mousePosition - middleOfScreen;
        faceDir = new Vector3(camVec.x, 0f, camVec.y);
        transform.LookAt(faceDir);
    }

    void Move() {
        if (charging || dashing) {
            return;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(x, 0, z).normalized;

        float y = rb.velocity.y;
        Vector3 velocity = moveDrag * rb.velocity + (1 - moveDrag) * (new Vector3(x, 0, z).normalized * moveSpeed);
        velocity.y = y;
        rb.velocity = velocity;

    }

    void Move2() {
        if (charging || dashing) {
            return;
        }
        float y = rb.velocity.y;
        Vector3 velocity = moveDrag * rb.velocity + (1 - moveDrag) * (faceDir.normalized * moveSpeed);
        velocity.y = y;
        rb.velocity = velocity;
    }

    void Dash() {
        if (!charging && !dashing && !up && canDash) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                rb.velocity = Vector3.zero;
                charging = true;
                cam.ZoomIn();
            }
        }

        if (charging) {

            if (Input.GetKeyUp(KeyCode.Space)) {
                charging = false;
                dashing = true;
                rb.velocity = faceDir.normalized * dashSpeed;
                Invoke("DashStop", dashLength);
                cam.ZoomOut();
            } else {
                rb.velocity = moveDrag * rb.velocity + (1 - moveDrag) * Vector3.zero;
            }
        }
    }

    void DashStop () {
        if (dashing) {
            dashing = false;
            dashCount++;
        }
    }

    void OnCollisionEnter(Collision collision) {
        string colliderTag = collision.collider.tag;
        if (colliderTag == "Block" && dashing && !bouncing) {
            up = true;
            Bounce(new Vector3(0, bounceUp, 0));
        }

        if (colliderTag == "Shield") {
            Vector3 dir = (transform.position - collision.collider.transform.position).normalized * bounceOff;
            if (dashing) {
                dir *= 0.4f;
                dir.y = bounceUp;
            } else {
                dir.y = 0f;
            }
            Bounce(dir);
            StageManager.Instance.EscaperHit();
        }

        //if (colliderTag == "IdleChaser" && dashing) {
        //    StageManager.Instance.EndGame();
        //}
    }

    void OnCollisionStay(Collision collision) {
        string colliderTag = collision.collider.tag;
        if (colliderTag == "Block" && dashing && !bouncing) {
            up = true;
            Bounce(new Vector3(0, bounceUp, 0));
        }

        if (colliderTag == "Shield" && !bouncing) {
            Vector3 dir = (transform.position - collision.collider.transform.position).normalized * bounceOff;
            if (dashing) {
                dir *= 0.4f;
                dir.y = bounceUp;
            } else {
                dir.y = 0f;
            }
            Bounce(dir);
            StageManager.Instance.EscaperHit();
        }

        //if (colliderTag == "IdleChaser" && dashing) {
        //    StageManager.Instance.EndGame();
        //}
    }

    public void Bounce(Vector3 force) {
        bouncing = true;
        rb.AddForce(force);
        Invoke("StopBouncing", 0.2f);
    }

    void StopBouncing() {
        bouncing = false;
    }
}
