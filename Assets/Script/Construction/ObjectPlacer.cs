using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameManager))]
public class ObjectPlacer : MonoBehaviour
{
	public GameObject[] tmp;
	public GameObject prefabToBuild;
	public TYPEOFBUILDING currentType;
	public GameObject SonOf;
	
	public class ObjectCreated
	{
		public System.Collections.Generic.List<GameObject> Obj;
		public TYPEOFBUILDING type;
		
		public ObjectCreated (TYPEOFBUILDING t = TYPEOFBUILDING.NONE)
		{
			Obj = new System.Collections.Generic.List<GameObject> ();
			type = t;
		}
	}
	
	public string TagMap;
	private GameObject prefabInstantiate;
	private bool mouseDown;
	private Vector3 mousePos;
	private Vector3 otherPos;
	private Rect guiRect;
	private float left = 0f;
	private float top = 0f;
	private float width = 0f;
	private float height = 0f;
	private Vector3 camPos;
	private bool destroyMode;
	public ObjectCreated[] Created;
	
	public enum TYPEOFBUILDING
	{
		HOUSE,
		STORE,
		MANUFACTORY,
		WATER,
		POWERSTATION,
		DESTROY,
		NONE
	}
	
	// Use this for initialization
	void Start ()
	{
		prefabToBuild = null;
		mouseDown = false;
		currentType = TYPEOFBUILDING.NONE;
		camPos = Camera.main.transform.position;
		Created = new ObjectCreated[5];
		for (int i=0; i<5; i++)
			Created [i] = new ObjectCreated ((TYPEOFBUILDING)i);
	}
	
	// Update is called once per frame
	void Update ()
	{
		InputKeyboardManager ();

		//Capture the position when MouseButtonPressed
		if (Input.GetMouseButtonDown (0)) {
			mousePos = Camera.main.ScreenToViewportPoint (Input.mousePosition) * camPos.y;
			mouseDown = true;
			//Debug.Log ("capture position pression " + mousePos);
		}
		if (Input.GetMouseButtonUp (0) && mouseDown) {
			//Possibility of areaCreationPrefab
			otherPos = Camera.main.ScreenToViewportPoint (Input.mousePosition) * camPos.y;
			//Debug.Log ("capture position apres pression " + otherPos);
			if (destroyMode)
				destroyPrefab ();
			else if (prefabToBuild != null)
				drawPrefab ();
				
			mouseDown = false;
		}
	
	}
	
	public void InputKeyboardManager ()
	{
		if (Input.GetKeyDown (KeyCode.D)) {
			destroyMode = true;
			prefabToBuild = null;
			currentType = TYPEOFBUILDING.DESTROY;
		} else if (Input.GetKeyDown (KeyCode.H)) {
			destroyMode = false;
			currentType = TYPEOFBUILDING.HOUSE;
		} else if (Input.GetKeyDown (KeyCode.S)) {
			destroyMode = false;
			currentType = TYPEOFBUILDING.STORE;
		} else if (Input.GetKeyDown (KeyCode.M)) {
			destroyMode = false;
			currentType = TYPEOFBUILDING.MANUFACTORY;
		} else if (Input.GetKeyDown (KeyCode.W)) {
			destroyMode = false;
			currentType = TYPEOFBUILDING.WATER;
		} else if (Input.GetKeyDown (KeyCode.P)) {
			destroyMode = false;
			currentType = TYPEOFBUILDING.POWERSTATION;
		}
		
		if ((currentType != TYPEOFBUILDING.NONE) && (currentType != TYPEOFBUILDING.DESTROY))
			prefabToBuild = tmp [(int)currentType];
		
	}
	
