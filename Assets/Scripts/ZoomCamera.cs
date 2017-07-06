using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			GetComponent<Transform> ().position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + .3f*10);
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			GetComponent<Transform> ().position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - .3f*10);
		}
	}
}
