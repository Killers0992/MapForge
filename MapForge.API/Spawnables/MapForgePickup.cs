using MapForge.API.Enums;
using MapForge.API.Models;

namespace MapForge.API.Spawnables
{
    public class MapForgePickup : SpawnableInfo
    {
        public override SpawnableType Type { get; } = SpawnableType.Pickup;

        public bool IsLocked;

        public SpawnableItemType Item;
    }
}
