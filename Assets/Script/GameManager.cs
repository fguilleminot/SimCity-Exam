using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(PopulationManager))]
public class GameManager : MonoBehaviour
{
	public bool inGame = true;
	
	public int startMoney = 1000;
	public int currentMoney = 0;
	public int impositionTaxe = 2;
	public int storeTaxe = 20;
	public int factoTaxe = 10;
	public int pay = 1;
	public double speedTime = 24.0;
	public float refreshTime = 0.5f;
	public TimeManager playingTime;
	//private PopulationManager population;
	private DateTime previous;
	
	public Texture2D tmp;
	
	public enum MONEYCOST
	{
		HOUSE = 10,
		STORE = 25,
		MANUFACTORY = 15,
		WATER = 5,
		POWERSTATION = 5,
		DESTROY = 0
	}
	
	public enum WORKER
	{
		NONE = 0,
		STORE = 30,
		MANUFACTORY = 90
	}
	
	private int peopleAccessStore = 60;
	private int peopleManufactory = 120;
	private int ratioH;
	private int ratioS;
	private int ratioM;
	private int people;
	private int numberHouse;
	private int numberStore;
	private int numberManufactory;
	
	void impot ()
	{
		SetCurrentMoney (impositionTaxe * this.GetComponent<PopulationManager> ().getPeople ());
	}
	
	void payDayPeople ()
	{
		SetCurrentMoney (- pay * this.GetComponent<PopulationManager> ().getPeople ());
	}
	
	void taxeDay ()
	{
		int workerNeeded = (this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.STORE].Obj.Count * 
				(int)GameManager.WORKER.STORE) + 
				(this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.MANUFACTORY].Obj.Count * 
				(int)GameManager.WORKER.MANUFACTORY);
		
		if (people > 0)
			SetCurrentMoney ((storeTaxe * this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.STORE].Obj.Count + 
				factoTaxe * this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.MANUFACTORY].Obj.Count) * people/workerNeeded);
	}
	
	public int GetCurrentMoney ()
	{
		return this.currentMoney;
	}
	
	void SetCurrentMoney (int variation)
	{
		this.currentMoney += variation;
	}
	
	// Use this for initialization
	void Start ()
	{
		currentMoney = startMoney;
		playingTime = new TimeManager (speedTime, refreshTime);
		//population = new PopulationManager();
	}
	
	// Update is called once per frame
	void Update ()
	{
		people = this.GetComponent<PopulationManager> ().getPeople ();
		numberHouse = this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.HOUSE].Obj.Count;
		numberStore = this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.STORE].Obj.Count;
		numberManufactory = this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.MANUFACTORY].Obj.Count;
		ratioH = people / Mathf.Max((numberStore * (int)WORKER.STORE + numberManufactory * (int)WORKER.MANUFACTORY), 1);
		ratioM = people / peopleManufactory;
		ratioS = people / peopleAccessStore;
		
		previous = playingTime.getTime ();
		//Debug.Log(playingTime.getTime().ToString());
		playingTime.Update ();
		if (playingTime.changeMonth (previous, playingTime.getTime ())) {
			payDayPeople ();
			taxeDay ();
			Debug.Log ("PAY : " + currentMoney);
		}
		if (playingTime.changeYear (previous, playingTime.getTime ())) {
			impot ();
			Debug.Log ("IMPOT : " + currentMoney);
		}
		
		
	}
	
	void OnGUI()
	{
			GUI.Label(new Rect(5, 50, 150, 25),  "Money : " + currentMoney);
			GUI.Label(new Rect(5, 75, 150, 25),  "Population : " + people);
			GUI.Label(new Rect(5, 100, 80, 50), "Tendance (H/S/M) : " + ratioH + "/" + ratioS + "/" + ratioM);
		GUI.
			GUI.DrawTexture(new Rect(0, 0, 100, 100), tmp);
	}
}
