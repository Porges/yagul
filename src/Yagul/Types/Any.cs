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

    [PublicAPI]
    public class Unit
    {
        private static readonly Unit _it = new Unit();

        private Unit()
        {
        }

        public static Unit It
        {
            get { return _it; }
        }
    }
}
