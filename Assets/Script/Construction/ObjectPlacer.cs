using UnityEngine;
using System.Collections;

public class ObjectPlacer : MonoBehaviour
{
	public GameObject[] tmp;
	public GameObject prefabToBuild;
	public GameObject SonOf;
	public System.Collections.Generic.List<GameObject> ObjectCreated;
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
	
	public enum TYPEOFBUILDING
	{
		HOUSE,
		STORE,
		MANUFACTORY,
		WATER,
		POWERSTATION,
		DESTROY
	}
	
	// Use this for initialization
	void Start ()
	{
		prefabToBuild = null;
		mouseDown = false;
		camPos = Camera.main.transform.position;
		ObjectCreated = new System.Collections.Generic.List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		InputKeyboardManager ();

		//Capture the position when MouseButtonPressed
		if (Input.GetMouseButtonDown (0)) {
			mousePos = Camera.main.ScreenToViewportPoint (Input.mousePosition) * camPos.y;
			mouseDown = true;
			Debug.Log ("capture position pression " + mousePos);
		}
		if (Input.GetMouseButtonUp (0) && mouseDown) {
			//Possibility of areaCreationPrefab
			otherPos = Camera.main.ScreenToViewportPoint (Input.mousePosition) * camPos.y;
			Debug.Log ("capture position apres pression " + otherPos);
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
		} else if (Input.GetKeyDown (KeyCode.H)) {
			prefabToBuild = tmp [(int)TYPEOFBUILDING.HOUSE];
			destroyMode = false;
		} else if (Input.GetKeyDown (KeyCode.S)) {
			prefabToBuild = tmp [(int)TYPEOFBUILDING.STORE];
			destroyMode = false;
		} else if (Input.GetKeyDown (KeyCode.M)) {
			prefabToBuild = tmp [(int)TYPEOFBUILDING.MANUFACTORY];
			destroyMode = false;
		} else if (Input.GetKeyDown (KeyCode.W)) {
			prefabToBuild = tmp [(int)TYPEOFBUILDING.WATER];
			destroyMode = false;
		} else if (Input.GetKeyDown (KeyCode.P)) {
			prefabToBuild = tmp [(int)TYPEOFBUILDING.POWERSTATION];
			destroyMode = false;
		}
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
						if (checkPlaceToCreate (prefabToBuild.transform.position)) {
							prefabInstantiate = Instantiate (prefabToBuild) as GameObject;
							if (prefabInstantiate) {
								prefabInstantiate.transform.parent = SonOf.transform;
								//ObjectCreated.Add (prefabInstantiate);
								
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
			if (outinfo.transform.tag != TagMap)
				Destroy (outinfo.transform.gameObject);
			else {
				Vector3 destructionPos = prefabPosition (outinfo.point);
				//Debug.DrawRay (Camera.main.transform.position, destructionPos - Camera.main.transform.position, Color.red, 100f);
				
				//Debug.Log ("l " + left + " t " + top + " w " + width + " h " + height);
				for (float indexW = 0f; indexW <= width; indexW+=0.95f) {
					for (float indexH = 0f; indexH <= height; indexH+=0.95f) {
						toDestroy = checkDestroy (new Vector3 (destructionPos.x + indexW - 0.5f, 0.1f, destructionPos.z + indexH - 0.5f));
						if (toDestroy != null) {
							Destroy (toDestroy);
						}
					}
				}
			}
		}
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
	
}
