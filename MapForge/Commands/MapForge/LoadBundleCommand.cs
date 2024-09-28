using CommandSystem;
using MapForge.API.Models;
using MapForge.API;
using System;

namespace MapForge.Commands.MapForge
{
    public class LoadBundleCommand : ICommand
    {
        public string Command { get; } = "loadbundle";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "Load bundles";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Syntax: mapforge loadbundle (bundleName)";
                return false;
            }

            string bundleName = string.Join(" ", arguments);

            if (!MapForgeAPI.BundleExists(bundleName))
            {
                response = $"Bundle \"{bundleName}\" not exists in bundles directory";
                return false;
            }

            if (!MapForgeAPI.TryLoadBundle(bundleName, out BundleInfo bundle))
            {
                response = $"Bundle \"{bundleName}\" is invalid!";
                return false;
            }

            response = $"Bundle \"{bundleName}\" loaded with \"{bundle.Prefabs.Length}\" prefabs!";
            return true;
        }
    }
}
