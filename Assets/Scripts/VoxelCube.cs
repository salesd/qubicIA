using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelCube : MonoBehaviour {

	private static GameObject cubePrefab;
	private static GameObject cubeContainer;
	private static int cubeCount = 0;
	private static List<GameObject> cubes;

	public BoxCollider collider;

	// Use this for initialization
	void Start () {
		for (float i = -3; i < 5; i+=2) {
			for (float j = -3; j < 5; j+=2) {
				for (float k = -3; k < 5; k+=2) {
					MakeCube (new Vector3 (i, j, k), Color.white, 1);
				}
			}
		}
	}

	void Update() {
		if (Input.GetButtonDown("Movimentar")) {
			collider.size = new Vector3 (8, 8, 8);
		}
		if (Input.GetButtonUp ("Movimentar")) {
			collider.size = new Vector3 (0, 0, 0);
		}
	}

	private GameObject GetCubePrefab()
	{
		if (cubePrefab == null)
			cubePrefab = Resources.Load("Cube") as GameObject;
		return cubePrefab;
	}

	public GameObject MakeCube(Vector3 position, Color color, float size)
	{
		cubeCount++;
		if (cubeContainer == null)
		{
			cubeContainer = new GameObject("cube container");
			cubeContainer.layer = 8;
			collider = cubeContainer.AddComponent<BoxCollider> ();
			MeshRenderer renderer = cubeContainer.AddComponent<MeshRenderer>();
			collider.center = renderer.bounds.center;
			collider.size = new Vector3 (0, 0, 0);
			RotateObj ro = cubeContainer.AddComponent<RotateObj> ();
			ro.collider = collider;
			cubes = new List<GameObject>();
		}

		GameObject cube = Instantiate(GetCubePrefab()) as GameObject;
		cubes.Add(cube);
		cube.transform.position = position;
		cube.transform.parent = cubeContainer.transform;
		cube.name = "cube_" + cubeCount;

		cube.GetComponent<Renderer>().material.color = color;
		cube.transform.localScale = new Vector3(size, size, size);


		return cube;
	}
}
