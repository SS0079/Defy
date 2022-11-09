using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Defy.MonoBehavior
{
    public class LoadScene : MonoBehaviour
    {
        private void Start()
        {
            // // Note: calling GetSceneGUID is slow, please keep reading for a proper example.
            // var sceneSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SceneSystem>();
            // var guid = sceneSystem.GetSceneGUID("Assets/Defy/Scenes/PrototypeScene.unity");
            // var sceneEntity = sceneSystem.LoadSceneAsync(guid);
            SceneManager.LoadScene("PrototypeScene");
        }
    }
}