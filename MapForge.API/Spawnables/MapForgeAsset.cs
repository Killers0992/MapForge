using MapForge.API.Enums;
using MapForge.API.Models;

namespace MapForge.API.Spawnables
{
    public class MapForgeAsset : SpawnableInfo
    {
        public override SpawnableType Type { get; } = SpawnableType.Asset;

        public SpawnableAssetType AssetType;
    }
}
