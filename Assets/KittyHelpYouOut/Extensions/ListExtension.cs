using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace KittyHelpYouOut
{
	public static class ListExtension
	{

		#region CustomMethod
        public static List<T1> Allocate<T1,T2>(this List<T1> from,List<T2> to,string fieldName)
        {
            var fieldInfo = typeof(T2).GetField(fieldName);
            if (from.Count==0)
            {
                to.ForEach(m => fieldInfo.SetValue(m, null));
                return from;
            }
            if (to.Count==0)
            {
                return from;
            }
            for (int i = 0; i < from.Count; i++)
            {
                if (i<to.Count)
                {
                    if (fieldInfo.GetType()!=typeof(T1))
                    {
                        Debug.LogError("Type not fit: " + from + to);
                    }
                    fieldInfo.SetValue(to[i],from[i]);
                }
                else
                {
                    break;
                }
            }
            return from;
        }

        public static List<T1> Allocate<T1, T2>(this List<T1> from, string fieldName, List<T2> to)
        {
            var fieldInfo = typeof(T1).GetField(fieldName);
            if (from.Count == 0)
            {
                to.ForEach(m => fieldInfo.SetValue(m, null));
                return from;
            }
            if (to.Count == 0)
            {
                return from;
            }
            for (int i = 0; i < from.Count; i++)
            {
                if (i < to.Count)
                {
                    if (fieldInfo.GetType() != typeof(T2))
                    {
                        Debug.LogError("Type not fit: " + from + to);
                    }
                    fieldInfo.SetValue(to[i], from[i]);
                }
                else
                {
                    break;
                }
            }
            return from;
        }

        public static bool Valid<T>(this List<T> user)
        {
            if (user!=null&&user.Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


    }
}


