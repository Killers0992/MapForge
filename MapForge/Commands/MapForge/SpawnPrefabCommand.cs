using CommandSystem;
using MapForge.API.Models;
using MapForge.API;
using System;
using RemoteAdmin;
using UnityEngine;

namespace MapForge.Commands.MapForge
{
    public class SpawnPrefabCommand : ICommand
    {
        public string Command { get; } = "spawnprefab";

        public string[] Aliases { get; } = new string[] { "sp" };

        public string Description { get; } = "Spawns prefab";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "This command can be only executed ingame!";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Syntax: mapforge spawnprefab (bundleName) (prefabName)";
                return false;
            }

            string bundleName = arguments.At(0);
            string prefabName = arguments.At(1);

            if (!MapForgeAPI.TryGetBundle(bundleName, out BundleInfo bundle))
            {
                response = $"Bundle \"{bundleName}\" is not loaded!";
                return false;
            }

            if (!bundle.ContainsPrefab(prefabName))
            {
                response = $"Prefab \"{prefabName}\" not exists in bundle \"{bundleName}\"";
                return false;
            }

            PrefabInfo prefab = bundle.CreatePrefab(prefabName);
            prefab.Spawn(playerSender.ReferenceHub.transform.position, Vector3.zero, Vector3.one);
            response = $"Spawned prefab [{prefab.Id}] {prefabName}";
            return true;
        }
    }
}
