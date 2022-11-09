using System;
using UnityEngine;

namespace KittyHelpYouOut
{
    public class KittyMonoSingleton<T> : MonoBehaviour where T : Component
    {
        // 单件子类实例
        private static T _instance;

        /// <summary>
        ///     获得单件实例，查询场景中是否有该种类型，如果有存储静态变量，如果没有，构建一个带有这个component的gameobject
        ///     这种单件实例的GameObject直接挂接在bootroot节点下，在场景中的生命周期和游戏生命周期相同，创建这个单件实例的模块
        ///     必须通过DestroyInstance自行管理单件的生命周期
        /// </summary>
        /// <returns>返回单件实例</returns>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Type theType = typeof(T);

                    _instance = (T)FindObjectOfType(theType);

                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();

                        //挂接到BootObj下
                        GameObject bootObj = GameObject.Find("BootObj");
                        if (bootObj == null)
                        {
                            bootObj = new GameObject("BootObj");
                            DontDestroyOnLoad(bootObj);
                        }
                        go.transform.SetParent(bootObj.transform);
                    }
                }


                return _instance;
            }
        }

        /// <summary>
        ///     删除单件实例,这种继承关系的单件生命周期应该由模块显示管理
        /// </summary>
        public static void DestroyInstance()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }
            _instance = null;
        }

        public static void ClearDestroy()
        {
            DestroyInstance();
        }

        /// <summary>
        ///     Awake消息，确保单件实例的唯一性
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance != null && _instance.gameObject != gameObject)
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject); // UNITY_EDITOR
                }
            }
            else if (_instance == null)
            {
                _instance = GetComponent<T>();
            }

            DontDestroyOnLoad(gameObject);

            Init();
        }

        /// <summary>
        ///     OnDestroy消息，确保单件的静态实例会随着GameObject销毁
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance != null && _instance.gameObject == gameObject)
            {
                _instance = null;
            }
        }

        public virtual void DestroySelf()
        {
            _instance = null;
            Destroy(gameObject);
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }

        protected virtual void Init()
        {

        }
    };

}
