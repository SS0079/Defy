using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{
    public abstract class AbstractPerformanceAnalyzer : MonoBehaviour
    {
        private float WindowWidth = 400;
        private float WindowHeight = 300;
        private Rect _DebugWindowRect = default;
        private Rect DebugWindowRect
        {
            get
            {

                _DebugWindowRect = _DebugWindowRect == default ? new Rect(0, Screen.height - WindowHeight, WindowWidth, WindowHeight) : _DebugWindowRect;
                return _DebugWindowRect;
            }
            set
            {
                _DebugWindowRect = value;
            }
        }
        private bool ShowDebugInfo=false;
        protected float FrameRate;
        protected Dictionary<string, string> Infos=new Dictionary<string, string>();
        private void Start()
        {
            StartCoroutine(UpdateFPSEverySecond());
            SetUpInfos();
        }
        private void OnGUI()
        {
            ShowDebugInfo = GUI.Toggle(new Rect(15, 15, 100, 20), ShowDebugInfo, "ShowPerformance");
            if (ShowDebugInfo)
            {
                DebugWindowRect = GUI.Window(1, DebugWindowRect, DebugWindow, "Performance");
            }
        }
        private void DebugWindow(int windowID)
        {
            int lineHeight = 30;
            int infoCount=0;
            if (Infos.Count>0)
            {
                foreach (var info in Infos)
                {
                    if (infoCount % 2 != 0)//odd number column
                    {
                        GUI.Label(new Rect(203, 15 + lineHeight * (infoCount / 2), 194, lineHeight), info.Key+"= "+info.Value);
                    }
                    else
                    {
                        GUI.Label(new Rect(3, 15 + lineHeight * (infoCount / 2), 194, lineHeight), info.Key + "= " + info.Value);
                    }
                    infoCount++;
                }
            }
            GUI.DragWindow();
        }
        protected IEnumerator UpdateFPSEverySecond()
        {
            while (true)
            {
                FrameRate = 1f / Time.deltaTime;
                yield return new WaitForSeconds(1);
            }
        }
        private void Update()
        {
            UpdateInfos();
        }
        protected abstract void SetUpInfos();
        protected abstract void UpdateInfos();
    }
}