using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour {
	public float rotationSpeed = 20;

	public BoxCollider ccollider;

	void OnMouseDrag()
	{
		float rotX = Input.GetAxis ("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
		float rotY = Input.GetAxis ("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;

		transform.RotateAround (Vector3.up, -rotX);
		transform.RotateAround (Vector3.right, rotY);
	}
}
