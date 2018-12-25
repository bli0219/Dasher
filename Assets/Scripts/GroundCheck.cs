using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.gameObject != gameObject) {
            Player.Instance.up = false;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject != gameObject) {
            Player.Instance.up = false;
        }
    }
}
