using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    Rigidbody2D rb;

    void Start () {
        float cost = 0f;
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i!= 20000; i++) {
            cost += Vector2.SqrMagnitude(new Vector2(1f, -1f)) * 2f;
        }
        Debug.Log("Time for 20k " + (Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;
        for (int i = 0; i != 600000; i++) {
            cost += Vector2.SqrMagnitude(new Vector2(1f, -1f)) * 2f;
        }
        Debug.Log("Time for 600k " + (Time.realtimeSinceStartup - time));
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
