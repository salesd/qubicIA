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
	public Enum Jogador;
}

public class IaManager : ScriptableObject {

	private static IaManager instance = null;

	private bool isComputerTime;

	protected List<string> jogadasDisponiveis = new List<string>();

	protected Dictionary<string, ItemIA> estado;

	private string idJogadaPlayer;

	private IaManager() 
	{
		isComputerTime = false;
	}

	public void setInitialState(Dictionary<string, ItemIA> est)
	{
		estado = est;
		foreach (KeyValuePair<string, ItemIA> entry in estado) {
			jogadasDisponiveis.Add (entry.Key);
		}
	}

	public static IaManager getInstance()
	{
		if (instance == null)
			instance = new IaManager ();
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
		return jogadasDisponiveis.Contains (idJogada);
	}

	public void playerTime(string idJogada)
	{
		idJogadaPlayer = idJogada;
		jogadasDisponiveis.Remove (idJogada);
	}

	public void changeTurn(bool time)
	{
		isComputerTime = time;
	}

	public string computerTime()
	{
		string idJogadaComp = getRandom ();
		jogadasDisponiveis.Remove (idJogadaComp);
		return idJogadaComp;
	}

	public bool isTurnComputer()
	{
		return isComputerTime;
	}

	public void evalState ()
	{
		
	}
}
