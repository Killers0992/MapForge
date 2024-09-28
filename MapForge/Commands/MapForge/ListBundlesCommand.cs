using CommandSystem;
using MapForge.API;
using System;
using System.Text;

namespace MapForge.Commands.MapForge
{
    public class ListBundlesCommand : ICommand
    {
        public string Command { get; } = "listbundles";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "List bundles";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] bundleNames = MapForgeAPI.GetAvailableBundles();

            StringBuilder sb = new StringBuilder();
            
            if (bundleNames.Length == 0)
            {
                sb.AppendLine("Theres no bundles in bundle directory!");
            }
            else 
            {
                sb.AppendLine($"Bundles:");

                for (int x = 0; x < bundleNames.Length; x++)
                {
                    sb.Append($" - \"{bundleNames[x]}\" - Status {(MapForgeAPI.IsBundleLoaded(bundleNames[x]) ? "loaded" : "not loaded")}");
                }
            }

            response = sb.ToString();
            return true;
        }
    }
}
