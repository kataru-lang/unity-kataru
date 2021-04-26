using System.Text;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kataru
{
    /// <summary>
    /// Low level interface with Rust FFI.
    /// </summary>
    internal class FFI
    {
        #region Bookmark
        [DllImport("kataru_ffi")]
        static extern FFIStr load_bookmark(byte[] path, UIntPtr length);
        public static void LoadBookmark(string path)
        {
            var bytes = Encoding.UTF8.GetBytes(path);
            load_bookmark(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr save_bookmark(byte[] path, UIntPtr length);
        public static void SaveBookmark(string path)
        {
            var bytes = Encoding.UTF8.GetBytes(path);
            save_bookmark(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr load_snapshot(byte[] name, UIntPtr length);
        public static void LoadSnapshot(string name)
        {
            var bytes = Encoding.UTF8.GetBytes(name);
            load_snapshot(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr save_snapshot(byte[] name, UIntPtr length);
        public static void SaveSnapshot(string name)
        {
            var bytes = Encoding.UTF8.GetBytes(name);
            save_snapshot(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr set_state_string(byte[] key, UIntPtr length, byte[] value, UIntPtr value_length);
        public static void SetState(string key, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            var value_bytes = Encoding.UTF8.GetBytes(value);
            set_state_string(bytes, (UIntPtr)bytes.Length, value_bytes, (UIntPtr)value_bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr set_state_number(byte[] key, UIntPtr length, double value);
        public static void SetState(string key, double value)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            set_state_number(bytes, (UIntPtr)bytes.Length, value).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr set_state_bool(byte[] key, UIntPtr length, bool value);
        public static void SetState(string key, bool value)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            set_state_bool(bytes, (UIntPtr)bytes.Length, value).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr set_line(UIntPtr value);
        public static void SetLine(int line) => set_line((UIntPtr)line).ThrowIfError();

        [DllImport("kataru_ffi")]
        static extern FFIStr get_passage();
        public static string GetPassage() => get_passage().ToString();
        #endregion

        #region Story
        [DllImport("kataru_ffi")]
        static extern FFIStr load_story(byte[] path, UIntPtr length);
        public static void LoadStory(string path)
        {
            var bytes = Encoding.UTF8.GetBytes(path);
            load_story(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr save_story(byte[] path, UIntPtr length);
        public static void SaveStory(string path)
        {
            var bytes = Encoding.UTF8.GetBytes(path);
            save_story(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr init_runner();
        public static void InitRunner() =>
            init_runner().ThrowIfError();

        [DllImport("kataru_ffi")]
        static extern FFIStr validate();
        public static void Validate() =>
            validate().ThrowIfError();

        [DllImport("kataru_ffi")]
        static extern FFIStr next(byte[] input, UIntPtr length);
        public static void Next(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            next(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern LineTag tag();
        public static LineTag Tag() => tag();

        [DllImport("kataru_ffi")]
        static extern FFIStr goto_passage(byte[] passage, UIntPtr length);
        public static void GotoPassage(string passage)
        {
            var bytes = Encoding.UTF8.GetBytes(passage);
            goto_passage(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }
        #endregion

        #region Choices
        [DllImport("kataru_ffi")]
        static extern UIntPtr get_choices();
        static int GetChoices() => (int)get_choices();

        [DllImport("kataru_ffi")]
        static extern FFIStr get_choice(UIntPtr i);
        static string GetChoice(int i) => get_choice((UIntPtr)i).ToString();

        [DllImport("kataru_ffi")]
        static extern double get_timeout();
        static double GetTimeout() => get_timeout();

        public static Choices LoadChoices()
        {
            var choices = new List<string>();
            int numChoices = GetChoices();
            for (int i = 0; i < numChoices; ++i) choices.Add(GetChoice(i));
            return new Choices() { choices = choices, timeout = GetTimeout() };
        }
        #endregion

        #region Commands
        [DllImport("kataru_ffi")]
        static extern FFIStr get_command();
        static string GetCommand() => get_command().ToString();

        [DllImport("kataru_ffi")]
        static extern int get_params();
        static int GetParams() => (int)get_params();

        [DllImport("kataru_ffi")]
        static extern FFIStr get_param(UIntPtr i);
        static string GetParam(int i) => get_param((UIntPtr)i).ToString();

        [DllImport("kataru_ffi")]
        static extern ParamType get_param_type(UIntPtr i);
        [DllImport("kataru_ffi")]
        static extern bool get_param_bool(UIntPtr i);
        [DllImport("kataru_ffi")]
        static extern FFIStr get_param_string(UIntPtr i);
        [DllImport("kataru_ffi")]
        static extern double get_param_number(UIntPtr i);
        static object GetParamValue(int i)
        {
            UIntPtr u = (UIntPtr)i;
            ParamType paramType = get_param_type(u);
            switch (paramType)
            {
                case ParamType.Bool:
                    return get_param_bool(u);

                case ParamType.Number:
                    return get_param_number(u);

                case ParamType.String:
                    return get_param_string(u).ToString();

                default:
                    return null;
            }
        }

        public static Command LoadCommand()
        {
            var parameters = new Dictionary<string, object>();
            int numParameters = GetParams();
            for (int i = 0; i < numParameters; ++i)
            {
                parameters[GetParam(i)] = GetParamValue(i);
            }
            return new Command() { name = GetCommand(), parameters = parameters };
        }

        public static InputCommand LoadInputCommand()
        {
            return new InputCommand() { prompt = "Not implemented" };
        }
        #endregion

        #region Dialogue
        [DllImport("kataru_ffi")]
        static extern FFIStr get_speaker();
        static string GetSpeaker() => get_speaker().ToString();

        [DllImport("kataru_ffi")]
        static extern FFIStr get_speech();
        static string GetSpeech() => get_speech().ToString();


        [DllImport("kataru_ffi")]
        static extern UIntPtr get_attributes();
        static int GetAttributes() => (int)get_attributes();

        [DllImport("kataru_ffi")]
        static extern FFIStr get_attribute(UIntPtr i);
        static string GetAttribute(int i) => get_attribute((UIntPtr)i).ToString();

        [DllImport("kataru_ffi")]
        static extern FFIArray get_attribute_positions(UIntPtr i);
        static int[] GetAttributePositions(int i) => get_attribute_positions((UIntPtr)i).ToArray();

        public static Dialogue LoadDialogue() => new Dialogue()
        {
            name = GetSpeaker(),
            text = GetSpeech(),
            attributes = LoadAttributes()
        };

        public static IDictionary<string, Dialogue.Span[]> LoadAttributes()
        {
            var attributes = new Dictionary<string, Dialogue.Span[]>();
            int numAttributes = GetAttributes();
            for (int i = 0; i < numAttributes; ++i)
            {
                string attribute = GetAttribute(i);
                int[] positions = GetAttributePositions(i);
                Dialogue.Span[] spans = Dialogue.Span.FromArray(positions);
                attributes[attribute] = spans;
            }
            return attributes;
        }
        #endregion
    }
}