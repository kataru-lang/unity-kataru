using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Kataru
{
    /// <summary>
    /// Enumerates the different types of Kataru lines.
    /// </summary>
    public enum LineTag
    {
        Choices,
        InvalidChoice,
        Dialogue,
        InputCommand,
        Command,
        End,
    }

    /// <summary>
    /// A span annotated with attributes.
    /// </summary>
    public struct AttributedSpan
    {
        /// <summary>
        /// Where in the stripped text the span begins.
        /// </summary>
        public int start;

        /// <summary>
        /// Where in the stripped text the span ends.
        /// </summary>
        public int end;

        /// <summary>
        /// Map of attribute parameter names to values.
        /// </summary>
        [JsonProperty("params")]
        public Dictionary<string, object> parameters;

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    /// <summary>
    /// Represents a single line of dialogue.
    /// </summary>
    public class Dialogue
    {
        /// <summary>
        /// Name of the dialogue speaker.
        /// </summary>
        public string name;
        /// <summary>
        /// Text (stripped of attributes) said in this line.
        /// </summary>
        public string text;
        /// <summary>
        /// List of attributed span annotations on this dialogue line.
        /// </summary>
        public AttributedSpan[] attributes;
    }

    /// <summary>
    /// Represents a command to be run in the game.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Name of the command.
        /// </summary>
        public string name;

        /// <summary>
        /// Parameters
        /// </summary>
        public Dictionary<string, object> parameters;

        /// <summary>
        /// Get the parameter, cast to type T.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return (T)parameters[key];
        }

        /// <summary>
        /// Construct the parameter array for dynamic call.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public object[] Params(MethodInfo methodInfo)
        {
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            object[] parameters = new object[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; ++i)
            {
                if (!this.parameters.TryGetValue(paramInfos[i].Name, out parameters[i]))
                {
                    throw new KeyNotFoundException($"Parameter '{paramInfos[i].Name}' was not provided.");
                }
            }

            return parameters;
        }
    }

    /// <summary>
    /// Represents the choices to be presented to the user for interacting
    /// with the Kataru story.
    /// </summary>
    public class Choices
    {
        /// <summary>
        /// List of choices.
        /// </summary>
        public List<string> choices;
        /// <summary>
        /// How long the user has to make the choice in seconds.
        /// </summary>
        public double timeout;
    }

    /// <summary>
    /// Represents an input prompt for the user to enter a string.
    /// </summary>
    public class InputCommand
    {
        /// <summary>
        /// Prompt text to display to the user.
        /// </summary>
        public string prompt;
    }
}