using AdminToys;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Pickups;
using MapForge.API;
using MapForge.API.Enums;
using MapForge.API.Models;
using MapForge.API.Spawnables;
using MapGeneration.Distributors;
using Mirror;
using UnityEngine;
using static PlayerList;

namespace MapForge
{
    public class GameObjects : ObjectsInfo
    {
        public LightSourceToy LightObject;
        public PrimitiveObjectToy PrimitiveObject;

        public ShootingTarget TargetSportObject;
        public ShootingTarget TargetDBoyObject;
        public ShootingTarget TargetBinaryObject;

        public Locker Locker;
        public Locker LargeGunLocker;
        public Locker RifleRackLocker;

        public Locker MedkitLocker;
        public Locker AdrenalineMedkitLocker;

        public PedestalScpLocker PedestalScp;

        public GameObject Workstation;

        public BreakableDoor LczDoor;
        public BreakableDoor HczDoor;
        public BreakableDoor EzDoor;

        public BreakableDoor GetDoorFromType(SpawnableDoorType type)
        {
            switch (type)
            {
                case SpawnableDoorType.HeavyContaiment:
                    return HczDoor;

                case SpawnableDoorType.Entrance:
                    return EzDoor;

                default:
                    return LczDoor;
            }
        }

        public Locker GetLockerFromType(SpawnableLockerType type)
        {
            switch (type)
            {
                case SpawnableLockerType.Big3x3:
                    return LargeGunLocker;

                case SpawnableLockerType.RifleRack:
                    return RifleRackLocker;

                case SpawnableLockerType.AdrenalineMedkit:
                    return AdrenalineMedkitLocker;

                case SpawnableLockerType.Medkit:
                    return MedkitLocker;

                case SpawnableLockerType.Scp:
                    return PedestalScp;

                default:
                    return Locker;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach(var spawnable in NetworkClient.prefabs.Values)
            {
                if (spawnable.TryGetComponent(out AdminToyBase toy))
                {
                    switch (toy)
                    {
                        case LightSourceToy light:
                            LightObject = light;
                            break;
                        case PrimitiveObjectToy primitive:
                            PrimitiveObject = primitive;
                            break;
                        case ShootingTarget target:
                            switch (target.CommandName)
                            {
                                case "TargetSport":
                                    TargetSportObject = target;
                                    break;
                                case "TargetDBoy":
                                    TargetDBoyObject = target;
                                    break;
                                case "TargetBinary":
                                    TargetBinaryObject = target;
                                    break;
                            }
                            break;
                    }
                }
                else if (spawnable.TryGetComponent(out SpawnableStructure structure))
                {
                    switch (structure)
                    {
                        case Locker locker:
                            switch (locker)
                            {
                                case PedestalScpLocker scpLocker:
                                    PedestalScp = scpLocker;
                                    break;
                                default:
                                    switch (locker.StructureType)
                                    {
                                        case StructureType.StandardLocker:
                                            Locker = locker;
                                            break;
                                        case StructureType.LargeGunLocker when !locker.gameObject.name.StartsWith("Rifle"):
                                            LargeGunLocker = locker;
                                            break;
                                        case StructureType.LargeGunLocker when locker.gameObject.name.StartsWith("Rifle"):
                                            RifleRackLocker = locker;
                                            break;
                                        case StructureType.SmallWallCabinet when !locker.gameObject.name.StartsWith("Regular"):
                                            AdrenalineMedkitLocker = locker;
                                            break;
                                        case StructureType.SmallWallCabinet when locker.gameObject.name.StartsWith("Regular"):
                                            MedkitLocker = locker;
                                            break;
                                    }
                                    break;
                            }
                            break;
                        default:
                            switch (structure.StructureType)
                            {
                                case StructureType.Workstation:
                                    Workstation = structure.gameObject;
                                    break;
                            }
                            break;
                    }
                }
                else if (spawnable.TryGetComponent(out BreakableDoor door))
                {
                    string doorName = door.gameObject.name;

                    if (doorName.StartsWith("EZ"))
                    {
                        EzDoor = door;
                    }
                    else if (doorName.StartsWith("HCZ"))
                    {
                        HczDoor = door;
                    }
                    else if (doorName.StartsWith("LCZ"))
                    {
                        LczDoor = door;
                    }
                }
            }
        }

        public override void OnSpawnObject(SpawnableInfo info)
        {
            switch (info)
            {
                case MapForgeDoor door:
                    BreakableDoor doorInstance = GetDoorFromType(door.DoorType).CreateNewInstance(door.Position, door.Rotation, door.Scale, info.transform);
                    info.SpawnedBy.SubObjects.Add(doorInstance.gameObject);

                    doorInstance.TargetState = door.IsOpened;

                    doorInstance.RequiredPermissions = new DoorPermissions()
                    {
                        RequireAll = door.RequireAllPermissions,
                        Bypass2176 = door.Bypass2176,
                        RequiredPermissions = door.Permissions.ToKeycardPermission(),
                    };

                    NetworkServer.Spawn(doorInstance.gameObject);
                    break;
                case MapForgeLocker locker:
                    Locker lockerInstance = GetLockerFromType(locker.LockerType).CreateNewInstance(locker.Position, locker.Rotation, locker.Scale, locker.transform);
                    info.SpawnedBy.SubObjects.Add(lockerInstance.gameObject);

                    NetworkServer.Spawn(lockerInstance.gameObject);
                    break;
                case MapForgePickup pickup:
                    ItemType item = pickup.Item.ToItemType();
                    
                    ItemPickupBase pickupInstance = item.ToPickup(pickup.Position, pickup.Rotation, pickup.Scale, info.transform);
                    info.SpawnedBy.SubObjects.Add(pickupInstance.gameObject);

                    if (pickup.IsLocked)
                        pickupInstance.Lock();

                    NetworkServer.Spawn(pickupInstance.gameObject);
                    break;
                case MapForgePrimitive primitive:
                    PrimitiveObjectToy primitiveInstance = PrimitiveObject.CreateNewInstance(primitive.Position, primitive.Rotation, primitive.Scale, primitive.transform);
                    primitiveInstance.NetworkScale = primitiveInstance.transform.CalculateGlobalScale();

                    info.SpawnedBy.SubObjects.Add(primitiveInstance.gameObject);

                    primitiveInstance.NetworkMaterialColor = primitive.Color;
                    primitiveInstance.NetworkPrimitiveType = primitive.PrimitiveType.ToPrimitiveType();

                    PrimitiveFlags constructFlags = PrimitiveFlags.None.Set(PrimitiveFlags.Visible, primitive.IsVisible).Set(PrimitiveFlags.Collidable, primitive.IsCollidable);
                    primitiveInstance.NetworkPrimitiveFlags = constructFlags;

                    primitive.ColorChanged += (Color color) =>
                    {
                        primitiveInstance.NetworkMaterialColor = color;
                    };

                    primitive.PrimitiveTypeChanged += (SpawnablePrimitiveType type) =>
                    {
                        primitiveInstance.NetworkPrimitiveType = type.ToPrimitiveType();
                    };

                    primitive.CollisionChanged += (bool newColision) =>
                    {
                        primitiveInstance.NetworkPrimitiveFlags = primitiveInstance.NetworkPrimitiveFlags.Set(PrimitiveFlags.Collidable, newColision);
                    };

                    primitive.VisibilityChanged += (bool newVisibility) =>
                    {
                        primitiveInstance.NetworkPrimitiveFlags = primitiveInstance.NetworkPrimitiveFlags.Set(PrimitiveFlags.Visible, newVisibility);
                    };

                    NetworkServer.Spawn(primitiveInstance.gameObject);
                    break;
                case MapForgeLight light:
                    LightSourceToy lightInstance = LightObject.CreateNewInstance(light.Position, light.Rotation, light.Scale, light.transform);
                    info.SpawnedBy.SubObjects.Add(lightInstance.gameObject);

                    lightInstance.NetworkLightColor = light.Color;
                    lightInstance.NetworkLightRange = light.Range;
                    lightInstance.NetworkLightIntensity = light.Intensity;
                    lightInstance.NetworkShadowType = light.Shadows ? LightShadows.Hard : LightShadows.None;

                    light.ColorChanged += (Color color) =>
                    {
                        lightInstance.NetworkLightColor = color;
                    };

                    light.RangeChanged += (float range) =>
                    {
                        lightInstance.NetworkLightRange = range;
                    };

                    light.IntensityChanged += (float intensity) =>
                    {
                        lightInstance.NetworkLightIntensity = intensity;
                    };

                    light.ShadowsChanged += (bool shadows) =>
                    {
                        lightInstance.NetworkShadowType = light.Shadows ? LightShadows.Hard : LightShadows.None;
                    };

                    NetworkServer.Spawn(lightInstance.gameObject);
                    break;
            }
        }
    }
}
