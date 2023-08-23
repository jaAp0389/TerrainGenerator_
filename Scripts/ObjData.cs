/*****************************************************************************
* Project: MapGen
* File   : ObjData.cs
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
using UnityEngine;

public class ObjData
{
    public GameObject obj { get; set; }
    public Vector2Int pos { get; set; }

    public ObjData(GameObject _obj, Vector2Int _pos)
    {
        obj = _obj;
        pos = _pos;
    }
}
