using UnityEngine;

namespace MapForge.API.Models
{
    public class ObjectsInfo
    {
        public bool IsInitialized { get; private set; }

        public virtual void Initialize() 
        {
            if (IsInitialized)
                return;

            IsInitialized = true;
        }

        public virtual void SpawnObject(GameObject go, int dimensionId)
        {

        }

        public virtual GameObject OnSpawnObject(SpawnableInfo info)
        {
            return null;
        }
    }
}
