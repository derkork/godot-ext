using System.Text;
using JetBrains.Annotations;

namespace GodotExt
{
    [PublicAPI]
    public static class StringExt
    {
        /// <summary>
        /// Converts a string from PascalCase to snake_case. This is useful if you need to call internal Godot
        /// which are encoded in snake case.
        /// </summary>
        public static string ToSnakeCase(this string text)
        {
            if (text == null)
            {
                return null;
            }

            if (text.Length < 2)
            {
                return text;
            }

            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(text[0]));
            for (var i = 1; i < text.Length; ++i)
            {
                var c = text[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}