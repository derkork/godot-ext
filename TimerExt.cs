using Godot;
using JetBrains.Annotations;

namespace GodotExt
{
    /// <summary>
    /// This class contains functions that revolve around timers and waiting for things.
    /// </summary>
    [PublicAPI]
    public static class TimerExt
    {
        /// <summary>
        /// Creates a one-off timer for the given amount of time that can be awaited.
        /// </summary>
        [MustUseReturnValue]
        public static SignalAwaiter Sleep(this Node source, float sleepTime)
        {
            GdAssert.That(source.IsInsideTree(), "source.IsInsideTree()");
            return source.GetTree().CreateTimer(sleepTime).Timeout();
        }

        /// <summary>
        /// Returns a <see cref="SignalAwaiter"/> that waits until this Timer
        /// runs into a timeout.
        /// </summary>
        public static SignalAwaiter Timeout(this SceneTreeTimer timer)
        {
            return timer.FiresSignal("timeout");
        }

        /// <summary>
        /// Returns a <see cref="SignalAwaiter"/> that waits until this Timer
        /// runs into a timeout.
        /// </summary>
        public static SignalAwaiter Timeout(this Timer timer)
        {
            return timer.FiresSignal("timeout");
        }

        /// <summary>
        /// Returns a <see cref="SignalAwaiter"/> that waits until the next frame.
        /// </summary>
        public static SignalAwaiter NextFrame(this SceneTree tree)
        {
            return tree.FiresSignal("idle_frame");
        }

        /// <summary>
        /// Returns a <see cref="SignalAwaiter"/> that waits until the next frame.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static SignalAwaiter NextFrame(this Node node)
        {
            return node.GetTree().NextFrame();
        }
    }
}