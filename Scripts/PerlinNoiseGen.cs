/*****************************************************************************
* Project: MapGen
* File   : PerlinNoiseGen.cs
* Date   : 25.11.2021
* Author : Jan Apsel (JA)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*   15.11.2021	JA	Created
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PerlinNoiseGen
{
    static Texture2D mTexture;
    //unused but i'm a function messi
    public static float[,] CreateNoiseField(int _size, float _resolution)
    {
        float[,] NoiseField = new float[_size, _size];
        for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                NoiseField[x, y] = 
                    Mathf.PerlinNoise(x / _resolution, y / _resolution);
            }
        }
        return NoiseField;
    }
    //also not used. These were my own attempts at noiseGen.
    public static float[,] CreateNoiseField
        (int _size, float _resolution, int _octaves, float _ocOffset)
    {
        float[,] NoiseField = new float[_size, _size];
        for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int o = 0; o < _octaves; o++)
                {
                    NoiseField[x, y] +=
                        Mathf.PerlinNoise(x / _resolution + o * _ocOffset, 
                                          y / _resolution + o * _ocOffset);

                }
            }
        }
        return NoiseField;
    }
    //same
    public static float GetNoiseOctave(float _x, float _y, int _octaves, float _ocOffset, float _max, float _min)
    {
        float noise = Mathf.PerlinNoise(_x, _y);
        for (int o = 1; o <= _octaves; o++)
        {
            noise += Mathf.PerlinNoise(_x + _ocOffset * o, _y + _ocOffset * o) * 2 -1;
        }
        return noise;//Mathf.Clamp(noise, _min, _max);
    }
    //original perlinnoisefunction from https://www.youtube.com/watch?v=MRNFcywkUSA
    public static float GetNoiseOctave2(float _x, float _y, int _octaves, float _persistance, float _lacunarity)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int o = 0; o < _octaves; o++)
        {
            float sampleX = _x * frequency;
            float sampleY = _y * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            noiseHeight += perlinValue * amplitude;

            amplitude *= _persistance;
            frequency *= _lacunarity;
        }
        return noiseHeight;
    }

    /// <summary>
    /// PrelinNoise general function. Generates Perlinnoise map with octaves. 
    /// Creates a bool position map for each area.  Creates a texture with the 
    /// predefined colors from each area. 
    /// </summary>
    public static float[,] CreateNoiseMap(int _size, float _scale, 
        int _octaves, float _persistance, float _lacunarity, Color defaultColor, 
        float shiftX = 0f, float shiftY = 0f, params Area[] areas)
    {
        foreach (Area a in areas)
        {
            if(a.isObjectField)
            {
                //a.cellsPos = new List<Vector2Int>();
                a.areaCells = new bool[_size, _size];
            }
        }

        if (_scale <= 0) _scale = 0.001f;

        float[,] noiseMap = new float[_size, _size];
        mTexture = new Texture2D(_size, _size);
        //mTexture.filterMode = FilterMode.Point;
        Color[] colourMap = new Color[_size * _size];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //region from youtube
#region https://www.youtube.com/watch?v=MRNFcywkUSA

        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int o = 0; o < _octaves; o++)
                {
                    float sampleX = (x + shiftX)/ _scale * frequency;
                    float sampleY = (y + shiftY)/ _scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= _persistance;                               
                    frequency *= _lacunarity;                              
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

#endregion

                bool isPlateau = false;
                foreach (Area a in areas)
                    if (noiseHeight > a.minHeight && noiseHeight < a.maxHeight)
                    {
                        if(a.isObjectField)
                        {
                            //a.cellsPos.Add(new Vector2Int(x, y));
                            a.areaCells[x, y] = true;
                        }
                        noiseMap[x, y] = a.isFlat ? a.targetHeight : noiseHeight;
                        colourMap[x * _size +  y] = a.color;
                        isPlateau = true;
                    }
                if(!isPlateau)
                {
                    noiseMap[x, y] = noiseHeight;
                    colourMap[x * _size + y] = defaultColor;
                }   
            }
        }
        for (int y = 0; y < _size; y++)
            for (int x = 0; x < _size; x++)
                noiseMap[x, y] = Mathf.InverseLerp
                    (0f, 1.5f, noiseMap[x, y]);
        mTexture.SetPixels(colourMap);
        mTexture.Apply();
        return noiseMap;
    }

    static public Texture2D ReturnTexture()
    {
        return mTexture;
    }
}
