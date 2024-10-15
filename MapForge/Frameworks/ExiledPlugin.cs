#if EXILED
using System;
using System.IO;

using MapForge.API;

using Exiled.API.Features;

namespace MapForge.Frameworks
{
    public class ExiledPlugin : Plugin<PluginConfig>
    {
        public override string Name { get; } = BuildSettings.PluginName;
        public override string Author { get; } = BuildSettings.Author;
        public override Version Version { get; } = new Version(BuildSettings.Version);

        public override void OnEnabled()
        {
            string mapForgePath = Path.Combine(Paths.Plugins, "MapForge");

            if (!Directory.Exists(mapForgePath))
                Directory.CreateDirectory(mapForgePath);

            Exiled.Events.Handlers.Server.WaitingForPlayers += PluginInitializer.InitializeObjects;

            PluginInitializer.Initialize(mapForgePath);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= PluginInitializer.InitializeObjects;

            StaticUnityMethods.OnUpdate -= () =>
            {
                MapForgeAPI.CheckForFileChanges();
            };

            base.OnDisabled();
        }
    }
}
#endif