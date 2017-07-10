using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Jogador { X, O, N };

public enum TipoNivel { Min, Max};

public class ItemIA {
	public int x;
	public int y;
	public int z;
	public string id;
	public Jogador Jogada = Jogador.N;
}
	
public class IaManager {

	private static IaManager instance = null;

	private bool isComputerTime;

	protected List<string> jogadasDisponiveis = new List<string>();

	private string idJogadaPlayer;

	//private VoxelCube vc;
	private Dictionary<string, ItemIA> idCubos;

	private ItemIA[,,] cubos;

	private bool fimDeJogo = false;

	private bool fimAvaliacao = false;

	private IaManager() 
	{
		isComputerTime = false;
		idCubos = new Dictionary<string, ItemIA> ();
		cubos = new ItemIA[4,4,4];
	}

	public IaManager Clone()
	{
		IaManager ia = new IaManager();
		foreach (KeyValuePair<string, ItemIA> entry in idCubos) {
			ia.cubos [entry.Value.x, entry.Value.y, entry.Value.z] = entry.Value;
			ia.idCubos.Add (entry.Key, entry.Value);
		}
		foreach (string idj in jogadasDisponiveis) {
			ia.jogadasDisponiveis.Add (idj);
		}
		return ia;
	}

	public bool isGameOver()
	{
		return fimDeJogo;
	}

	public bool isEndEval()
	{
		return fimAvaliacao;
	}

	public void setInitialState(VoxelCube v)
	{
		foreach (KeyValuePair<string, ItemIA> entry in v.idCubos) {
			cubos [entry.Value.x, entry.Value.y, entry.Value.z] = entry.Value;
			idCubos.Add (entry.Key, entry.Value);
			jogadasDisponiveis.Add (entry.Key);
		}
	}

	public static IaManager getInstance()
	{
		if (instance == null)
			instance = new IaManager ();
		return instance;
	}

	public string getMoveIA()
	{
		//Joga otimamente
		//Implementação minimax

		//Joga aleatoriamente
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
		idCubos [idJogadaPlayer].Jogada = Jogador.X;
		ItemIA it = idCubos [idJogadaPlayer];
		//Debug.Log (it.x + ", " + it.y + ", " + it.z);
		jogadasDisponiveis.Remove (idJogada);
	}

	public void changeTurn(bool time)
	{
		isComputerTime = time;
	}

	public void computerTime(string idJogadaComp)
	{
		if (idCubos.ContainsKey(idJogadaComp)) {
			idCubos [idJogadaComp].Jogada = Jogador.O;
			jogadasDisponiveis.Remove (idJogadaComp);
		}
	}

	public bool isTurnComputer()
	{
		if (isGameOver ())
			return false;
		return isComputerTime;
	}

	private Jogador avaliaEstados1(bool finalizavel)
	{
		int[] col = new int[3];
		//Teste diagonal principal cubo
		Jogador t = Jogador.N;
		bool vencedor = true;
		for (int i = 0; i < 4; i++) {
			//Teste diagonal principal
			if (i > 0 && t != cubos[i, i, i].Jogada || cubos[col [0], col [1], col [2]].Jogada == Jogador.N) {
				vencedor = false;
				break;
			}
			t = cubos[i, i, i].Jogada;
		}
		if (vencedor) {
			fimDeJogo = finalizavel;
			fimAvaliacao = !finalizavel;
			if (fimDeJogo)
				Debug.Log ("Vencedor1 " + t.ToString ());
			return t;
		} else {
			t = Jogador.N;
		}
		return t;
	}

