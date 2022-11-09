using Defy.MonoBehavior;
using Unity.Entities;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public class UpgradeUIReference : IComponentData
    {
        public UpgradePanelController Value;
        
    }
}