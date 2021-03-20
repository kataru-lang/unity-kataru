using System.Collections.Generic;
using System.Reflection;

namespace Kataru
{
    public enum LineTag
    {
        Choices,
        InvalidChoice,
        Dialogue,
        InputCommand,
        Commands,
        None,
    }

    public enum ParamType
    {
        None,
        String,
        Number,
        Bool,
    }

    public class Dialogue
    {
        public string name;
        public string text;
        public IDictionary<string, Span[]> attributes;

        public struct Span
        {
            int start;
            int stop;

            public static Span[] FromArray(int[] positions)
            {
                int length = positions.Length / 2;
                Span[] spans = new Span[length];
                for (int i = 0; i < length; ++i)
                {
                    spans[i].start = positions[2 * i];
                    spans[i].stop = positions[2 * i + 1];
                }
                return spans;
            }

            public override string ToString() => $"({start}, {stop})";
        }
    }

    public class Command
    {
        public string name;
        public Dictionary<string, object> parameters;

        public T Get<T>(string key)
        {
            return (T)parameters[key];
        }

        public object[] Params(MethodInfo methodInfo)
        {
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            object[] @params = new object[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; ++i)
            {
                if (!parameters.TryGetValue(paramInfos[i].Name, out @params[i]))
                {
                    throw new KeyNotFoundException($"Parameter '{paramInfos[i].Name}' was not provided.");
                }
            }

            return @params;
        }
    }

    public class Choices
    {
        public List<string> choices;
        public double timeout;
    }

    public class InputCommand
    {
        public string prompt;
    }

}