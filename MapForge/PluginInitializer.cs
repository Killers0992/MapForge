#if EDITOR
#define NWAPI
#endif

using HarmonyLib;
using InventorySystem.Items.Pickups;
using MapForge.API;
using MapForge.API.Models;
using MapForge.API.Spawnables;

namespace MapForge
{
    /// <summary>
    /// Plugin Initializer.
    /// </summary>
    public class PluginInitializer
    {
        static Harmony _harmony;
        static GameObjects _objects = new GameObjects();
        
        /// <summary>
        /// Main initialization method.
        /// </summary>
        /// <param name="path">The path of mapforge folder.</param>
        public static void Initialize(string path)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony("com.killers0992.mapforge");
                _harmony.PatchAll();
            }

            MapForgeAPI.Initialize(path, _objects);

            StaticUnityMethods.OnUpdate += () =>
            {
                MapForgeAPI.CheckForFileChanges();
            };
        }

        /// <summary>
        /// Initializes objects.
        /// </summary>
        public static void InitializeObjects()
        {
            _objects.Initialize();
        }

        /// <summary>
        /// Invoked when someone tries to search for pickup.
        /// </summary>
        /// <param name="hub">The hub searching for pickup.</param>
        /// <param name="pickup">The pickup being searched.</param>
        public static bool PickupInteraction(ReferenceHub hub, ItemPickupBase pickup)
        {
            if (!MapForgePickup.Interactables.TryGetValue(pickup.Info.Serial, out MapForgePickup fPickup))
                return true;

            PrefabInfo info = fPickup.SpawnedBy;

            if (info == null)
                return true;

            info.OnUsedInteraction?.Invoke(fPickup, hub.authManager.UserId);
            return false;
        }
    }
}
