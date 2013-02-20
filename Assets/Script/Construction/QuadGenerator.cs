using UnityEngine;
using System.Collections;

public class QuadGenerator : MonoBehaviour {
	
	public Vector3[] newVertices = { new Vector3(-5,-5,0), new Vector3(5,-5,0), new Vector3(5,5,0), new Vector3(-5,5,0) };
	public Vector2[] newUV = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1,0) };
	public int[] newTriangles = { 0, 3, 2, 0, 2, 1 };
	public Material defaultMaterial;
	
	
	// Use this for initialization
	void Start () {
		MeshCreator();
	}
	
	void MeshCreator()
	{
		MeshFilter mf = GetComponent<MeshFilter>();
		if (!mf)
			mf = this.gameObject.AddComponent<MeshFilter>();
		
		Mesh mesh = mf.mesh;
		if (!mesh)
		{
			mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh;
		}
		
		if (mesh)
		{
			mesh.Clear();
			mesh.vertices = newVertices;
			mesh.uv = newUV;
			mesh.triangles = newTriangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
		
		MeshRenderer mr = GetComponent<MeshRenderer>();
		if(!mr)
			mr = this.gameObject.AddComponent<MeshRenderer>();
		
		Material mat = new Material(defaultMaterial);
		mr.material = mat;
	}
}
