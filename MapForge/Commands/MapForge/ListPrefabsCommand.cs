using System;
using System.Text;
using CommandSystem;
using MapForge.API;
using MapForge.API.Models;

namespace MapForge.Commands.MapForge
{
    public class ListPrefabsCommand : ICommand
    {
        public string Command { get; } = "listprefabs";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "Gets prefabs which can be spawned in bundle";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Syntax: mapforge listprefabs (bundleName)";
                return false;
            }

            string bundleName = string.Join(" ", arguments);

            if (!MapForgeAPI.TryGetBundle(bundleName, out BundleInfo bundle))
            {
                response = $"Bundle \"{bundleName}\" is not loaded, use mapforge loadbundle {bundleName} !";
                return false;
            }

            StringBuilder sb = new StringBuilder();

            if (bundle.Prefabs.Length == 0)
            {
                sb.AppendLine($"Theres no prefabs in bundle \"{bundleName}\"");
            }
            else
            {
                sb.AppendLine($"Available prefabs in bundle \"{bundleName}\"");

                foreach (var prefab in bundle.Prefabs)
                {
                    sb.AppendLine($" - {prefab}");
                }
            }

            response = sb.ToString();
            return true;
        }
    }
}
