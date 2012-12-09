using System;
using JetBrains.Annotations;

namespace Yagul.Extensions
{
    public static class EventExtensions
    {
        /// <summary>
        /// Raises an event handler, handling the null case correctly.
        /// </summary>
        [PublicAPI]
        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T parameter)
        {
            if (eventHandler != null)
                eventHandler(sender, parameter);
        }
    }
}
