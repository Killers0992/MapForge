using AdminToys;
//using Dimensions;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Pickups;
using MapForge.API;
using MapForge.API.Enums;
using MapForge.API.Models;
using MapForge.API.Spawnables;
using MapGeneration.Distributors;
using MapGeneration.RoomConnectors;
using Mirror;
using UnityEngine;
using static InventorySystem.Items.Firearms.Modules.CylinderAmmoModule;
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
        public BreakableDoor BulkDoor;

        public GameObject GetAssetFromType(SpawnableAssetType type)
        {
            switch (type)
            {
                case SpawnableAssetType.Workstation:
                    return Workstation;

                default:
                    foreach(SpawnableRoomConnector connector in RoomConnectorDistributorSettings.RegisteredConnectors)
                    {
                        if (connector.SpawnData.ConnectorType == SpawnableRoomConnectorType.HczBulkDoor)
                            return connector.gameObject;
                    }

                    return Workstation;
            }
        }

        public BreakableDoor GetDoorFromType(SpawnableDoorType type)
        {
            switch (type)
            {
                case SpawnableDoorType.HeavyContaiment:
                    return HczDoor;

                case SpawnableDoorType.Entrance:
                    return EzDoor;

                case SpawnableDoorType.BulkDoor:
                    return BulkDoor;

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

        public override void SpawnObject(GameObject go, int dimensionId)
        {
            if (!go.TryGetComponent(out NetworkIdentity identity))
                return;

            //identity.SetDimensionId(dimensionId);
            NetworkServer.Spawn(go);
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

                    if (doorName.StartsWith("EZ Breakable"))
                    {
                        EzDoor = door;
                    }
                    else if (doorName.StartsWith("HCZ Brekable"))
                    {
                        HczDoor = door;
                    }
                    else if (doorName.StartsWith("LCZ Breakable"))
                    {
                        LczDoor = door;
                    }
                    else if (doorName.StartsWith("HCZ Bulk"))
                    {
                        BulkDoor = door;
                    }
                }
            }
        }

        public override GameObject OnSpawnObject(SpawnableInfo info)
        {
            switch (info)
            {
                default:
                    return null;
                case MapForgeDoor door:
                    BreakableDoor doorInstance = GetDoorFromType(door.DoorType).CreateNewInstance(door.transform);
                    info.SpawnedBy.SubObjects.Add(doorInstance.gameObject);

                    doorInstance.TargetState = door.IsOpened;

                    doorInstance.RequiredPermissions = new DoorPermissions()
                    {
                        RequireAll = door.RequireAllPermissions,
                        Bypass2176 = door.Bypass2176,
                        RequiredPermissions = door.Permissions.ToKeycardPermission(),
                    };
                    return doorInstance.gameObject;
                case MapForgeLocker locker:
                    Locker lockerInstance = GetLockerFromType(locker.LockerType).CreateNewInstance(locker.transform);
                    info.SpawnedBy.SubObjects.Add(lockerInstance.gameObject);
                    return lockerInstance.gameObject;
                case MapForgeAsset asset:
                    GameObject assetInstance = GetAssetFromType(asset.AssetType).CreateNewInstance(asset.transform);
                    info.SpawnedBy.SubObjects.Add(asset.gameObject);
                    return assetInstance;
                case MapForgePickup pickup:
                    ItemType item = pickup.Item.ToItemType();
                    
                    ItemPickupBase pickupInstance = item.ToPickup(info.transform);
                    info.SpawnedBy.SubObjects.Add(pickupInstance.gameObject);

                    pickup.ItemSerial = pickupInstance.Info.Serial;
                    MapForgePickup.Interactables.Add(pickupInstance.Info.Serial, pickup);

                    if (pickup.IsLocked)
                        pickupInstance.Lock();

                    return pickupInstance.gameObject;
                case MapForgePrimitive primitive:
                    PrimitiveObjectToy primitiveInstance = PrimitiveObject.CreateNewInstance(primitive.transform);
                    primitiveInstance.NetworkScale = primitive.Scale;
                    primitiveInstance.NetworkPosition = primitive.Position;
                    primitiveInstance.NetworkRotation = Quaternion.Euler(primitive.Rotation);

                    info.SpawnedBy.SubObjects.Add(primitiveInstance.gameObject);

                    // Setup intial values.

                    primitiveInstance.NetworkIsStatic = !primitive.IsAnimated;

                    primitiveInstance.NetworkScale = primitiveInstance.transform.CalculateGlobalScale();
                    primitiveInstance.NetworkMaterialColor = primitive.Color;
                    primitiveInstance.NetworkPrimitiveType = primitive.PrimitiveType.ToPrimitiveType();

                    PrimitiveFlags constructFlags = PrimitiveFlags.None.Set(PrimitiveFlags.Visible, primitive.IsVisible).Set(PrimitiveFlags.Collidable, primitive.IsCollidable);
                    primitiveInstance.NetworkPrimitiveFlags = constructFlags;

                    // Listen for changes.
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

                    return primitiveInstance.gameObject;
                case MapForgeLight light:
                    LightSourceToy lightInstance = LightObject.CreateNewInstance(light.transform);
                    lightInstance.NetworkScale = light.Scale;
                    lightInstance.NetworkPosition = light.Position;
                    lightInstance.NetworkRotation = Quaternion.Euler(light.Rotation);

                    info.SpawnedBy.SubObjects.Add(lightInstance.gameObject);

                    // Setup intial values.
                    lightInstance.NetworkIsStatic = !light.IsAnimated;

                    lightInstance.NetworkLightType = light.LightType.ToLightType();
                    lightInstance.NetworkLightColor = light.Color;
                    lightInstance.NetworkLightRange = light.Range;
                    lightInstance.NetworkLightIntensity = light.Intensity;
                    lightInstance.NetworkShadowType = light.ShadowType.ToShadowType();
                    lightInstance.NetworkShadowStrength = light.ShadowStrength;
                    lightInstance.NetworkLightShape = light.Shape.ToShapeType();
                    lightInstance.NetworkSpotAngle = light.SpotAngle;
                    lightInstance.NetworkInnerSpotAngle = light.InnerSpotAngle;

                    // Listen for changes.
                    light.LightTypeChanged += (SpawnableLightType type) =>
                    {
                        lightInstance.NetworkLightType = type.ToLightType();
                    };

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

                    light.ShadowTypeChanged += (LightShadowType type) =>
                    {
                        lightInstance.NetworkShadowType = type.ToShadowType();
                    };

                    light.ShadowStrengthChanged += (float strength) =>
                    {
                        lightInstance.NetworkShadowStrength = strength;
                    };

                    light.ShapeChanged += (LightShapeType type) =>
                    {
                        lightInstance.NetworkLightShape = type.ToShapeType();
                    };

                    light.SpotAngleChanged += (float angle) =>
                    {
                        lightInstance.NetworkSpotAngle = angle;
                    };

                    light.InnerSpotAngleChanged += (float angle) =>
                    {
                        lightInstance.NetworkInnerSpotAngle = angle;
                    };

                    return lightInstance.gameObject;
            }
        }
    }
}