	private Jogador avaliaEstados2(bool finalizavel)
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
					if (col [(idx + 1) % 3] > 0 && t != cubos[col [0], col [1], col [2]].Jogada || cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
						vencedor = false;
						break;
					}
					t = cubos [col [0], col [1], col [2]].Jogada;
				}
				if (vencedor) {
					fimDeJogo = finalizavel;
					fimAvaliacao = !finalizavel;
					if (fimDeJogo)
						Debug.Log ("Vencedor2 " + t.ToString());
					return t;
				}
			}
		}
		return Jogador.N;
	}

	private Jogador avaliaEstados3(bool finalizavel)
	{
		int[] col = new int[3];
		Jogador t = Jogador.N;
		for (int idx = 0; idx < 3; idx++) {
			for (col[idx % 3] = 0; col[idx % 3] < 4; col[idx % 3]++) {
				//Teste diagonais planos eixos
				bool vencedor = true;
				t = Jogador.N;
				for (col [(idx + 1) % 3] = 0; col [(idx + 1) % 3] < 4; col [(idx + 1) % 3]++) {
					col [(idx + 2) % 3] = 3 - col [(idx + 1) % 3];
					if (col [(idx + 1) % 3] > 0 && t != cubos [col [0], col [1], col [2]].Jogada || cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
						vencedor = false;
						break;
					}
					t = cubos [col [0], col [1], col [2]].Jogada;

				}
				if (vencedor) {
					fimDeJogo = finalizavel;
					fimAvaliacao = !finalizavel;
					if (fimDeJogo)
						Debug.Log ("Vencedor3 " + t.ToString ());
					return t;
				}
			}
		}
		return Jogador.N;
	}


	private Jogador avaliaEstados4(bool finalizavel)
	{
		int[] col = new int[3];
		Jogador t = Jogador.N;
		for (int idx = 0; idx < 3; idx++) {
			//Teste diagonais secundarias
			bool vencedor = true;
			t = Jogador.N;
			for (col [idx % 3] = 0; col [idx % 3] < 4; col [idx % 3]++) {
				col [(idx + 1) % 3] = 3 - col [idx % 3];
				col [(idx + 2) % 3] = col [(idx + 1) % 3];
				if (col [idx % 3] > 0 && t != cubos [col [0], col [1], col [2]].Jogada || cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
					vencedor = false;
					break;
				}
				t = cubos [col [0], col [1], col [2]].Jogada;
				//Debug.Log(String.Format ("{0}, {1}, {2} - {3}", col [0], col [1], col [2], t.ToString()));
			}
			if (vencedor) {
				fimDeJogo = finalizavel;
				fimAvaliacao = !finalizavel;
				if (fimDeJogo)
					Debug.Log ("Vencedor4 " + t.ToString());
				return t;
			}
		}
		return Jogador.N;
	}

	private Jogador avaliaEstados5(bool finalizavel)
	{
		int[] col = new int[3];
		Jogador t = Jogador.N;
		for (int idx = 0; idx < 3; idx++) {
			for (col[idx % 3] = 0; col[idx % 3] < 4; col[idx % 3]++) {
				for (col [(idx + 1) % 3] = 0; col [(idx + 1) % 3] < 4; col [(idx + 1) % 3]++) {
					//Teste planos eixos
					t = Jogador.N;
					bool vencedor = true;
					for (col [(idx + 2) % 3] = 0; col [(idx + 2) % 3] < 4; col [(idx + 2) % 3]++) {
						if (col [(idx + 2) % 3] > 0 && t != cubos [col [0], col [1], col [2]].Jogada || cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
							vencedor = false;
							break;
						}
						t = cubos [col [0], col [1], col [2]].Jogada;
						//Debug.Log(String.Format("{0}, {1}, {2} - {3}", col [0], col [1], col [2], t.ToString()));
					}
					if (vencedor) {
						fimDeJogo = finalizavel;
						fimAvaliacao = !finalizavel;
						if (fimDeJogo)
							Debug.Log ("Vencedor5 " + t.ToString());
						return t;
					}
				}
			}
		}
		return Jogador.N;
	}

	public Jogador avaliarJogada ()
	{
		return avaliarJogada (true);
	}
	public Jogador avaliarJogada (bool finalizavel)
	{
		Jogador t; 
		t = avaliaEstados1 (finalizavel);
		if (t != Jogador.N) {
			return t;
		}
		t = avaliaEstados2 (finalizavel);
		if (t != Jogador.N) {
			return t;
		}
		t = avaliaEstados3 (finalizavel);
		if (t != Jogador.N)
			return t;
		t = avaliaEstados4 (finalizavel);
		if (t != Jogador.N) {
			return t;
		}
		t = avaliaEstados5 (finalizavel);
		if (t != Jogador.N) {
			return t;
		}
		if (jogadasDisponiveis.Count == 0 && finalizavel)
			fimDeJogo = true;
		if (jogadasDisponiveis.Count == 0 && !finalizavel)
			fimAvaliacao = true;
		return Jogador.N;
	}
}
