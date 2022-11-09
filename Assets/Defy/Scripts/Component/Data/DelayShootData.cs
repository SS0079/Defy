using Unity.Entities;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public partial struct DelayShootData: IComponentData
    {
        public float Value;
    }
}