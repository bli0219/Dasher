using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public static PlayerControl instance;
    public float moveSpeed = 10f;
    public float moveForce = 5f;
    public float moveDrag = 0.5f;
    public float dashForce = 10f;
    public float dashSpeed = 60f;
    public float dashLength = 0.2f;
    public float bounceUp = 1f;
    private Rigidbody rb;
    Vector3 middleOfScreen;

    public bool mouseMove = false;
    bool up = false;
    public bool dashing = false;
    public bool ult = false;
    bool charging = false;
    Vector3 faceDir;
    CameraFollow cam;

    void Awake() {
        instance = this;
        cam = Camera.main.GetComponent<CameraFollow>();
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        middleOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.K)) {
            rb.AddForce(new Vector3(100, 100, 0));
        }

        Face();
        if (up) {
            return;
        }
        if (mouseMove) {
            Move2();
        } else {
            Move();
        }
        Dash();
    }

    private void Actions() {


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
        //rb.AddForce(new Vector3(x, 0, z).normalized * movementForce);
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
        if (!charging && !dashing && !up) {
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
               // rb.AddForce(faceDir.normalized * dashForce);
                rb.velocity = faceDir.normalized * dashSpeed;
                Invoke("DashStop", dashLength);
                cam.ZoomOut();
            }
        }
    }

    void DashStop () {
        if (dashing) {
            //rb.velocity = Vector3.zero;
            dashing = false;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Block" && dashing) {
            Debug.Log("UP");
            up = true;
            rb.AddForce(new Vector3(0, bounceUp, 0));
        }   

        if (collision.collider.tag == "Ground") {
            up = false;
        }
    }
}
