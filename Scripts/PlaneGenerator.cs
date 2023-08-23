///aus Vorlesung Generische Welten
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    MeshFilter mMeshFilter;
    MeshRenderer mMeshRenderer;

    Mesh mCustomMesh;

    [SerializeField] int mSize = 0,
                         mResolution = 0,
                         mNOctaves = 0;
    [SerializeField] float mNOffset = 0, mMin = 0;

    void Awake()
    {
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshRenderer = GetComponent<MeshRenderer>();

        mCustomMesh = new Mesh();
        mMeshFilter.sharedMesh = mCustomMesh;
    }
    private void Start()
    {
        FillMesh();
    }

    /// <summary>
    /// the original function from the vorlesung "generative Welten"
    /// </summary>
    void FillMesh()
    {
        if (mResolution < 2) return;
        //Vector3[] verts = new Vector3[mSize * mSize];
        Vector3[] verts = new Vector3[mResolution * mResolution];
        //int[] trisIdx = new int[(mSize - 1) * (mSize - 1) * 2 * 3];
        int[] trisIdx = new int[(mResolution - 1) * (mResolution - 1) * 2 * 3];

        Vector3 startPos = new Vector3(mSize, 0, mSize) * -0.5f; //new Vector3(mSize - 1, 0, mSize - 1) * -0.5f; 

        int currentTriIdx = 0;
        int currVertIdx = 0;
        for (int y = 0; y < mResolution; y++)
        {
            for (int x = 0; x < mResolution; x++)
            {
                Vector2 percent = new Vector2(x, y) / (mResolution - 1); //---

                currVertIdx = y * mResolution + x;

                verts[currVertIdx] = startPos + Vector3.right * percent.x * mSize + Vector3.forward * percent.y * mSize + Vector3.up * PerlinNoiseGen.GetNoiseOctave2(percent.x * mSize, percent.y * mSize, mNOctaves, mNOffset, mMin); //Mathf.PerlinNoise(percent.x * mSize, percent.y * mSize); //---

                if (x < mResolution - 1 && y < mResolution - 1)
                {
                    trisIdx[currentTriIdx + 0] = currVertIdx + 0;
                    trisIdx[currentTriIdx + 1] = currVertIdx + mResolution + 1;
                    trisIdx[currentTriIdx + 2] = currVertIdx + 1;

                    trisIdx[currentTriIdx + 3] = currVertIdx + 0;
                    trisIdx[currentTriIdx + 4] = currVertIdx + mResolution;
                    trisIdx[currentTriIdx + 5] = currVertIdx + mResolution + 1;

                    currentTriIdx += 6;
                }
            }
        }

        mCustomMesh.Clear();

        mCustomMesh.vertices = verts;
        mCustomMesh.triangles = trisIdx;
        mCustomMesh.RecalculateNormals();
    }
    void ScrambleMesh()
    {

    }

    private void OnValidate()
    {
        if (mCustomMesh == null) return;
        FillMesh();
    }
}
