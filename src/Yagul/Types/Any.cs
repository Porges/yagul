using System;
using JetBrains.Annotations;

namespace Yagul.Types
{
    /// <summary>
    /// For use in APIs which can take an 'Any' type as a generic parameter.
    /// </summary>
    [PublicAPI]
    public class Any {
        private Any()
        {
        } 
    }

    /// <summary>
    /// A Unit type for C#. Void is not usable for many things.
    /// </summary>
    [PublicAPI]
    public struct Unit : IEquatable<Unit>
    {
        public bool Equals(Unit other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }
        
        public static bool operator ==(Unit left, Unit right)
        {
            return true;
        }

        public static bool operator !=(Unit left, Unit right)
        {
            return false;
        }

        public override string ToString()
        {
            return "()";
        }
    }
}
