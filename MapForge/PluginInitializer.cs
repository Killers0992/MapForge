using HarmonyLib;
using MapForge.API;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace MapForge
{
    /// <summary>
    /// Plugin Initializer.
    /// </summary>
    public class PluginInitializer
    {
        Harmony _harmony;

        /// <summary>
        /// Handler for plugin.
        /// </summary>
        public PluginHandler Handler;
        
        public GameObjects Objects = new GameObjects();

        [PluginEntryPoint("MapForge", "1.0.0", "Plugin made for transforming SCP: SL map.", "Killers0992")]
        void OnInitialize()
        {
            _harmony = new Harmony("com.killers0992.mapforge");
            _harmony.PatchAll();

            Handler = PluginHandler.Get(this);

            EventManager.RegisterEvents(this);

            MapForgeAPI.Initialize(Handler.PluginDirectoryPath, Objects);

            StaticUnityMethods.OnUpdate += () =>
            {
                MapForgeAPI.CheckForFileChanges();
            };
        }

        [PluginEvent]
        void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Objects.Initialize();
        }
    }
}
