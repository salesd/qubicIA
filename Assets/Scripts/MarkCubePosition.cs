using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkCubePosition : MonoBehaviour {

	public Renderer rend;

	void OnMouseDown() {
		if (Input.GetMouseButton (0)) {
			int c = System.Convert.ToInt32 (this.name.Split ('_') [1]);
			if (IaManager.getInstance ().jogadaPermitida (this.name)) {
				rend.material.color = Color.blue;
				IaManager.getInstance ().changeTurn (true);
				IaManager.getInstance ().playerTime (this.name);
			}
			//Debug.Log (this.name);
		}
	}
}
