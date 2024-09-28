using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public virtual void OnSpawnObject(SpawnableInfo info)
        {

        }
    }
}
