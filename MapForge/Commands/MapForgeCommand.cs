using CommandSystem;
using MapForge.Commands.MapForge;
using System;
using System.Text;

namespace MapForge.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MapForgeCommand : ParentCommand
    {
        public override string Command { get; } = "mapforge";

        public override string[] Aliases { get; } = new string[] { "mf" };

        public override string Description { get; } = "Main command for MapForge plugin.";

        public MapForgeCommand() => this.LoadGeneratedCommands();

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ListBundlesCommand());
            RegisterCommand(new LoadBundleCommand());
            RegisterCommand(new ListPrefabsCommand());
            RegisterCommand(new SpawnPrefabCommand());
            RegisterCommand(new SpawnedPrefabsCommand());
            RegisterCommand(new RemovePrefabCommand());
            RegisterCommand(new RemoveAllCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Avilable commands");
            sb.AppendLine($" - mapforge listBundles - Gets all bundles which are loaded or can be loaded.");
            sb.AppendLine($" - mapforge loadBundle (bundleName) - Loads bundle.");
            sb.AppendLine($" - mapforge listPrefabs (bundleName) - Gets prefabs which can be spawned in bundle.");
            sb.AppendLine($" - mapforge spawnPrefab (bundleName) (prefabName) - Spawns prefab on current position of player.");
            sb.AppendLine($" - mapforge spawnedPrefabs (bundleName) - Gets spawned prefabs for specific bundle.");
            sb.AppendLine($" - mapforge removePrefab (bundleName) (id) - Removes spawned prefab by id.");
            sb.AppendLine($" - mapforge removeAll - Deletes all spawned prefabs.");
            response = sb.ToString();
            return true;
        }
    }
}
