using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace Kataru
{

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class NamedAttribute : System.Attribute
    {
        public string Name { get; }
        public bool Prefixed { get; }
        public bool PrefixOnly { get; }
        public NamedAttribute(string name, bool prefixed = false, bool prefixOnly = false)
        {
            Name = name;
            Prefixed = prefixed;
            PrefixOnly = prefixOnly;
        }
    }

    /// <summary>
    /// Attribute for handling a command.
    /// If declared as local, the handler will have "{Name}." prefixed to the command, i.e "Alice.Wave".
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class CommandHandler : NamedAttribute
    {
        public CommandHandler(string name = "", bool local = false) : base(name, prefixed: local) { }
    }

    /// <summary>
    /// Attribute for handling a character's dialogue.
    /// If no name provided, will fall back to use the instance's Name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class CharacterHandler : NamedAttribute
    {
        public CharacterHandler(string name = "") : base(name, prefixOnly: name.Length == 0) { }
    }

    public static class ReflectionUtils
    {
        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
        {
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);
            return Action.CreateDelegate(Expression.GetActionType(types.ToArray()), target, methodInfo.Name);
        }
    }

    /// <summary>
    /// A MonoBehavior that can has KataruAttributes on its methods.
    /// Inherit from this class to be able to iterate over all methods containing named KataruAttributes.
    /// </summary>
    public class Attributed : MonoBehaviour
    {
        protected virtual string Name { get; }

        protected struct NamedDelegate
        {
            public string name;
            public Delegate @delegate;
        }

        /// <summary>
        /// Gets all actions and their qualified names for all attributed methods in this class.
        /// Since each instance may want a different command name (i.e. to support Alice.Wave),
        /// This method supports prefixing for any attributes with Prefixed = true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A"></typeparam>
        /// <returns></returns>
        protected IEnumerable<NamedDelegate> GetActionsForAttribute<A>() where A : NamedAttribute
        {
            var type = this.GetType();
            foreach (var methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                foreach (var attribute in methodInfo.GetCustomAttributes<A>(false))
                {
                    // [KataruCharacterHandler] -> prefixedOnly = true -> Name
                    // [KataruCharacterHandler("Character")] -> prefixedOnly = true -> attribute.Name
                    // [KataruCommandHandler] -> prefixed = false -> method.Name
                    // [KataruCommandHandler("Name")] -> prefixed = false -> attribute.Name

                    // If prefix is set to something other than empty string, the action name will be "{prefix}.{name}".
                    // Otherwise it will just be "name".

                    string name = "";
                    if (attribute.PrefixOnly)
                    {
                        name = Name;
                    }
                    else
                    {
                        string suffix = attribute.Name.Length > 0 ? attribute.Name : methodInfo.Name;
                        if (attribute.Prefixed)
                        {
                            name = $"{Name}.{suffix}";
                        }
                        else
                        {
                            name = suffix;
                        }
                    }
                    Debug.Log($"Name '{name}'");

                    yield return new NamedDelegate
                    {
                        name = name,
                        @delegate = methodInfo.CreateDelegate(this)
                    };
                }
            }
        }
    }
}
