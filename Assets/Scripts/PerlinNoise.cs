using System;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int xOffset = 100;
    public int yOffset = 100;
    public float scale = 20.0f;

    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.mainTexture = GenerateTexture();
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Color CalculateColor(int x, int y)
    {
        Vector2 coords = new Vector2(
            (float)x / width * scale + xOffset,
            (float)y / height * scale + yOffset
        );
        float sample = Mathf.PerlinNoise(coords.x, coords.y);
        return new Color(sample, sample, sample);
    }
}
