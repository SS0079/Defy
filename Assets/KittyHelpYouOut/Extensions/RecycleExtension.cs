using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{
    public static class RecycleExtension
    {
        private static Vector3 FAR_AWAY = new Vector3(0, -100000, 0);
        private const float HIT_DEP_DELY = 3f;
        private const float RIGHT_NOW = 0f;
  

        public static GameObject Recycle(this GameObject go)
        {
            ExecuteRecycle(go);
            return go;
        }

        public static GameObject RecycleOnTime(this GameObject go,float second)
        {
            KittyCoroutine.Instance.StartCoroutine(ExecuteRecycleOnTime(go, second));
            return go;
        }


        public static MonoBehaviour RecycleOnTime(this MonoBehaviour cp, float second)
        {
            cp.StartCoroutine(ExecuteRecycleOnTime(cp.gameObject, second));
            return cp;
        }

        public static T Recycle<T>(this T cpt)where T : MonoBehaviour
        {
            ExecuteRecycle(cpt.gameObject);
            return cpt;
        }

        public static MonoBehaviour DelayedRecycle(this MonoBehaviour mono, float second=3f)
        {
            mono.StartCoroutine(ExecuteDelayedRecycle(mono.gameObject,second));
            return mono;
        }

        public static GameObject DelayedRecycle(this GameObject go, float second=3f)
        {
            KittyCoroutine.Instance.StartCoroutine(ExecuteDelayedRecycle(go, second));
            return go;
        }

        private static void ExecuteRecycle(GameObject go)
        {
            go.SetActive(false);
            go.transform.position = FAR_AWAY;
        }

        private static IEnumerator ExecuteDelayedRecycle(GameObject go, float second)
        {
            go.transform.position = FAR_AWAY;
            yield return new WaitForSeconds(second);
            go.SetActive(false); 
        }

        private static IEnumerator ExecuteRecycleOnTime(GameObject go,float second)
        {
            yield return new WaitForSeconds(second);
            go.SetActive(false);
        }
    }


}
