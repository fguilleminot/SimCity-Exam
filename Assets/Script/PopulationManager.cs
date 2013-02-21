using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectPlacer))]
public class PopulationManager : MonoBehaviour
{
	
	public int growthPeople = 1;
	public int popMaxInHouse = 50;
	public float timeToGrowth = 0.5f;
	private float timeBetweenModification = 0;
	public int people = 0;
	public int populationMax = 0;
	public int workerNeeded = 0;
	
	void start ()
	{
		
	}
	
	void Update ()
	{
		//Debug.Log(this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.HOUSE].Obj.Count);
		timeBetweenModification += Time.deltaTime;
		populationMax = this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.HOUSE].Obj.Count * popMaxInHouse;
		workerNeeded = (this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.STORE].Obj.Count * 
				(int)GameManager.WORKER.STORE) + 
				(this.GetComponent<ObjectPlacer> ().Created [(int)ObjectPlacer.TYPEOFBUILDING.MANUFACTORY].Obj.Count * 
				(int)GameManager.WORKER.MANUFACTORY);
		
		if (timeBetweenModification >= timeToGrowth) {
			timeBetweenModification = 0f;
			if ((people < populationMax) && (people < workerNeeded))
				people += growthPeople;
			else if (people > populationMax)
				people = populationMax;
			else if (people > workerNeeded)
				people -= growthPeople;
			//Debug.Log ("people : " + people + "populationMax : " + populationMax);
		}
	}
	
	public int getPeople ()
	{
		return this.people;
	}
	
	public override string ToString ()
	{
		return this.people.ToString ();
	}
}
