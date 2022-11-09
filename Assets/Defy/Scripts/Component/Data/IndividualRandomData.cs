using Unity.Entities;
using Random=Unity.Mathematics.Random;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public partial struct IndividualRandomData : IComponentData
    {
        public Random Value;
        public void Init(uint seed) => Value = Random.CreateFromIndex(seed);
        public void Init(int seed) => Value = Random.CreateFromIndex((uint)seed);
    }
}