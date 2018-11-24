using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    PlayerControl player;
    private Vector3 offset;
    float smoothTime = 0.2f;
    Vector3 velocity = Vector3.zero;
    Vector3 followPos;
    float followSharpness = 0.5f;
    public float zoomDistance = 5f;
    void Start () {
        player = PlayerControl.instance;
        offset = transform.position - player.transform.position;	
	}
	
    public void ZoomIn() {
        offset += zoomDistance * new Vector3(0, -1f, 1f);
    }

    public void ZoomOut() {
        offset -= zoomDistance * new Vector3(0, -1f, 1f);
    }

    void FixedUpdate () { 
        followPos = player.transform.position + offset;

        //float blend = 1f - Mathf.Pow(1f - followSharpness, Time.deltaTime * 60f);

        //transform.position = Vector3.Lerp(
        //       transform.position,
        //       followPos,
        //       blend);

        //float interpolation = 2 * Time.deltaTime;
        //followPos.x = Mathf.Lerp(transform.position.x, followPos.x, interpolation);
        //followPos.z = Mathf.Lerp(transform.position.z, followPos.z, interpolation);
        //transform.position = followPos;

        transform.position = Vector3.SmoothDamp(transform.position, followPos, ref velocity, smoothTime);
        //transform.position = player.transform.position + offset;
    }

}
