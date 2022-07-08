using System.Collections.Generic;
using System.Linq;

using Godot;

using JetBrains.Annotations;

namespace GodotExt
{
    /// <summary>
    /// Contains extension methods for <see cref="Directory"/>.
    /// </summary>
    [PublicAPI]
    public static class DirectoryExt
    {
        /// <summary>
        /// Returns the complete file paths of all files inside <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory">The <see cref="Directory"/> to search in.</param>
        /// <param name="recursive">Whether the search should be conducted recursively (return paths of files inside <paramref name="directory"/>'s subdirectories and so on) or not.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the paths of all files inside <paramref name="directory"/>.</returns>
        [MustUseReturnValue]
        public static IEnumerable<string> GetFiles(this Directory directory, bool recursive = false)
        {
            return recursive
                ? directory.GetDirectories(true)
                    .SelectMany(path =>
                    {
                        Directory recursiveDirectory = new Directory();
                        recursiveDirectory.Open(path);
                        return recursiveDirectory.GetElementsNonRecursive(true);
                    })
                    .Concat(directory.GetElementsNonRecursive(true))
                : directory.GetElementsNonRecursive(true);
        }
        
        /// <summary>
        /// Returns the complete file paths of all files inside <paramref name="directory"/> whose extensions match any of <paramref name="fileExtensions"/>.
        /// </summary>
        /// <param name="directory">The <see cref="Directory"/> to search in.</param>
        /// <param name="recursive">Whether the search should be conducted recursively (return paths of files inside <paramref name="directory"/>'s subdirectories and so on) or not.</param>
        /// <param name="fileExtensions">The file extensions to search for. If none are provided, all file paths are returned.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the paths of all files inside <paramref name="directory"/> whose extensions match any of <paramref name="fileExtensions"/>.</returns>
        [MustUseReturnValue]
        public static IEnumerable<string> GetFiles(this Directory directory, bool recursive = false, params string[] fileExtensions)
        {
            return fileExtensions.Any()
                ? directory.GetFiles(recursive)
                    .Where(file => fileExtensions.Any(file.EndsWith))
                : directory.GetFiles(recursive);
        }

        /// <summary>
        /// Returns the complete directory paths of all directories inside <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory">The <see cref="Directory"/> to search in.</param>
        /// <param name="recursive">Whether the search should be conducted recursively (return paths of directories inside <paramref name="directory"/>'s subdirectories and so on) or not.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the paths of all files inside <paramref name="directory"/>.</returns>
        [MustUseReturnValue]
        public static IEnumerable<string> GetDirectories(this Directory directory, bool recursive = false)
        {
            return recursive
                ? directory.GetElementsNonRecursive(false)
                    .SelectMany(path =>
                    {
                        Directory recursiveDirectory = new Directory();
                        recursiveDirectory.Open(path);
                        return recursiveDirectory.GetDirectories(true).Prepend(path);
                    })
                : directory.GetElementsNonRecursive(false);
        }
        
        [MustUseReturnValue]
        private static IEnumerable<string> GetElementsNonRecursive(this Directory directory, bool trueIfFiles)
        {
            directory.ListDirBegin(true);
            while (true)
            {
                string next = directory.GetNext();
                if (next is "")
                {
                    yield break;
                }
                if (directory.CurrentIsDir() == trueIfFiles)
                {
                    continue;
                }
                string current = directory.GetCurrentDir();
                yield return current.EndsWith("/") ? $"{current}{next}" : $"{current}/{next}";
            }
        }
    }
}