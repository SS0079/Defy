using Defy.MonoBehavior;
using Unity.Entities;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public class AmmoUIReference : IComponentData
    {
        public AmmoPanelController Value;
    }
}