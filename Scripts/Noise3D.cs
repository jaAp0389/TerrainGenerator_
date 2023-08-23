using UnityEngine;

class Noise3D
{
    public static float Perlin3D(float _x, float _y, float _z)
    {
        float ab = Mathf.PerlinNoise(_x, _y);
        float bc = Mathf.PerlinNoise(_y, _z);
        float ac = Mathf.PerlinNoise(_x, _z);

        return ab + bc + ac;
    }
}

