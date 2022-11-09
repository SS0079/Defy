using Unity.Entities;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct IncomingDamageData : IComponentData
    {
        public float Value;
        
    }
}