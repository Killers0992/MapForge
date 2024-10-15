#if EDITOR
#define NWAPI
#endif

using HarmonyLib;
using MapForge.API;

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
    }
}
