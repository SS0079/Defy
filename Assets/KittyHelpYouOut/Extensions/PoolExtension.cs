using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KittyHelpYouOut
{
	public static class PoolExtension
	{
        public static GameObject GetPoolObjectHere(this Transform reference, GameObject go,Transform parent=null)
        {
            return KittyPool.Instance.GetPoolObject(go, reference.position, reference.rotation);
        }


        public static GameObject GetPoolObject(this GameObject go,Vector3 pos=default,Quaternion rot=default,Transform parent=null)
        {
            return KittyPool.Instance.GetPoolObject(go, pos, rot, parent);
        }

    }
}