	public void drawPrefab ()
	{
		//Corner of the rectangle
		left = Mathf.Min (otherPos.x, mousePos.x);
		top = Mathf.Min (otherPos.y, mousePos.y);
		width = Mathf.Max (Mathf.Abs (mousePos.x - otherPos.x), 1f);
		height = Mathf.Max (Mathf.Abs (mousePos.y - otherPos.y), 1f);
			
		Ray ray = Camera.main.ViewportPointToRay (mousePos / camPos.y);

		RaycastHit outinfo;
		if (Physics.Raycast (ray, out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag == TagMap) {
				Vector3 prefabPos = prefabPosition (outinfo.point);
				//Debug.DrawRay (Camera.main.transform.position, prefabPos - Camera.main.transform.position, Color.red, 100f);
				
				//Debug.Log ("l " + left + " t " + top + " w " + width + " h " + height);
				for (float indexW = 0f; indexW < width; indexW+=1.05f) {
					for (float indexH = 0f; indexH < height; indexH+=1.05f) {
						prefabToBuild.SetActive (true);
						prefabToBuild.transform.position = new Vector3 (prefabPos.x + indexW - 0.5f, 0.1f, prefabPos.z + indexH - 0.5f);
						if (checkPlaceToCreate (prefabToBuild.transform.position) && canConstruct (currentType)) {
							prefabInstantiate = Instantiate (prefabToBuild) as GameObject;
							if (prefabInstantiate) {
								prefabInstantiate.transform.parent = SonOf.transform;
								Created [(int)currentType].Obj.Add (prefabInstantiate);
							} else
								Debug.Log ("fuck");
						}
					}
				}
			}
		}
	}
	
	public void destroyPrefab ()
	{
		//Corner of the rectangle
		left = Mathf.Min (otherPos.x, mousePos.x);
		top = Mathf.Min (otherPos.y, mousePos.y);
		width = Mathf.Abs (mousePos.x - otherPos.x);
		height = Mathf.Abs (mousePos.y - otherPos.y);
		
		GameObject toDestroy;
		Ray ray = Camera.main.ViewportPointToRay (mousePos / camPos.y);
		RaycastHit outinfo;
		if (Physics.Raycast (ray, out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				removeInList (outinfo.transform.gameObject);
				Destroy (outinfo.transform.gameObject);
			} else {
				Vector3 destructionPos = prefabPosition (outinfo.point);
				//Debug.DrawRay (Camera.main.transform.position, destructionPos - Camera.main.transform.position, Color.red, 100f);
				
				//Debug.Log ("l " + left + " t " + top + " w " + width + " h " + height);
				for (float indexW = 0f; indexW <= width; indexW+=0.25f) {
					for (float indexH = 0f; indexH <= height; indexH+=0.25f) {
						toDestroy = checkDestroy (new Vector3 (destructionPos.x + indexW - 0.5f, 0.1f, destructionPos.z + indexH - 0.5f));
						if (toDestroy != null) {
							removeInList (toDestroy);
							Destroy (toDestroy);
						}
					}
				}
			}
		}
	}
	
	public void removeInList (GameObject toDestroy)
	{
		if (toDestroy.CompareTag ("House"))
			Created [(int)TYPEOFBUILDING.HOUSE].Obj.Remove (toDestroy);
		else if (toDestroy.CompareTag ("Store"))
			Created [(int)TYPEOFBUILDING.STORE].Obj.Remove (toDestroy);
		else if (toDestroy.CompareTag ("Facto"))
			Created [(int)TYPEOFBUILDING.MANUFACTORY].Obj.Remove (toDestroy);
		else if (toDestroy.CompareTag ("Water"))
			Created [(int)TYPEOFBUILDING.WATER].Obj.Remove (toDestroy);
		else
			Created [(int)TYPEOFBUILDING.POWERSTATION].Obj.Remove (toDestroy);
	}
	
	public Vector3 prefabPosition (Vector3 rayCastHitPos)
	{
		Vector3 result;
		
		if (mousePos.x == left && mousePos.y == top)
			result = rayCastHitPos;
		else if (mousePos.x == left)
			result = new Vector3 (rayCastHitPos.x, 0f, rayCastHitPos.z - height);
		else if (mousePos.y == top)
			result = new Vector3 (rayCastHitPos.x - width, 0f, rayCastHitPos.z);
		else
			result = new Vector3 (rayCastHitPos.x - width, 0f, rayCastHitPos.z - height);
		
		return result;
	}
	
