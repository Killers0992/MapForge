using System;

namespace MapForge.API.Enums
{
    [Flags]
    public enum PrimitiveExtraFlags : byte
    {
        None = 0,
        Collidable = 1,
        Visible = 2,
    }
}
