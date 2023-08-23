/*****************************************************************************
* Project: MapGen
* File   : PrefabCreator.cs
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

namespace nPool
{
    public class PrefabCreator : MonoBehaviour
    {
        private void Awake()
        {
            RefLibrary.pFabCreator = this.GetComponent<PrefabCreator>();
        }
        public GameObject CreatePrefab(GameObject _object)
        {
            GameObject newObj =
                Instantiate(_object, transform.position, transform.rotation);
            newObj.transform.SetParent(transform);

            return newObj;
        }
    }
}
