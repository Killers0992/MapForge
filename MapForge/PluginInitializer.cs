#if EDITOR
#define NWAPI
#endif

using HarmonyLib;
using System;
using MapForge.API;

#if EXILED
using Exiled.API.Features;
using System.IO;
#endif

#if NWAPI
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Events;
#endif

namespace MapForge
{
    /// <summary>
    /// Plugin Initializer.
    /// </summary>
    public class PluginInitializer
#if EXILED
        : Plugin<PluginConfig>
#endif
    {
        Harmony _harmony;
        GameObjects _objects = new GameObjects();

#if EXILED
        public override string Name { get; } = BuildSettings.PluginName;
        public override string Author { get; } = BuildSettings.Author;
        public override Version Version { get; } = new Version(BuildSettings.Version);
#endif

#if NWAPI
        /// <summary>
        /// Handler for plugin.
        /// </summary>
        public PluginHandler Handler;
#endif

        void Initialize(string path)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony("com.killers0992.mapforge");
                _harmony.PatchAll();
            }

            Exiled.Events.Handlers.Server.WaitingForPlayers += InitializeObjects;

            MapForgeAPI.Initialize(path, _objects);

            StaticUnityMethods.OnUpdate += () =>
            {
                MapForgeAPI.CheckForFileChanges();
            };
        }

        void InitializeObjects()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= InitializeObjects;
            _objects.Initialize();
        }

#if EXILED
        public override void OnEnabled()
        {
            string mapForgePath = Path.Combine(Paths.Plugins, "MapForge");

            if (!Directory.Exists(mapForgePath))
                Directory.CreateDirectory(mapForgePath);

            Initialize(mapForgePath);
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            StaticUnityMethods.OnUpdate -= () =>
            {
                MapForgeAPI.CheckForFileChanges();
            };
            base.OnDisabled();
        }
#endif

#if NWAPI
        [PluginEntryPoint(BuildSettings.PluginName, BuildSettings.Version, "Plugin made for transforming SCP: SL map.", BuildSettings.Author)]
        void InitializeNWAPI()
        {            
            Handler = PluginHandler.Get(this);
            EventManager.RegisterEvents(this);

            Initialize(Handler.PluginDirectoryPath);
        }

        [PluginEvent]
        void OnWaitingForPlayers(WaitingForPlayersEvent ev) => InitializeObjects();
#endif
    }
}
