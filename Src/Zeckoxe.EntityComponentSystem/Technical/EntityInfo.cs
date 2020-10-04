using System;
using System.Collections.Generic;

namespace Zeckoxe.EntityComponentSystem.Technical
{
    internal struct EntityInfo
    {
        #region Fields

        public short Version;
        public ComponentEnum Components;
        public HashSet<int> Children;
        public Func<int, bool> Parents;

        public readonly bool IsAlive(short version) => Version == version && Components[World.IsAliveFlag];

        #endregion
    }
}
