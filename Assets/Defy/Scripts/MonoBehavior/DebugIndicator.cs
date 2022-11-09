using Defy.Component;
using KittyHelpYouOut;
using Unity.Entities;
using UnityEngine;

namespace Defy.MonoBehavior
{
    public class DebugIndicator : MonoBehaviour
    {
        public float FrameRateUpdateInterval=1f;
        public float FrameSafeGardStartTime;
        public float SafetyFrameRate = 5;
        private KittyTimer SafeGardTimer = new KittyTimer(true);
        private float _UpdateTimer = 0;
        private float _FrameRate;

        private void Start()
        {
            SafeGardTimer.StartTimer(FrameSafeGardStartTime);
        }

        private void Update()
        {
            if (_UpdateTimer<=0)
            {
                _FrameRate = 1f / Time.deltaTime;
                _UpdateTimer = FrameRateUpdateInterval;
            }
            else
            {
                _UpdateTimer -= Time.deltaTime;
            }
#if UNITY_EDITOR
            // if (SafeGardTimer.Tick(Time.deltaTime))
            // {
            //     if (1f/Time.deltaTime<SafetyFrameRate)
            //     {
            //         Debug.Break();
            //     }
            // }
#endif
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(Screen.width-65,15,50,50));
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_FrameRate.ToString("F3"));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}