#if EXILED
using Exiled.API.Interfaces;
#endif

namespace MapForge
{
    public class PluginConfig
#if EXILED
        : IConfig
#endif
    {
#if EXILED
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
#endif
    }
}
