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

	protected List<string> jogadasDisponiveis = new List<string> ();

	protected List<string> avaliados = new List<string> ();

	private string idJogadaPlayer;

	private Dictionary<string, ItemIA> idCubos;

	private ItemIA[,,] cubos;

	private bool fimDeJogo = false;

	private bool fimAvaliacao = false;

	private int valorHeuristica = 0;

	private IaManager() 
	{
		isComputerTime = false;
		idCubos = new Dictionary<string, ItemIA> ();
		cubos = new ItemIA[4,4,4];
	}

	public static void Destroy()
	{
		instance = null;
	}

	public bool isGameOver()
	{
		return fimDeJogo;
	}

	public bool isEndEval()
	{
		if (fimAvaliacao) {
			fimAvaliacao = false;
			return true;
		}
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

	private void imprimeEstato()
	{
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				for (int k = 0; k < 4; k++) {
					Debug.Log (String.Format("{0},{1},{2}", i, j, k) + cubos [i, j, k].Jogada);
				}
			}
		}
	}

	public string getMoveIA()
	{
		//Joga otimamente
		string idj;
		//imprimeEstato ();
		MiniMax (3, this, TipoNivel.Max, out idj);
		//imprimeEstato ();
		//Debug.Log ("Saiu " + idj + " R " + jogadasDisponiveis.Count);
		return idj;
		//Joga aleatoriamente
		//System.Random rnd = new System.Random();
		//int idx = rnd.Next (0, jogadasDisponiveis.Count-1);
		//return jogadasDisponiveis[idx];
	}

	static int MiniMax(int deep, IaManager ia, TipoNivel tipo , out string idJogada)
	{
		idJogada = "";
		int alpha = tipo == TipoNivel.Max ? -2 : 2;
		for (int i = ia.jogadasDisponiveis.Count - 1; i >= 0; i--) {
			string idItem = ia.jogadasDisponiveis [i];
			if (tipo == TipoNivel.Max)
				ia.computerTime (idItem);
			else
				ia.playerTime (idItem);
			Jogador j = ia.avaliarJogada (false);
			//Debug.Log ("idCubos2 " + j.ToString() + " R " + ia.jogadasDisponiveis.Count);
			if (ia.isEndEval () || deep == 0) {
				ia.idCubos [idItem].Jogada = Jogador.N;
				ia.Insert (idItem, i);
				idJogada = idItem;
				if (j == Jogador.N) {
					if (deep == 0)
						return ia.valorHeuristica;
					else
						return 0;
				} else if (j == Jogador.O) {
					return -2;
				} else if (j == Jogador.X) {
					return 2;
				}
			} else {
				int score = 0;
				string idj;
				if (ia.valorHeuristica == 0) {
					ia.idCubos [idItem].Jogada = Jogador.N;
					ia.Insert (idItem, i);
					continue;
				}
				if (tipo == TipoNivel.Max) {
					score = MiniMax (deep - 1, ia, TipoNivel.Min, out idj);
					if (idj != "") {
						ia.idCubos [idItem].Jogada = Jogador.N;
						ia.Insert (idItem, i);
						if (score >= alpha) {
							alpha = score;
							idJogada = idj;
						}
						if (alpha == 2)
							break;
					}
				} else {
					score = MiniMax (deep - 1, ia, TipoNivel.Max, out idj);
					if (idj != "") {
						ia.idCubos [idItem].Jogada = Jogador.N;
						ia.Insert (idItem, i);
						if (score <= alpha) {
							alpha = score;
							idJogada = idj;
						}
						if (alpha == -2)
							break;
					}
				}
			}

		}
		//Debug.Log ("idJogada: " + idJogada + ", alpha: " + alpha); 
		return alpha;
	}

	public bool jogadaPermitida(string idJogada)
	{
		if (isGameOver ())
			return false;
		return jogadasDisponiveis.Contains (idJogada);
	}

	private void Insert(string idJogada, int index)
	{
		if (jogadasDisponiveis.Count > index) {
			jogadasDisponiveis.Insert (index, idJogada);
		} else {
			jogadasDisponiveis.Add (idJogada);
		}
	}

	private void Remove(string idJogada)
	{
		jogadasDisponiveis.Remove (idJogada);
	}

	public void playerTime(string idJogadaPlayer)
	{
		idCubos [idJogadaPlayer].Jogada = Jogador.X;
		ItemIA it = idCubos [idJogadaPlayer];
		//Debug.Log (iia.x + ", " + iia.y + ", " + iia.z);
		Remove(idJogadaPlayer);
	}

	public void changeTurn(bool time)
	{
		isComputerTime = time;
	}

	public void computerTime(string idJogadaComp)
	{
		if (idCubos.ContainsKey(idJogadaComp)) {
			idCubos [idJogadaComp].Jogada = Jogador.O;
			Remove (idJogadaComp);
		}
	}

	public bool isTurnComputer()
	{
		if (isGameOver ())
			return false;
		return isComputerTime;
	}

	private void avaliarHeuristica(Jogador jogada)
	{
		if (jogada == Jogador.O) {
			if (valorHeuristica <= 0)
				valorHeuristica = -1;
			else
				valorHeuristica = 0;
		} else if (jogada == Jogador.X) {
			if (valorHeuristica >= 0)
				valorHeuristica = 1;
			else
				valorHeuristica = 0;
		}
	}

	private Jogador avaliaEstados1(bool finalizavel)
	{
		//Teste diagonal principal cubo
		Jogador t = Jogador.N;
		bool vencedor = true;
		valorHeuristica = 0;
		for (int i = 0; i < 4; i++) {
			//Teste diagonal principal
			avaliarHeuristica(cubos[i, i, i].Jogada);
			if (cubos[i, i, i].Jogada == Jogador.N) {
				vencedor = false;
				continue;
			}
			if (i > 0 && t != cubos[i, i, i].Jogada) {
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
					avaliarHeuristica(cubos [col [0], col [1], col [2]].Jogada);
					if (cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
						vencedor = false;
						continue;
					}

					if (col [(idx + 1) % 3] > 0 && t != cubos[col [0], col [1], col [2]].Jogada) {
						valorHeuristica = 0;
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
				valorHeuristica = 0;
				for (col [(idx + 1) % 3] = 0; col [(idx + 1) % 3] < 4; col [(idx + 1) % 3]++) {
					col [(idx + 2) % 3] = 3 - col [(idx + 1) % 3];
					avaliarHeuristica(cubos [col [0], col [1], col [2]].Jogada);
					if (cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
						vencedor = false;
						continue;
					}
					if (col [(idx + 1) % 3] > 0 && t != cubos [col [0], col [1], col [2]].Jogada) {
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
			valorHeuristica = 0;
			for (col [idx % 3] = 0; col [idx % 3] < 4; col [idx % 3]++) {
				col [(idx + 1) % 3] = 3 - col [idx % 3];
				col [(idx + 2) % 3] = col [(idx + 1) % 3];
				avaliarHeuristica(cubos [col [0], col [1], col [2]].Jogada);
				if (cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
					vencedor = false;
					continue;
				}
				if (col [idx % 3] > 0 && t != cubos [col [0], col [1], col [2]].Jogada) {
					vencedor = false;
					break;
				}
				t = cubos [col [0], col [1], col [2]].Jogada;
				//Debug.Log(String.Format ("{0}, {1}, {2} - {3}", col [0], col [1], col [2], ia.ToString()));
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
					valorHeuristica = 0;
					for (col [(idx + 2) % 3] = 0; col [(idx + 2) % 3] < 4; col [(idx + 2) % 3]++) {
						avaliarHeuristica(cubos [col [0], col [1], col [2]].Jogada);
						if (cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
							vencedor = false;
							continue;
						}
						if ((col [(idx + 2) % 3] > 0 && t != cubos [col [0], col [1], col [2]].Jogada) || cubos [col [0], col [1], col [2]].Jogada == Jogador.N) {
							vencedor = false;
							break;
						}
						t = cubos [col [0], col [1], col [2]].Jogada;
						//Debug.Log(String.Format("{0}, {1}, {2} - {3}", col [0], col [1], col [2], ia.ToString()));
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
		if (t != Jogador.N) {
			return t;
		}
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
