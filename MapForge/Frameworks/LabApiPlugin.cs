#if LABAPI
using System;
using System.IO;
using LabApi.Events.Handlers;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using MapForge.API;

namespace MapForge.Frameworks
{
    public class LabApiPlugin : Plugin
    {
        public override string Name { get; } = BuildSettings.PluginName;

        public override string Description { get; } = BuildSettings.Author;

        public override string Author { get; } = BuildSettings.Author;

        public override Version Version { get; } = new Version(BuildSettings.Version);

        public override Version RequiredApiVersion { get; } = new Version(0, 4, 0);

        public override void Disable()
        {
            ServerEvents.WaitingForPlayers -= PluginInitializer.InitializeObjects;

            StaticUnityMethods.OnUpdate -= () =>
            {
                MapForgeAPI.CheckForFileChanges();
            };
        }

        public override void Enable()
        {
            DirectoryInfo mapForgePath = PathManager.Plugins.CreateSubdirectory("MapForge");

            ServerEvents.WaitingForPlayers += PluginInitializer.InitializeObjects;

            PluginInitializer.Initialize(mapForgePath.FullName);

        }
    }
}
#endif