using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace GodotExt
{

    [PublicAPI]
    public static class NodeExt
    {
         /// <summary>
        /// Adds a child to the parent but keeps its global position on
        /// screen. If the child is currently attached to another parent,
        /// it will be detached first.
        /// </summary>
        /// <param name="parent">the parent to add the child to.</param>
        /// <param name="child">the child to add.</param>
        public static void MoveToNewParentKeepPosition(this Node2D child, Node parent)
        {
            var globalPosition = child.GlobalPosition;
            var globalRotation = child.GlobalRotation;

            child.MoveToNewParent(parent);

            child.GlobalPosition = globalPosition;
            child.GlobalRotation = globalRotation;
        }

        /// <summary>
        /// Moves a node to a new parent. Detaches it from the current
        /// parent if it has any. 
        /// </summary>
        public static void MoveToNewParent(this Node node, Node newParent)
        {
            node.RemoveFromParent();
            newParent.AddChild(node);
        }

        /// <summary>
        /// Removes a node from its parent.
        /// </summary>
        public static void RemoveFromParent(this Node node)
        {
            if (node.GetParent() != null)
            {
                node.GetParent().RemoveChild(node);
            }
        }

        /// <summary>
        /// Finds a parent up the tree that is of the given type. If none
        /// is found, returns null.
        /// </summary>
        [CanBeNull]
        public static T FindParent<T>(this Node node) where T : Node
        {
            var parent = node.GetParent();
            while (parent != null && !(parent is T))
            {
                parent = parent.GetParent();
            }

            return (T) parent;
        }

        /// <summary>
        /// Finds the first descendant in each branch of the sub-tree which matches the given predicate.
        /// If a node matches the predicate its subtree will not be searched anymore. 
        /// </summary>
        public static IEnumerable<T> FindClosestDescendants<T>(this Node root, Predicate<T> predicate = null)
        {
            if (root is T t && (predicate == null || predicate(t)))
            {
                yield return t;
                yield break;
            }
            
            foreach (var child in root.GetChildNodes())
            {
                foreach (var match in child.FindClosestDescendants(predicate))
                {
                    yield return match;
                }
            }
        }

        /// <summary>
        /// Finds the all descendants in each branch of the sub-tree which matches the given predicate.
        /// If a node matches the predicate its subtree will still be searched for matching nodes.
        /// </summary>
        public static IEnumerable<T> FindAllDescendants<T>(this Node root, Predicate<T> predicate = null) 
        {
            if (root is T t && (predicate == null || predicate(t)))
            {
                yield return t;
            }
            
            // it may have been deleted, so we check if the instance is still valid.
            if (!Object.IsInstanceValid(root))
            {
                yield break;
            }
            
            foreach (var child in root.GetChildNodes())
            {
                foreach (var match in child.FindAllDescendants(predicate))
                {
                    yield return match;
                }
            }
        }
        
        /// <summary>
        /// Gets a node with the given path below the root. Same as get_node but
        /// verifies that the node is not null using an assert.
        /// </summary>
        /// <param name="node">the node from which to look</param>
        /// <param name="path">the path</param>
        /// <typeparam name="T">the expected return type</typeparam>
        /// <returns></returns>
        public static T AtPath<T>(this Node node, string path) where T : Node
        {
            var result = node.AtPathOrNull<T>(path);
            GdAssert.That(result != null, $"Referenced node at path {path} not found.");
            return result;
        }


        /// <summary>
        /// Same as at_path but doesn't verify that the node exists.
        /// This is technically redundant, as Node.GetNodeOrNull provides the
        /// same functionality but is included in here for API consistency.
        /// </summary>
        public static T AtPathOrNull<T>(this Node node, string path) where T : Node
        {
            return node.GetNodeOrNull<T>(path);
        }


        /// <summary>
        /// Finds a node in hierarchy using the given name and returns it.
        /// Verifies that the node actually exists. This is useful to initialize
        /// ready variables with nodes in deeper hierarchies that may change
        /// so you don't need to update the paths all the time.
        /// </summary>
        public static T WithName<T>(this Node node, string name) where T : Node
        {
            var result = node.WithNameOrNull<T>(name);
            GdAssert.That(result != null, $"Referenced node with name {name} of type {typeof(T).Name} not found.");
            return result;
        }


        /// <summary>
        /// Same as WithName but does not assert the node to be not null.
        /// </summary>
        public static T WithNameOrNull<T>(this Node node, string name) where T : Node
        {
            var result = node.FindNode(name, true, false);
            if (result == null)
            {
                return null;
            }

            GdAssert.That(result is T, $"Node {node.Name} is {typeof(T).Name} (was {result.GetType().Name})");
            return result as T;
        }

        /// <summary>
        /// Removes a node from its parent and queues it for deletion.
        /// </summary>
        /// <param name="node"></param>
        public static void RemoveAndFree(this Node node)
        {
            node.RemoveFromParent();
            node.QueueFree();
        }

        /// <summary>
        /// Finds all nodes below the given node that are in the given group.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindBelowInGroup<T>(this Node parent, string group) where T : Node
        {
            return parent.GetTree()
                .GetNodesInGroup<T>(group)
                .Where(parent.IsAParentOf);
        }

        /// <summary>
        /// Returns all nodes in the given group that are of the given type.
        /// </summary>
        public static IEnumerable<T> GetNodesInGroup<T>(this SceneTree parent, string group)
        {
            return parent
                .GetNodesInGroup(group)
                .OfType<T>();
        }

        /// <summary>
        /// Enumerator over the child nodes of a node which better plays with
        /// LINQ.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<Node> GetChildNodes(this Node parent)
        {
            // we get the array here, because children may be modified
            // while this is running. Otherwise we could just call
            // GetChild(index) here.
            var children = parent.GetChildren();
            foreach (var child in children)
            {
                yield return (Node) child;
            }
        }

        /// <summary>
        /// Gets all child nodes that are of a certain type.
        /// </summary>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetChildNodes<T>(this Node parent)
        {
            return parent.GetChildNodes().OfType<T>();
        }

        /// <summary>
        /// Returns the amount of children of this node which are of the given type.
        /// </summary>
        public static int GetChildCount<T>(this Node parent)
        {
            return parent.GetChildNodes<T>().Count();
        }

        /// <summary>
        /// A type-safe version of <see cref="Node.Duplicate"/>.
        /// </summary>
        public static T Clone<T>(this T self, int flags = 15) where T : Node
        {
            return (T) self.Duplicate(flags);
        }

        /// <summary>
        /// Returns a <see cref="SignalAwaiter"/> that waits until this object
        /// fires a certain signal.
        /// </summary>
        public static SignalAwaiter FiresSignal(this Object source, string signal)
        {
            return source.ToSignal(source, signal);
        }

    }
}