using System.Threading.Tasks;
using Godot;
using JetBrains.Annotations;

namespace GodotExt
{
    /// <summary>
    /// This class contains useful extension methods for <see cref="Tween"/>
    /// </summary>
    [PublicAPI]
    public static class TweenExt
    {
        /// <summary>
        /// Returns a <see cref="SignalAwaiter"/> that waits until this Tween is
        /// completed.
        /// </summary>
        public static SignalAwaiter IsCompleted(this Tween tween)
        {
            return tween.FiresSignal("tween_completed");
        }

        /// <summary>
        /// Returns a <see cref="SignalAwaiter"/> that waits until all interpolations
        /// of the tween are completed.
        /// </summary>
        public static SignalAwaiter AreAllCompleted(this Tween tween)
        {
            return tween.FiresSignal("tween_all_completed");
        }

        /// <summary>
        /// Waits until the Tween has finished an interpolation of the given object.
        /// </summary>
        public static async Task IsCompleted(this Tween tween, Object interpolatedObject)
        {
            object[] result;
            do
            {
                result = await tween.IsCompleted();
            } while (!(result.Length == 2 && result[0] == interpolatedObject));
        }
    }
}