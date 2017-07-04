using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkCubePosition : MonoBehaviour {

	public Renderer rend;

	public Material colorPlayer;

	public Material colorComputer;

	void OnMouseDown() {
		if (Input.GetMouseButton (0)) {
			rend.material = colorPlayer;
			Debug.Log (this.name);
		}
	}
}
