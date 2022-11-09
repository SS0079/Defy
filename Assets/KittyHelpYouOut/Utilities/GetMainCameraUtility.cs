using UnityEngine;

namespace KittyHelpYouOut
{
    public static class GetMainCameraUtility
    {
        private static Camera _MainCamera;

        public static Camera MainCamera
        {
            get
            {
                _MainCamera ??= Camera.main;
                return _MainCamera;
            }
        }
    }
}