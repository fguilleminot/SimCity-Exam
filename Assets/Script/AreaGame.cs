using UnityEngine;
using System.Collections;

public class AreaGame : MonoBehaviour {
	
	public int numberOfSection = 10; //number of section per lane, define a grid (numberOfSection*sizeOfSection, numberOfSection*sizeOfSection)
	
	public enum AREAUSING{
		EMPTY,
		BUSY
	}
	
	public enum CONSTRUCTION{
		ROAD,
		WATER,
		ELECTRICITY,
		HOUSE,
		SHOP,
		INDUSTRY
	}
	
	// Use this for initialization
	void Start () {
		if (this)
		{
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
