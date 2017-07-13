using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarkCubePosition : MonoBehaviour {

	public Renderer rend;

	void OnMouseDown() {
		if (Input.GetMouseButton (0)) {

			if (IaManager.getInstance ().isGameOver ()) {
				IaManager.Destroy ();
				SceneManager.LoadScene("Menu", LoadSceneMode.Single);
			}
				
			if (IaManager.getInstance ().jogadaPermitida (this.name)) {
				rend.material.color = Color.blue;
				IaManager.getInstance ().changeTurn (true);
				IaManager.getInstance ().playerTime (this.name);
				IaManager.getInstance ().avaliarJogada ();
			}
			//Debug.Log (this.name);

		}
	}
}
