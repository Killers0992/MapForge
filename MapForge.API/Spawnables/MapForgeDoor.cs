using MapForge.API.Enums;
using MapForge.API.Models;

namespace MapForge.API.Spawnables
{
    public class MapForgeDoor : SpawnableInfo
    {
        public override SpawnableType Type { get; } = SpawnableType.Door;

        public SpawnableDoorType DoorType;

        public bool IsOpened;

        public bool Bypass2176;

        public bool RequireAllPermissions;

        public DoorKeycardPermissions Permissions;
    }
}
