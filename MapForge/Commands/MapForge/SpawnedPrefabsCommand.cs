using CommandSystem;
using MapForge.API.Models;
using MapForge.API;
using System;
using System.Text;

namespace MapForge.Commands.MapForge
{
    public class SpawnedPrefabsCommand : ICommand
    {
        public string Command { get; } = "spawnedprefabs";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "Spawned prefabs";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Syntax: mapforge spawnedprefabs (bundleName)";
                return false;
            }

            string bundleName = arguments.At(0);

            if (!MapForgeAPI.TryGetBundle(bundleName, out BundleInfo bundle))
            {
                response = $"Bundle \"{bundleName}\" is not loaded!";
                return false;
            }

            StringBuilder sb = new StringBuilder();
            
            if (bundle.PrefabInstances.Count == 0)
            {
                sb.AppendLine($"Theres no spawned prefabs for bundle \"{bundleName}\"");
            }
            else
            {
                sb.AppendLine($"Spawned prefabs in bundle \"{bundleName}\"");
                foreach (var prefab in bundle.PrefabInstances)
                {
                    sb.AppendLine($" [{prefab.Key}] {prefab.Value.PrefabName}");
                }
            }

            response = sb.ToString();
            return true;
        }
    }
}