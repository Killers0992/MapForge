#if NWAPI
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Events;

namespace MapForge.Frameworks
{
    public class NwApiPlugin
    {
        /// <summary>
        /// Handler for plugin.
        /// </summary>
        public PluginHandler Handler;

        [PluginEntryPoint(BuildSettings.PluginName, BuildSettings.Version, "Plugin made for transforming SCP: SL map.", BuildSettings.Author)]
        void InitializeNWAPI()
        {            
            Handler = PluginHandler.Get(this);
            EventManager.RegisterEvents(this);

            PluginInitializer.Initialize(Handler.PluginDirectoryPath);
        }

        [PluginEvent]
        void OnWaitingForPlayers(WaitingForPlayersEvent ev) => PluginInitializer.InitializeObjects();
    }
}
#endif