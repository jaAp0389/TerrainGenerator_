//this was from this youtube series https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=1
//episode 2 i think
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NoiseMapDisplay : MonoBehaviour
{
    [SerializeField] Renderer textureRenderer;

    [SerializeField] int mSize = 10;
    void Awake()
    {
        //textureRenderer = GetComponent<Renderer>();
    }

    void DrawNoiseMap(float[,] _noiseMap)
    {
        int width = _noiseMap.GetLength(0);
        int height = _noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    float[,] CreateNoisMap()
    {
        float[,] newMap = new float[mSize, mSize];

        for (int x = 0; x < mSize; x++)
        {
            for (int y = 0; y < mSize; y++)
            {
                newMap[x, y] = Mathf.PerlinNoise(x/mSize, y/mSize);//PerlinNoiseGen.GetNoiseOctave2(x, y, mNOctaves, mPersistance, mLactitude);
            }
        }
        return newMap;
    }
    void OnValidate()
    {
        //DrawNoiseMap(PerlinNoiseGen.CreateNoiseMap(mSize, mScale, mNOctaves, mPersistance, mLactitude));
    }
}
