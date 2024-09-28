using CommandSystem;
using MapForge.API.Models;
using MapForge.API;
using System;

namespace MapForge.Commands.MapForge
{
    public class RemovePrefabCommand : ICommand
    {
        public string Command { get; } = "removeprefab";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "Removes prefab";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Syntax: mapforge removeprefab (bundleName) (id)";
                return false;
            }

            string bundleName = arguments.At(0);
            
            if (!int.TryParse(arguments.At(1), out int prefabId))
            {
                response = $"Failed parsing prefab id from \"{arguments.At(1)}\"";
                return false;
            }

            if (!MapForgeAPI.TryGetBundle(bundleName, out BundleInfo bundle))
            {
                response = $"Bundle \"{bundleName}\" is not loaded!";
                return false;
            }

            if (!bundle.PrefabInstances.TryGetValue(prefabId, out PrefabInfo prefab))
            {
                response = $"Prefab with id {prefabId} is not spawned in bundle \"{bundleName}\"";
                return false;
            }

            prefab.Unload();
            response = $"Remove prefab from bundle \"{bundleName}\"";
            return true;
        }
    }
}