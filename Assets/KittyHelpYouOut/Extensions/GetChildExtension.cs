using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{
	public static class GetChildExtension
	{

        #region CustomMethod

        public static List<T> GetComponentsOnlyInChildren<T>(this Component go)
        {
            List<T> TList = new List<T>();
            TList.AddRange(go.GetComponentsInChildren<T>());
            if (go.GetComponent<T>() != null)
            {
                TList.RemoveAt(0);
            }
            return TList;
        }

        public static T[] GetComponentsOnlyInChildren<T>(this GameObject go)
        {
            List<T> TList = new List<T>();
            TList.AddRange(go.GetComponentsInChildren<T>(true));
            if (go.GetComponent<T>() != null)
            {
                TList.RemoveAt(0);
            }
            return TList.ToArray();
        }

        public static List<T> GetNamedInChildren<T>(this GameObject user, string name) where T : Component
        {
            List<T> items = new List<T>(user.GetComponentsOnlyInChildren<T>());

            return items.FindAll(m => m.gameObject.name == name);
        }
        #endregion

    }
}


