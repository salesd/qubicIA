using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelCube : MonoBehaviour {

	private static GameObject cubePrefab;
	private static GameObject cubeContainer;
	private static int cubeCount = 0;
	private static List<GameObject> cubes;

	public BoxCollider ccollider;

	public GameObject[, , ] cubos = new GameObject[4,4,4];
	public Dictionary<string, ItemIA> idCubos = new Dictionary<string, ItemIA>();

	// Use this for initialization
	void Start () {
		int ii = 0;
		for (float i = -3; i < 5; i+=2, ii++) {
			int jj = 0;
			for (float j = -3; j < 5; j+=2, jj++) {
				int kk = 0;
				for (float k = -3; k < 5; k+=2, kk++) {
					cubos[ii, jj, kk] = MakeCube (new Vector3 (i, j, k), Color.white, 1);
					ItemIA it = new ItemIA ();
					it.x = ii;
					it.y = jj;
					it.z = kk;
					it.Jogada = Jogador.N;
					idCubos [cubos [ii, jj, kk].name] = it;
				}
			}
		}
		IaManager.getInstance().setInitialState (this);
	}

	public string getIdCube(int x, int y, int z)
	{
		return cubos[x, y, z].name;
	}

	void Update() {
		if (Input.GetButtonDown("Movimentar")) {
			ccollider.size = new Vector3 (8, 8, 8);
		}
		if (Input.GetButtonUp ("Movimentar")) {
			ccollider.size = new Vector3 (0, 0, 0);
		}
		if (Input.GetMouseButton (0)) {
			if (IaManager.getInstance ().isTurnComputer ()) {
				string idJog = IaManager.getInstance ().getMoveIA ();
				IaManager.getInstance ().computerTime (idJog);
				IaManager.getInstance ().changeTurn (false);
				IaManager.getInstance ().avaliarJogada ();
				ItemIA it = idCubos [idJog];
				Renderer rend = cubos[it.x, it.y, it.z].GetComponent<Renderer> ();
				rend.material.color = Color.red;
			}
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
			ccollider = cubeContainer.AddComponent<BoxCollider> ();
			MeshRenderer renderer = cubeContainer.AddComponent<MeshRenderer>();
			ccollider.center = renderer.bounds.center;
			ccollider.size = new Vector3 (0, 0, 0);
			RotateObj ro = cubeContainer.AddComponent<RotateObj> ();
			ro.ccollider = ccollider;
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
