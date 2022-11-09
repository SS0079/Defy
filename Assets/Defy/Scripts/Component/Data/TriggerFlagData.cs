using Unity.Entities;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public partial struct TriggerFlagData : IComponentData
    {
        public bool Value;
    }
}