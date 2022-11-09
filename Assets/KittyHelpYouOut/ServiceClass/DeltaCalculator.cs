using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{
    public class DeltaCalculator
    {
        private float lastValue = 0;
        private Vector3 lastVector = default;
        public float DeltaFloat(float crtValue)
        {
            float delta;
            if (lastValue == 0)
            {
                lastValue = crtValue;
            }
            delta = crtValue - lastValue;
            lastValue = crtValue;
            return delta;
        }

        public float DeltaVectorSignedAngle(Vector3 crtVector, Vector3 axis)
        {
            float delta;
            if (lastVector == default)
            {
                lastVector = crtVector;
            }
            delta = Vector3.SignedAngle(crtVector, lastVector, axis);
            lastVector = crtVector;
            return delta;
        }
    }
}


