using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public GameObject darkFilter;
	Player player;
	Escaper escaper;
	Vector3 offset;
	public float smoothTime = 0.2f;
	Vector3 velocity = Vector3.zero;
	Vector3 targetPos;
	Vector3 followPos;
	float followSharpness = 0.5f;
	public float zoomDistance = 5f;
	bool finished = false;
	float lerpTime = 0f;

	void Start () {
		player = Player.Instance;
		offset = transform.position - player.transform.position;
		//transform.position = player.transform.position + offset;	
	}

	public void ZoomIn () {
		offset += zoomDistance * new Vector3(0, -1f, 1f);
	}

	public void ZoomOut () {
		offset -= zoomDistance * new Vector3(0, -1f, 1f);
	}

	void FixedUpdate () {
		if (!finished) {
			followPos = player.transform.position + offset;
			transform.position = Vector3.SmoothDamp(transform.position, followPos, ref velocity, smoothTime);
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Zone") {
			StageManager.Instance.ResumeFromDarkZone();
		}
	}

	//public IEnumerator LerpToEscaper() {
	//escaper = Escaper.Instance;
	//finished = true;
	//Quaternion start = transform.rotation;
	//Quaternion end = start;
	//end.x = 0;

	//float delta = 0f;
	//while (delta < 20f) {
	//    delta += Time.deltaTime;
	//    transform.rotation = Quaternion.Lerp(start, end, delta / 20f);
	//    yield return new WaitForEndOfFrame();
	//}
	//}
}
