using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashback : MonoBehaviour {

	public static Flashback Instance;

	public GameObject[] fadeObjects;
	public GameObject[] shadowObjects;

	public float flashbackLength;
	void Awake () {
		Instance = this;
	}
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)) {
			FlashbackFor(flashbackLength);
		}
	}

	void Start () {
		foreach (GameObject go in fadeObjects) {
			go.SetActive(true);
		}
		foreach (GameObject go in shadowObjects) {
			go.SetActive(false);
		}
	}

	public void FlashbackFor(float sec) {
		StartCoroutine(FlashbackCR(sec));
	}

	IEnumerator FlashbackCR(float sec) {
		foreach(GameObject go in shadowObjects) {
			go.SetActive(true);
		}
		foreach (GameObject go in fadeObjects) {
			go.SetActive(false);
		}
		yield return new WaitForSecondsRealtime(sec);
		foreach (GameObject go in shadowObjects) {
			go.SetActive(false);
		}
		foreach (GameObject go in fadeObjects) {
			go.SetActive(true);
		}
	}
}
