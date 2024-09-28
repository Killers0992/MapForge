using CommandSystem;
using MapForge.API.Models;
using MapForge.API;
using System;
using System.Linq;

namespace MapForge.Commands.MapForge
{
    public class RemoveAllCommand : ICommand
    {
        public string Command { get; } = "removeall";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "Removes all spawned prefabs";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            int removedPrefabs = 0;
            foreach (var bundle in MapForgeAPI.LoadedBundles.Values.ToArray())
            {
                foreach (PrefabInfo prefab in bundle.PrefabInstances.Values.ToArray())
                {
                    prefab.Unload();
                    removedPrefabs++;
                }
            }

            response = removedPrefabs == 0 ? "Theres no prefabs to remove" : $"Removed {removedPrefabs} spawned prefabs";
            return true;
        }
    }
}
