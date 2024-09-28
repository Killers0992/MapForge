using MapForge.API.Enums;
using MapForge.API.Models;

namespace MapForge.API.Spawnables
{
    public class MapForgeLocker : SpawnableInfo
    {
        public override SpawnableType Type { get; } = SpawnableType.Locker;

        public SpawnableLockerType LockerType;
    }
}
