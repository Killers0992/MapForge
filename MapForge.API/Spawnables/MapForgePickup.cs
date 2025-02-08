using System.Collections.Generic;
using MapForge.API.Enums;
using MapForge.API.Models;

namespace MapForge.API.Spawnables
{
    public class MapForgePickup : SpawnableInfo
    {
        public static Dictionary<ulong, MapForgePickup> Interactables = new Dictionary<ulong, MapForgePickup>();

        public override SpawnableType Type { get; } = SpawnableType.Pickup;

        public ulong ItemSerial { get; set; }

        public string InteractionEventName;

        public bool IsLocked;

        public SpawnableItemType Item;

        public void OnDestroy()
        {
            if (Interactables.ContainsKey(ItemSerial))
                Interactables.Remove(ItemSerial);
        }
    }
}
