using UnityEngine;
using System.Collections;

public class FractalTexture
{

	bool gray;
	int width;
	int height;

	float lacunarity;
	float h;
	float octaves;
	float offset;
	float scale;

	float offsetPos;

	public Texture2D texture;
	private Perlin perlin;
	private FractalNoise fractal;

	public FractalTexture(bool gray = true, int width = 128, int height = 128, float lacunarity = 6.18f, float h = 0.99f,
						  float octaves = 4f, float offset = 0.75f, float scale = 0.09f, float offsetPos = 0.0f)
	{
		this.gray = gray;
		this.width = width;
		this.height = height;
		this.lacunarity = lacunarity;
		this.h = h;
		this.octaves = octaves;
		this.offset = offset;
		this.scale = scale;
		this.offsetPos = offsetPos;

		texture = new Texture2D(width, height, TextureFormat.RGB24, false);
	}

	public void Calculate()
	{
		
		if (perlin == null)
			perlin = new Perlin();
		
		fractal = new FractalNoise(h, lacunarity, octaves, perlin);

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				if (gray)
				{
					var value = fractal.HybridMultifractal(x * scale + Time.time, y * scale + Time.time, offset);
					texture.SetPixel(x, y, new Color(value, value, value, value));
				}
				else
				{
					offsetPos = Time.time;
					var valuex = fractal.HybridMultifractal(x * scale + offsetPos * 0.6f, y * scale + offsetPos * 0.6f, offset);
					var valuey = fractal.HybridMultifractal(x * scale + 161.7f + offsetPos * 0.2f, y * scale + 161.7f + offsetPos * 0.3f, offset);
					var valuez = fractal.HybridMultifractal(x * scale + 591.1f + offsetPos, y * scale + 591.1f + offsetPos * 0.1f, offset);
					texture.SetPixel(x, y, new Color(valuex, valuey, valuez, 1));
				}
			}
		}

		texture.Apply();
	}
}
