using Unity.Entities;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct RotateSpeedConfig : IComponentData
    {
        public float Value;
        public bool InstantRotate;

        public static RotateSpeedConfig GetDefault()
        {
            return new RotateSpeedConfig() { Value = 30, InstantRotate = false };
        }
    }
}