	public GameObject checkDestroy (Vector3 nextPrefabInstantiate)
	{
		GameObject result = null;
		//Debug.Log("toto " + nextPrefabInstantiate);
		
		Vector3 posToCheck1 = new Vector3 (nextPrefabInstantiate.x, nextPrefabInstantiate.y, nextPrefabInstantiate.z);
		Vector3 posToCheck2 = new Vector3 (nextPrefabInstantiate.x + 0.95f, nextPrefabInstantiate.y, nextPrefabInstantiate.z);
		Vector3 posToCheck3 = new Vector3 (nextPrefabInstantiate.x + 0.95f, nextPrefabInstantiate.y, nextPrefabInstantiate.z + 0.95f);
		Vector3 posToCheck4 = new Vector3 (nextPrefabInstantiate.x, nextPrefabInstantiate.y, nextPrefabInstantiate.z + 0.95f);
		
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck1 - Camera.main.transform.position), Color.cyan, 100f);
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck2 - Camera.main.transform.position), Color.cyan, 100f);
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck3 - Camera.main.transform.position), Color.cyan, 100f);
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck4 - Camera.main.transform.position), Color.cyan, 100f);
		RaycastHit outinfo;
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck1 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return outinfo.transform.gameObject;
			}
		}
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck2 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return outinfo.transform.gameObject;
			}
		}
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck3 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return outinfo.transform.gameObject;
			}
		}
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck4 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return outinfo.transform.gameObject;
			}
		}
		
		return result;
	}
	
	public bool checkPlaceToCreate (Vector3 nextPrefabInstantiate)
	{
		bool result = true;
		//Debug.Log("toto " + nextPrefabInstantiate);
		
		Vector3 posToCheck1 = new Vector3 (nextPrefabInstantiate.x, nextPrefabInstantiate.y, nextPrefabInstantiate.z);
		Vector3 posToCheck2 = new Vector3 (nextPrefabInstantiate.x + 1.05f, nextPrefabInstantiate.y, nextPrefabInstantiate.z);
		Vector3 posToCheck3 = new Vector3 (nextPrefabInstantiate.x + 1.05f, nextPrefabInstantiate.y, nextPrefabInstantiate.z + 1.05f);
		Vector3 posToCheck4 = new Vector3 (nextPrefabInstantiate.x, nextPrefabInstantiate.y, nextPrefabInstantiate.z + 1.05f);
		
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck1 - Camera.main.transform.position), Color.cyan, 100f);
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck2 - Camera.main.transform.position), Color.cyan, 100f);
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck3 - Camera.main.transform.position), Color.cyan, 100f);
		//Debug.DrawRay (Camera.main.transform.position, (posToCheck4 - Camera.main.transform.position), Color.cyan, 100f);
		RaycastHit outinfo;
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck1 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return false;
			}
		}
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck2 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return false;
			}
		}
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck3 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return false;
			}
		}
		if (Physics.Raycast (Camera.main.transform.position, (posToCheck4 - Camera.main.transform.position), out outinfo, Mathf.Infinity)) {
			if (outinfo.transform.tag != TagMap) {
				return false;
			}
		}
		
		return result;
	}
	
	public bool canConstruct (ObjectPlacer.TYPEOFBUILDING typeOfBuild)
	{
		bool result = false;
		switch (typeOfBuild) {
		case ObjectPlacer.TYPEOFBUILDING.HOUSE :
			{
				if (this.GetComponent<GameManager> ().currentMoney - (int)GameManager.MONEYCOST.HOUSE > 0) {
					this.GetComponent<GameManager> ().currentMoney -= (int)GameManager.MONEYCOST.HOUSE;
					result = true;
				}
				break;
			}
		case ObjectPlacer.TYPEOFBUILDING.STORE :
			{
				if (this.GetComponent<GameManager> ().currentMoney - (int)GameManager.MONEYCOST.STORE > 0) {
					this.GetComponent<GameManager> ().currentMoney -= (int)GameManager.MONEYCOST.STORE;
					result = true;
				}
				break;
			}
		case ObjectPlacer.TYPEOFBUILDING.MANUFACTORY :
			{
				if (this.GetComponent<GameManager> ().currentMoney - (int)GameManager.MONEYCOST.MANUFACTORY > 0) {
					this.GetComponent<GameManager> ().currentMoney -= (int)GameManager.MONEYCOST.MANUFACTORY;
					result = true;
				}
				break;
			}
		case ObjectPlacer.TYPEOFBUILDING.WATER :
			{
				if (this.GetComponent<GameManager> ().currentMoney - (int)GameManager.MONEYCOST.WATER > 0) {
					this.GetComponent<GameManager> ().currentMoney -= (int)GameManager.MONEYCOST.WATER;
					result = true;
				}
				break;
			}
		case ObjectPlacer.TYPEOFBUILDING.POWERSTATION :
			{
				if (this.GetComponent<GameManager> ().currentMoney - (int)GameManager.MONEYCOST.POWERSTATION > 0) {
					this.GetComponent<GameManager> ().currentMoney -= (int)GameManager.MONEYCOST.POWERSTATION;
					result = true;
				}
				break;
			}
		default :
			break;
		}
		return result;
	}
	
}
