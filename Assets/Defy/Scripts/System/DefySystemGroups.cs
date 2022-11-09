using Unity.Entities;
using Unity.Transforms;

namespace Defy.System
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class DefyUpdateGroup : ComponentSystemGroup
    {
        
    }
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public class DefyLateUpdateGroup : ComponentSystemGroup
    {
        
    }

    [UpdateInGroup(typeof(PresentationSystemGroup),OrderFirst = true)]
    public class DefyFinalUpdateGroup : ComponentSystemGroup
    {
        
    }
    
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class DefyInitializeGroup : ComponentSystemGroup{}
}