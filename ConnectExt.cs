using System.Linq;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;
using Array = Godot.Collections.Array;

namespace GodotExt
{
    [PublicAPI]
    public static class ConnectExt
    {
        /// <summary>
        /// A more type-safe and user-friendly replacement for Godot's <see cref="Object.Connect"/>.
        /// Also makes it easier to add flags and binds without having to create
        /// Godot Arrays or fiddling with uints.
        /// </summary>
        [MustUseReturnValue]
        public static ConnectBinding Connect(this Object source, string signal)
        {
            return new ConnectBinding(source, signal);
        }

        [PublicAPI]
        public class ConnectBinding
        {
            private readonly Object _source;
            private readonly string _signal;
            private Array _binds;
            private uint _flags;

            public ConnectBinding(Object source, string signal)
            {
                _source = source;
                _signal = signal;
            }

            /// <summary>
            /// Adds additional parameters which will be given to the target method after the signal parameters.
            /// Note that calling this multiple times will overwrite the previous values, so all parameter values
            /// should be given in a single call.
            /// </summary>
            [MustUseReturnValue]
            public ConnectBinding WithBinds(params object[] binds)
            {
                _binds = new Array(binds);
                return this;
            }

            /// <summary>
            /// Adds connect flags to the signal binding. See <see cref="Object.ConnectFlags"/> for a list of
            /// available flags. Note that calling this multiple times will overwrite the previous values.
            /// </summary>
            [MustUseReturnValue]
            public ConnectBinding WithFlags(params Object.ConnectFlags[] flags)
            {
                _flags = flags.Aggregate(0U, (current, flag) => current | (uint) flag);
                return this;
            }

            /// <summary>
            /// Finishes the signal binding and returns a binding object which can be used to disconnect the signal later.
            /// </summary>
            public ConnectedBinding To(Object target, string method)
            {
                var result = _source.Connect(_signal, target, method, _binds, _flags);
                GdAssert.That(result == Error.Ok, $"{result} == Error.Ok");
                return new ConnectedBinding(_source, _signal, target, method);
            }
        }

        [PublicAPI]
        public readonly struct ConnectedBinding
        {
            private readonly Object _source;
            private readonly string _signal;
            private readonly Object _target;
            private readonly string _method;

            public ConnectedBinding(Object source, string signal, Object target, string method)
            {
                _source = source;
                _signal = signal;
                _target = target;
                _method = method;
            }

            /// <summary>
            /// Disconnects the signal binding.
            /// </summary>
            public void Disconnect()
            {
                _source.Disconnect(_signal, _target, _method);
            }
        }
    }
}