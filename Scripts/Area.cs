/*****************************************************************************
* Project: MapGen
* File   : Area.cs
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
using nPool;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Area container with settings for if flat, the height it gets then, which 
/// min height and maxheight on the noisemap this area contains,
/// and arrays for objects. 
/// For each objects Objectcontainer there has to be an 
/// objNum declared or an error occurs.
/// </summary>
[System.Serializable]
public class Area
{
    public float minHeight,
                 maxHeight,
                 targetHeight = 0;
    public bool isFlat, isObjectField;
    public float objectChance = 0f;
    public Color color;
    public bool[,] areaCells;
    [System.NonSerialized] public List<Vector2Int> cellsPos;
    public ObjectContainer[] objects;
    public ObjectPool[] objPools;
    public List<ObjData>[] objLists;
    public int[] objNum;
    [System.NonSerialized] public int[] currObjNum;
}

