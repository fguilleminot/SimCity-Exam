using UnityEngine;
using System.Collections;



public class MapGenerator : MonoBehaviour
{

	public Texture2D MapTexture;
	public Material material;
	public Vector3 size = new Vector3(200f, 30f, 200f);
	public Color BaseColor = new Color(4,99,47,255);
	public Color HeightColor = new Color(70,40,15,255);
	public Shader MapColoration;
	
	private float seuil;

	private int width;
	private int height;
	private int surface;

	private Vector3[] vertices;
	private Vector2[] uv;
	private Vector4[] tangents;
	private int[] triangles;

	void Start()
	{
		seuil = Random.Range(0.99f, 1f);
		
		if (MapTexture == null)
		{
			FractalTexture tmp = new FractalTexture(true, Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.z));
			tmp.Calculate();
			MapTexture = tmp.texture;
		}
		
		if (MapColoration == null)
			MapColoration= Shader.Find("MapColoration");
		
		Generate();
		gameObject.renderer.material.shader = MapColoration;
		gameObject.renderer.material.SetTexture("_MainTex", MapTexture);
		gameObject.renderer.material.SetColor("_BaseColor", BaseColor);
		gameObject.renderer.material.SetColor("_HeightColor", HeightColor);
		gameObject.renderer.material.SetFloat("_MaxAltitude", size.y / 10f);
		gameObject.renderer.material.SetFloat("_Proportion", size.y / 50f);
	}

	// Build vertices and UVs
	void GenerateGraphicsParameters()
	{
		float randomPixelHeight;
		float pixelHeight;
		
		surface = width * height;
		vertices = new Vector3[height * width];
		uv = new Vector2[height * width];
		tangents = new Vector4[height * width];

		Vector2 uvScale = new Vector2(1.0f / (width - 1), 1.0f / (height - 1));
		Vector3 sizeScale = new Vector3(size.x / (width - 1), size.y, size.z / (height - 1));

		for (int yIndex = 0; yIndex < height; yIndex++)
		{
			for (int xIndex = 0; xIndex < width; xIndex++)
			{
				// Correction of the heightMap to obtain a limit of plane pixel
				randomPixelHeight = Random.Range(0f, 1f);
				if (randomPixelHeight < seuil)
					pixelHeight = 0f;
				else
					pixelHeight = MapTexture.GetPixel(xIndex, yIndex).grayscale;
				Vector3 vertex = new Vector3(xIndex, pixelHeight, yIndex);
				vertices[yIndex * width + xIndex] = Vector3.Scale(sizeScale, vertex);
				uv[yIndex * width + xIndex] = Vector2.Scale(new Vector2(xIndex, yIndex), uvScale);

				// Calculate tangent vector: a vector that goes from previous vertex
				// to next along X direction. We need tangents if we intend to
				// use bumpmap shaders on the mesh.
				Vector3 vertexL = new Vector3(xIndex - 1, MapTexture.GetPixel(xIndex - 1, yIndex).grayscale, yIndex);
				Vector3 vertexR = new Vector3(xIndex + 1, MapTexture.GetPixel(xIndex + 1, yIndex).grayscale, yIndex);
				Vector3 tan = Vector3.Scale(sizeScale, vertexR - vertexL).normalized;
				tangents[yIndex * width + xIndex] = new Vector4(tan.x, tan.y, tan.z, -1.0f);
			}
		}
	}

	// Build triangle indices: 3 indices into vertex array for each triangle
	void GenerateTriangles()
	{
		triangles = new int[(height - 1) * (width - 1) * 6];
		int index = 0;
		for (int yIndex = 0; yIndex < height - 1; yIndex++)
		{
			for (int xIndex = 0; xIndex < width - 1; xIndex++)
			{
				// For each grid cell output two triangles
				triangles[index++] = (yIndex * width) + xIndex;
				triangles[index++] = ((yIndex + 1) * width) + xIndex;
				triangles[index++] = (yIndex * width) + xIndex + 1;

				triangles[index++] = ((yIndex + 1) * width) + xIndex;
				triangles[index++] = ((yIndex + 1) * width) + xIndex + 1;
				triangles[index++] = (yIndex * width) + xIndex + 1;
			}
		}
	}

	void Generate()
	{
		//default size
		width = Mathf.Min(MapTexture.width, 255);
		height = Mathf.Min(MapTexture.height, 255);

		// Create the game object containing the renderer
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent("MeshRenderer");
		if (material)
			renderer.material = material;
		else
			renderer.material.color = Color.white;
			//renderer.material.color = new Color(4f,99f,47f);

		// Retrieve a mesh instance
		Mesh mesh = GetComponent<MeshFilter>().mesh;

		// Call the build of vertices and UVs
		GenerateGraphicsParameters();

		// Assign them to the mesh
		mesh.vertices = vertices;
		mesh.uv = uv;

		// Call the build of triangle indices
		GenerateTriangles();

		// And assign them to the mesh
		mesh.triangles = triangles;

		// Auto-calculate vertex normals from the mesh
		mesh.RecalculateNormals();

		// Assign tangents after recalculating normals
		mesh.tangents = tangents;
	}

}
