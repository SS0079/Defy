using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace KittyHelpYouOut
{
    public class KittyPool : KittyMonoSingleton<KittyPool>
    {
        private Dictionary<string, Queue<GameObject>> GameObjectPoolDictionary=new Dictionary<string, Queue<GameObject>>();
        public int poolIniSize = 1;
        public int poolMaxSize = 10000;


        private void AddGameObjectPool(GameObject addObj)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolIniSize; i++)
            {
                GameObject obj = Instantiate(addObj);
                obj.gameObject.SetActive(false);
                obj.name = addObj.name;
                objectPool.Enqueue(obj);
            }
            GameObjectPoolDictionary.Add(addObj.gameObject.name, objectPool);
        }


        /// <summary>
        /// 返回一个active的GameObject
        /// </summary>
        /// <param name="go"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="parent"></param>
        /// <param name="setActive"></param>
        /// <returns></returns>
        public GameObject GetPoolObject(GameObject go, Vector3 pos = default, Quaternion rot = default, Transform parent=null)
        {
            GameObject objToSpawn;
            if (!GameObjectPoolDictionary.ContainsKey(go.name))
            {
                AddGameObjectPool(go);
            }
            Queue<GameObject> queue = GameObjectPoolDictionary[go.name];
            objToSpawn = Bubble(queue);
            if (objToSpawn==null)
            {
                if (queue.Count<poolMaxSize)
                {
                    objToSpawn = Instantiate(go);
                }
                else
                {
                    objToSpawn = queue.Dequeue();
                    objToSpawn.Recycle();
                }
            }
            queue.Enqueue(objToSpawn);
            objToSpawn.transform.SetPositionAndRotation(pos, rot);
            objToSpawn.transform.SetParent(parent);
            objToSpawn.name = go.name;
            objToSpawn.SetActive(true);
            return objToSpawn;
        }
        //================================================================================

        private GameObject Bubble(Queue<GameObject> queue)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue.Peek().activeSelf)
                {
                    queue.Enqueue(queue.Dequeue());
                }
                else
                {
                    return queue.Dequeue();
                }
            }
            return null;
        }

    }
}


