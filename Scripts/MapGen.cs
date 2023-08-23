/*****************************************************************************
* Project: MapGen
* File   : MapGen.cs
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
using nPool;

public class MapGen : MonoBehaviour
{
    MeshFilter mMeshFilter;
    MeshRenderer mMeshRenderer;

    Mesh mCustomMesh;

    [SerializeField] SettingsScriptable currSet;

    bool isShifting = false;
    float autoShiftX, autoShiftY;

    void Awake()
    {
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshRenderer = GetComponent<MeshRenderer>();

        mCustomMesh = new Mesh();
        mMeshFilter.sharedMesh = mCustomMesh;
        mMeshRenderer.sharedMaterial.mainTexture = null;
    }
    private void Start()
    {
        FillMesh();
    }
    void FixedUpdate()
    {
        if (currSet.IsAutoShift)
        {
            if (!isShifting)
            {
                isShifting = true;
                autoShiftX = 0f;
                autoShiftY = 0f;
            }
            autoShiftX += currSet.ShiftX;
            autoShiftY += currSet.ShiftY;
        }
        else if (isShifting) isShifting = false;

        if (mCustomMesh == null) return;
        FillMesh();

    }
    void GetTexture(Texture2D _texture)
    {
        mMeshRenderer.sharedMaterial.mainTexture = _texture;
        //mMeshRenderer.transform.localScale = new Vector3(mSize, 1, mSize);
    }

    /// <summary>
    /// Creates a mesh plane and also sets the height of each vertice according 
    /// to the perlinnoismap. 
    /// </summary>
    void FillMesh()
    {
        if (currSet.MapRes < 2) return;
        //Vector3[] verts = new Vector3[mSize * mSize];
        Vector3[] verts = new Vector3[currSet.MapRes * currSet.MapRes];
        //int[] trisIdx = new int[(mSize - 1) * (mSize - 1) * 2 * 3];
        int[] trisIdx = new int[(currSet.MapRes - 1) * (currSet.MapRes - 1) * 2 * 3];
        Vector2[] uvs = new Vector2[currSet.MapRes * currSet.MapRes];

        Vector3 startPos = new Vector3(currSet.MapSize, 0, currSet.MapSize) * -0.5f; 

        float[,] noiseMap = PerlinNoiseGen.CreateNoiseMap(currSet.MapRes, 
            currSet.Scale, currSet.Octaves, currSet.Persistance, currSet.Lactitude, 
            currSet.DefaultColor, currSet.IsAutoShift ? autoShiftX : currSet.ShiftX, currSet.IsAutoShift ? 
            autoShiftY : currSet.ShiftY, currSet.Areas);

        int currentTriIdx = 0;
        int currVertIdx = 0;

#region aus Vorlesung "generische Welten" with additions

        for (int y = 0; y < currSet.MapRes; y++)
        {
            for (int x = 0; x < currSet.MapRes; x++)
            {
                Vector2 percent = new Vector2(x, y) / (currSet.MapRes - 1); //---

                currVertIdx = y * currSet.MapRes + x;

                verts[currVertIdx] = startPos + Vector3.right * percent.x * currSet.MapSize
                                            + Vector3.forward * percent.y * currSet.MapSize
                           + Vector3.up * currSet.HeightCurve.Evaluate(noiseMap[x, y]) 
                           * currSet.HeightMulti;
                uvs[x* currSet.MapRes + y] = new Vector2((float)x / currSet.MapRes, (float)y / currSet.MapRes);

                if (x < currSet.MapRes - 1 && y < currSet.MapRes - 1)
                {
                    trisIdx[currentTriIdx + 0] = currVertIdx + 0;
                    trisIdx[currentTriIdx + 1] = currVertIdx + currSet.MapRes + 1;
                    trisIdx[currentTriIdx + 2] = currVertIdx + 1;

                    trisIdx[currentTriIdx + 3] = currVertIdx + 0;
                    trisIdx[currentTriIdx + 4] = currVertIdx + currSet.MapRes;
                    trisIdx[currentTriIdx + 5] = currVertIdx + currSet.MapRes + 1;

                    currentTriIdx += 6;
                }
            }
        }

        mCustomMesh.Clear();

        mCustomMesh.vertices = verts;
        mCustomMesh.triangles = trisIdx;
        mCustomMesh.uv = uvs;
        mCustomMesh.RecalculateNormals();

#endregion //aus Vorlesung "generische Welten"

        GetTexture(PerlinNoiseGen.ReturnTexture());

        foreach(Area _area in currSet.Areas)
        {
            if(_area.isObjectField)
                PlaceObjects(_area, noiseMap);
        }
    }

    /// <summary>
    /// Places Objects from area objectpool in an area defined by an area 
    /// bool map from the perlin noise gen. Picks a random location that 
    /// isn't taken in the area and checks if there are no neighbours.
    /// Maintains a list of active objects and shifts their position according 
    /// to the map movement.
    /// </summary>

    void PlaceObjects(Area _area, float[,] _nMap)
    {
        if(_area.objPools == null)
        {
            _area.objPools = new ObjectPool[_area.objects.Length];
            _area.objLists = new List<ObjData>[_area.objects.Length];
            for (int i = 0; i < _area.objects.Length; i++)
            {
                _area.objPools[i] = new ObjectPool(_area.objects[i].GameObj, 1);
                _area.objLists[i] = new List<ObjData>();
            }
        }

        if (currSet.ShiftX != 0 || currSet.ShiftY != 0)
            for (int i = _area.objects.Length -1; i >= 0; i--)
            {
                for(int o = _area.objLists[i].Count-1; o >= 0; o--)
                {
                    ObjData od = _area.objLists[i][o];
                    float mapRelation = (float)currSet.MapRes / (float)currSet.MapSize;
                    od.obj.transform.position -= currSet.ShiftX * new Vector3(1, 0, 0) 
                        / mapRelation + currSet.ShiftY * new Vector3(0, 0, 1) 
                        / mapRelation;
                    if (od.obj.transform.position.x < transform.position.x - (float)currSet.MapSize / 2 ||
                        od.obj.transform.position.x > transform.position.x + (float)currSet.MapSize / 2 ||
                        od.obj.transform.position.z < transform.position.z - (float)currSet.MapSize / 2 ||
                        od.obj.transform.position.z > transform.position.z + (float)currSet.MapSize / 2)
                    {
                        _area.objPools[i].ReturnObject(od.obj);
                        _area.objLists[i].Remove(od);
                        continue;
                    }
                    int temp = currSet.MapRes / currSet.MapSize;
                    od.pos += new Vector2Int((int)(temp * currSet.ShiftX), (int)(temp * currSet.ShiftY));
                }
            }
        for (int i = 0; i < _area.objects.Length; i++)
        {
            int missingObj = _area.objNum[i] - _area.objLists[i].Count;
            if (missingObj > 0)
            {
                if (Random.Range(0, missingObj) > _area.objects[i].chance)
                    break;
                for (int o = 0; o < missingObj; o++)
                {
                    GameObject obj = _area.objPools[i].GetObject();
                    Vector2Int pos = GetFreePosition(_area);
                    float height = currSet.HeightCurve.Evaluate(_nMap[pos.x, pos.y]) * currSet.HeightMulti;
                    float posX = transform.position.x - (float)currSet.MapSize / 2 + ((float)currSet.MapSize / (float)currSet.MapRes) * pos.x;
                    float posY = transform.position.y - (float)currSet.MapSize / 2 + ((float)currSet.MapSize / (float)currSet.MapRes) * pos.y;
                    obj.transform.position = new Vector3( posX, height, posY);
                    obj.transform.localScale = Vector3.one * _area.objects[i].objSize;
                    obj.SetActive(true);
                    _area.objLists[i].Add(new ObjData(obj, pos));
                    //_area.objNum[i] -= 1;
                }
            }
        }
    }

    /// <summary>
    /// The free and random position finder function for the placeObjects 
    /// function. Also Clamps to one side of the map for possible positions if 
    /// the map is moving.
    /// </summary>
    Vector2Int GetFreePosition(Area _area)
    {
        bool shiftNorth = currSet.ShiftY < 0, 
             shiftEast = currSet.ShiftX < 0, 
             shiftV = currSet.ShiftY != 0f, 
             shiftH = currSet.ShiftX != 0f;
        int tries = 100;
        while(true)
        {
            tries -= 1;
            if (tries <= 0)
            {
                print("freepos: no pos found");
                return new Vector2Int(0, 0);
            }
            Vector2Int pos = new Vector2Int(0, 0);
            int mapMax = currSet.MapRes - 1;

            if (isShifting)
                pos = new Vector2Int( shiftH ? shiftEast ? 0 : mapMax : Random.Range(0, mapMax),
                                      shiftV ? shiftNorth ? 0 : mapMax : Random.Range(0, mapMax));


            else pos = new Vector2Int(Random.Range(0, mapMax),
                                      Random.Range(0, mapMax));

            if (!_area.areaCells[pos.x, pos.y])
                continue;
            //Vector2Int pos = _area.cellsPos[Random.Range(0, _area.cellsPos.Count)];
            //_area.cellsPos.Remove(pos);
            for (int x = -3; x < 3; x++)
            {
                for (int y = -3; y < 3; y++)
                {
                    if(pos.x + x < 0 || pos.x + x > currSet.MapRes -1 ||
                       pos.y + y < 0 || pos.y + y > currSet.MapRes - 1)
                        continue;
                    if (!_area.areaCells[pos.x + x, pos.y + y])
                        goto End;
                }
            }
            _area.areaCells[pos.x, pos.y] = false;
            return pos;
        End: continue;
        }
    }


    private void OnValidate()
    {
        if (mCustomMesh == null) return;
        FillMesh();
    }
}
