/*****************************************************************************
* Project: MapGen
* File   : SettingsScriptable.cs
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

/// <summary>
/// Scriptable general Settings.
/// </summary>

[CreateAssetMenu(fileName = "SettingsScriptable", menuName = "ScriptableObjects/SettingsScriptable", order = 1)]
public class SettingsScriptable : ScriptableObject
{
    [Header("Map")]
    [SerializeField] [Range(1, 100)] int mapSize = 10;
    [SerializeField] [Range(1, 100)] int mapResolution = 10;
    [SerializeField] [Range(0f, 100f)] float heightMulti = 1f;
    [SerializeField] [Range(-20f, 20f)] float shiftX = 0f;
    [SerializeField] [Range(-20f, 20f)] float shiftY = 0f;
    [SerializeField] bool isAutoShift;
    [SerializeField] AnimationCurve heightCurve;
    [SerializeField] Area[] areas;
    [SerializeField] Color defaultColor;
    public float shiftmulti;

    [Header("Noise")]
    [SerializeField] [Range(1, 16)] int perlinOctaves = 8;
    [SerializeField] [Range(0f, 1f)] float persistance = 0.5f;
    [SerializeField] [Range(0f, 18f)] float lactitude = 2.5f;
    [SerializeField] [Range(0f, 100f)] float scale = 25f;

    public int MapSize { get { return mapSize; }  private set{ } }
    public int MapRes { get { return mapResolution; } private set{ } }
    public float HeightMulti { get { return heightMulti; } private set{ } }
    public float ShiftX { get { return shiftX; } private set{ } }
    public float ShiftY { get { return shiftY; } private set{ } }
    public bool IsAutoShift { get { return isAutoShift; } private set { } }
    public AnimationCurve HeightCurve { get { return heightCurve; } private set { } }
    public Area[] Areas { get { return areas; } private set { } }
    public int Octaves { get { return perlinOctaves; } private set { } }
    public float Persistance { get { return persistance; } private set { } }
    public float Lactitude { get { return lactitude; } private set { } }
    public float Scale { get { return scale; } private set { } }
    public Color DefaultColor { get { return defaultColor; } private set { } }

}