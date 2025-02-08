using InventorySystem.Items;
using InventorySystem;
using InventorySystem.Items.Pickups;
using MapForge.API.Enums;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using AdminToys;
using Mirror;
using System;

/// <summary>
/// Extensions for MapForge.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Calculates global scale ignoring own transform.
    /// </summary>
    /// <param name="transform">The transform.</param>
    /// <returns>Global scale</returns>
    public static Vector3 CalculateGlobalScale(this Transform transform)
    {
        if (transform.parent == null)
            return transform.localScale;

        Transform currentParent = transform.parent;
        Vector3 globalScale = currentParent.localScale;

        if (currentParent.parent == null)
        {
            return currentParent.localScale;
        }

        currentParent = currentParent.parent;

        while (currentParent != null)
        {
            globalScale = Vector3.Scale(globalScale, currentParent.localScale);
            currentParent = currentParent.parent;
        }

        return globalScale;
    }

    /// <summary>
    /// Converts MapForge variant to SCP: SL.
    /// </summary>
    /// <param name="type">The item type.</param>
    /// <returns>Item Type.</returns>
    public static ItemType ToItemType(this SpawnableItemType type)
    {
        return (ItemType)type;
    }

    /// <summary>
    /// Converts MapForge variant to SCP: SL.
    /// </summary>
    /// <param name="flags">The door permissions.</param>
    /// <returns>Door Permissions.</returns>
    public static KeycardPermissions ToKeycardPermission(this DoorKeycardPermissions permission)
    {
        return (KeycardPermissions)permission;
    }

    /// <summary>
    /// Converts MapForge variant to SCP: SL.
    /// </summary>
    /// <param name="flags">The primitive flags.</param>
    /// <returns>Primitive flags.</returns>
    public static PrimitiveFlags ToPrimitiveFlags(this PrimitiveExtraFlags flags)
    {
        return (PrimitiveFlags) flags;
    }

    /// <summary>
    /// Converts ItemType to Pickup instance.
    /// </summary>
    /// <param name="type">The type of item.</param>
    /// <param name="position">The position of pickup.</param>
    /// <param name="rotation">The rotation of pickup.</param>
    /// <param name="scale">The scale of pickup.</param>
    /// <param name="parent">The transfrom where object will be attached to.</param>
    /// <returns>New pickup instance.</returns>
    public static ItemPickupBase ToPickup(this ItemType type, Transform parent)
    {
        if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase ib))
            return null;

        if (ib.PickupDropModel == null)
            return null;

        ItemPickupBase newPickup = UnityEngine.Object.Instantiate(ib.PickupDropModel);
        SetDefaults(newPickup.transform, parent);

        newPickup.Info = new PickupSyncInfo(type, ib.Weight);

        return newPickup;
    }

    /// <summary>
    /// Creates new instance of specific type.
    /// </summary>
    /// <typeparam name="T">The instance type.</typeparam>
    /// <param name="parent">The transform where object will be atteched to.</param>
    /// <returns>New instance.</returns>
    public static T CreateNewInstance<T>(this T instance, Transform parent) where T : MonoBehaviour
    {
        T newInstance = UnityEngine.Object.Instantiate(instance);
        SetDefaults(newInstance.transform, parent);

        return newInstance;
    }


    /// <summary>
    /// Creates new instance of specific type.
    /// </summary>
    /// <param name="instance">The instance type.</param>
    /// <param name="position">The position of object.</param>
    /// <param name="rotation">The rotation of object</param>
    /// <param name="scale">The scale of object.</param>
    /// <param name="parent">The transform where object will be atteched to.</param>
    /// <returns>New instance.</returns>
    public static GameObject CreateNewInstance(this GameObject instance, Transform parent)
    {
        GameObject newInstance = UnityEngine.Object.Instantiate(instance);
        SetDefaults(newInstance.transform, parent);

        return newInstance;
    }

    /// <summary>
    /// Locks pickup which forbids player from picking it up.
    /// </summary>
    /// <param name="pickup">The pickup.</param>
    public static void Lock(this ItemPickupBase pickup)
    {
        pickup.Info.Locked = true;
    }

    /// <summary>
    /// Adds specific flag into enum.
    /// </summary>
    /// <param name="enum">The enum.</param>
    /// <param name="flags">The flag.</param>
    /// <returns>Flags</returns>
    public static PrimitiveFlags Add(this PrimitiveFlags @enum, PrimitiveFlags flags)
    {
        return @enum |= flags;
    }

    /// <summary>
    /// Removes specific flag from enum.
    /// </summary>
    /// <param name="enum">The enum.</param>
    /// <param name="flags">The flag.</param>
    /// <returns>Flags</returns>
    public static PrimitiveFlags Remove(this PrimitiveFlags @enum, PrimitiveFlags flags)
    {
        return @enum &= ~flags;
    }

    /// <summary>
    /// Sets specific flag in enum.
    /// </summary>
    /// <param name="enum">The enum.</param>
    /// <param name="flags">The flag.</param>
    /// <returns>Flags</returns>
    public static PrimitiveFlags Set(this PrimitiveFlags @enum, PrimitiveFlags flags, bool isEnabled)
    {
        return isEnabled ? @enum.Add(flags) : @enum.Remove(flags);
    }

    static void SetDefaults(Transform t, Transform parent)
    {
        t.transform.parent = parent;
        t.transform.localPosition = Vector3.zero;
        t.transform.localRotation = Quaternion.identity;
        t.transform.localScale = Vector3.one;
    }
}