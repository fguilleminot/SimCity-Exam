using UnityEngine;
using System.Collections;



public class MapGenerator : MonoBehaviour
{

	public Texture2D MapTexture;
	public Material material;
	public Vector3 size = new Vector3(200, 30, 200);

	private int width;
	private int height;

	private Vector3[] vertices;
	private Vector2[] uv;
	private Vector4[] tangents;
	private int[] triangles;

	void Start()
	{
		if (MapTexture == null)
		{
			FractalTexture tmp = new FractalTexture();
			tmp.Calculate();
			MapTexture = tmp.texture;
			//MapTexture.width = tmp.texture.width;
			//MapTexture.height = tmp.texture.height;
		}

		Generate();
	}

	// Build vertices and UVs
	void GenerateGraphicsParameters()
	{

		vertices = new Vector3[height * width];
		uv = new Vector2[height * width];
		tangents = new Vector4[height * width];

		Vector2 uvScale = new Vector2(1.0f / (width - 1), 1.0f / (height - 1));
		Vector3 sizeScale = new Vector3(size.x / (width - 1), size.y, size.z / (height - 1));

		for (int yIndex = 0; yIndex < height; yIndex++)
		{
			for (int xIndex = 0; xIndex < width; xIndex++)
			{
				float pixelHeight = MapTexture.GetPixel(xIndex, yIndex).grayscale;
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
