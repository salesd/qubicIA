using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Jogador { X, O, N };

public class ItemIA : ScriptableObject {
	public int x;
	public int y;
	public int z;
	public string id;
	public Jogador Jogada = Jogador.N;
}

public class IaManager : ScriptableObject {

	private static IaManager instance = null;

	private bool isComputerTime;

	protected List<string> jogadasDisponiveis = new List<string>();

	protected Dictionary<string, ItemIA> estado;

	private string idJogadaPlayer;

	private VoxelCube vc;

	private bool fimDeJogo = false;

	private IaManager() 
	{
		isComputerTime = false;
	}

	public bool isGameOver()
	{
		return fimDeJogo;
	}

	public void setInitialState(VoxelCube v)
	{
		vc = v;
		foreach (KeyValuePair<string, ItemIA> entry in vc.idCubos) {
			jogadasDisponiveis.Add (entry.Key);
		}
	}

	public static IaManager getInstance()
	{
		if (instance == null)
			instance = ScriptableObject.CreateInstance<IaManager> ();
		return instance;
	}

	string getRandom()
	{
		System.Random rnd = new System.Random();
		int idx = rnd.Next (0, jogadasDisponiveis.Count-1);
		return jogadasDisponiveis[idx];
	}

	public bool jogadaPermitida(string idJogada)
	{
		if (isGameOver ())
			return false;
		return jogadasDisponiveis.Contains (idJogada);
	}

	public void playerTime(string idJogada)
	{
		idJogadaPlayer = idJogada;
		vc.idCubos [idJogadaPlayer].Jogada = Jogador.X;
		ItemIA it = vc.idCubos [idJogadaPlayer];
		//Debug.Log (it.x + ", " + it.y + ", " + it.z);
		jogadasDisponiveis.Remove (idJogada);
	}

	public void changeTurn(bool time)
	{
		isComputerTime = time;
	}

	public string computerTime()
	{
		string idJogadaComp = getRandom ();
		vc.idCubos [idJogadaComp].Jogada = Jogador.O;
		jogadasDisponiveis.Remove (idJogadaComp);
		ItemIA it = vc.idCubos [idJogadaComp];
		Renderer rend = vc.cubos[it.x, it.y, it.z].GetComponent<Renderer> ();
		rend.material.color = Color.red;
		return idJogadaComp;
	}

	public bool isTurnComputer()
	{
		if (isGameOver ())
			return false;
		return isComputerTime;
	}

	public void evalState ()
	{
	}

	private Jogador avaliaEstados1()
	{
		int[] col = new int[3];
		//Teste diagonal principal cubo
		Jogador t = Jogador.N;
		bool vencedor = true;
		for (int i = 0; i < 4; i++) {
			//Teste diagonal principal
			if (i > 0 && t != vc.idCubos[vc.getIdCube(i, i, i)].Jogada || vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada == Jogador.N) {
				vencedor = false;
				break;
			}
			t = vc.idCubos[vc.getIdCube(i, i, i)].Jogada;
		}
		if (vencedor) {
			fimDeJogo = true;
			Debug.Log ("Vencedor1 " + t.ToString ());
		} else {
			t = Jogador.N;
		}
		return t;
	}

	private Jogador avaliaEstados2()
	{
		int[] col = new int[3];
		Jogador t = Jogador.N;
		bool vencedor = true;
		for (int idx = 0; idx < 3; idx++) {
			for (col[idx % 3] = 0; col[idx % 3] < 4; col[idx % 3]++) {
				//Teste diagonais
				t = Jogador.N;
				vencedor = true;
				for (col [(idx + 1) % 3] = 0; col [(idx + 1) % 3] < 4; col [(idx + 1) % 3]++) {
					col [(idx + 2) % 3] = col [(idx + 1) % 3];
					//Debug.Log(String.Format ("{0}, {1}, {2}", col [0], col [1], col [2]));
					if (col [(idx + 1) % 3] > 0 && t != vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada || vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada == Jogador.N) {
						vencedor = false;
						break;
					}
					t = vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada;
				}
				if (vencedor) {
					fimDeJogo = true;
					Debug.Log ("Vencedor2 " + t.ToString());

				}
			}
		}
		return Jogador.N;
	}

	private Jogador avaliaEstados3()
	{
		int[] col = new int[3];
		Jogador t = Jogador.N;
		for (int idx = 0; idx < 3; idx++) {
			for (col[idx % 3] = 0; col[idx % 3] < 4; col[idx % 3]++) {
				//Teste diagonais
				bool vencedor = true;
				t = Jogador.N;
				for (col [(idx + 1) % 3] = 0; col [(idx + 1) % 3] < 4; col [(idx + 1) % 3]++) {
					col [(idx + 2) % 3] = 3 - col [(idx + 1) % 3];
					if (col [(idx + 1) % 3] > 0 && t != vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada || vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada == Jogador.N) {
						vencedor = false;
						break;
					}
					t = vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada;

				}
				if (vencedor) {
					fimDeJogo = true;
					Debug.Log ("Vencedor3 " + t.ToString ());

				}
			}
		}
		return Jogador.N;
	}


	private Jogador avaliaEstados4()
	{
		int[] col = new int[3];
		Jogador t = Jogador.N;
		for (int idx = 0; idx < 3; idx++) {
			for (col[idx % 3] = 0; col[idx % 3] < 4; col[idx % 3]++) {
				for (col [(idx + 1) % 3] = 0; col [(idx + 1) % 3] < 4; col [(idx + 1) % 3]++) {
					//Teste posições dimensões
					t = Jogador.N;
					bool vencedor = true;
					for (col [(idx + 2) % 3] = 0; col [(idx + 2) % 3] < 4; col [(idx + 2) % 3]++) {
						if (col [(idx + 2) % 3] > 0 && t != vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada || vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada == Jogador.N) {
							vencedor = false;
							break;
						}
						t = vc.idCubos [vc.getIdCube (col [0], col [1], col [2])].Jogada;
						//Debug.Log(String.Format("{0}, {1}, {2} - {3}", col [0], col [1], col [2], t.ToString()));
					}
					if (vencedor) {
						fimDeJogo = true;
						Debug.Log ("Vencedor4 " + t.ToString());

					}
				}
			}
		}
		return Jogador.N;
	}


	public Jogador avaliarJogada ()
	{
		Jogador t; 
		t = avaliaEstados1 ();
		if (t != Jogador.N) {
			return t;
		}
		t = avaliaEstados2 ();
		if (t != Jogador.N) {
			return t;
		}
		t = avaliaEstados3 ();
		if (t != Jogador.N)
			return t;
		t = avaliaEstados4 ();
		if (t != Jogador.N) {
			return t;
		}
		return Jogador.N;
	}
}
