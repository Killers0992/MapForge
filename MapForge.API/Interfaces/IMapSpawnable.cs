using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapForge.API
{
    public interface IMapSpawnable
    {
        SpawnableType Type { get; }
    }
}
