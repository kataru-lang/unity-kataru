/// Kataru Constants.
/// This file provides the public interface into the automatically generated
/// set of constants and enums for idenitifers in Kataru.
/// See `Constants.Generated.cs` for the generated code. Do not edit manually.
/// NOTE: Enum representations (Namespace, Character, Passage) are NOT safe to serialize, as they are not guaranteed to correspond the the same string.
/// When serializing (i.e. setting values in the editor) prefer using the string representation.
/// Only use enums when hardcoding values in source code.

using System.Linq;

namespace Kataru
{
    /// <summary>
    /// Basic utils for Kataru constants.
    /// </summary>
    public static class NamespaceUtils
    {
        public const string Global = "global";
        public const string None = "None";
        public const string NamespaceDelimiter = ":";

        /// <summary>
        /// Returns all identifiers that belong to a given namespace.
        /// </summary>
        public static string[] FilterByNamespace(string[] identifiers, string @namespace)
        {
            // Global namespaces don't have any "namespace:" prepended.
            System.Func<string, bool> predicate;
            if (@namespace == Global)
            {
                predicate = identifier => identifier == None || !identifier.Contains(NamespaceDelimiter);
            }
            else
            {
                predicate = identifier => identifier == None || identifier.StartsWith(@namespace + NamespaceDelimiter);
            }

            return identifiers.
                Where(predicate).
                ToArray<string>();
        }
    }
}