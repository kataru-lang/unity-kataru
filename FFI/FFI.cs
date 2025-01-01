using System.Text;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        static extern FFIStr init_runner(byte[] story_path, UIntPtr story_length, byte[] bookmark_path, UIntPtr bookmark_length, bool validate);
        public static void InitRunner(string story_path, string bookmark_path, bool validate)
        {
            var story_bytes = Encoding.UTF8.GetBytes(story_path);
            var bookmark_bytes = Encoding.UTF8.GetBytes(bookmark_path);
            init_runner(
                story_bytes,
                (UIntPtr)story_bytes.Length,
                bookmark_bytes,
                (UIntPtr)bookmark_bytes.Length,
                validate
            ).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr save_story(byte[] path, UIntPtr length);
        public static void SaveStory(string path)
        {
            var bytes = Encoding.UTF8.GetBytes(path);
            save_story(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

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

        [DllImport("kataru_ffi")]
        static extern FFIStr get_params();
        static Dictionary<string, object> GetParams()
        {
            string json = get_params().ToString();
            Debug.Log($"Got json: {json}");
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        [DllImport("kataru_ffi")]
        static extern FFIStr get_state(byte[] var, UIntPtr var_len);
        public static T GetState<T>(string var)
        {
            var bytes = Encoding.UTF8.GetBytes(var);
            string json = get_state(bytes, (UIntPtr)bytes.Length).ToString();
            Debug.Log($"Got json: {json}");
            return JsonConvert.DeserializeObject<Dictionary<string, T>>(json)["value"];
        }

        #region Commands
        [DllImport("kataru_ffi")]
        static extern FFIStr get_command();
        public static Command GetCommand() => new Command() { name = get_command().ToString(), parameters = GetParams() };

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
        static extern FFIStr get_attributes();
        static AttributedSpan[] GetAttributes()
        {
            string json = get_attributes().ToString();
            return JsonConvert.DeserializeObject<AttributedSpan[]>(json);
        }

        public static Dialogue LoadDialogue() => new Dialogue()
        {
            name = GetSpeaker(),
            text = GetSpeech(),
            attributes = GetAttributes()
        };
        #endregion

        #region Codegen
        [DllImport("kataru_ffi")]
        static extern FFIStr codegen_consts(byte[] path, UIntPtr length);
        public static void CodegenConsts(string path)
        {
            var bytes = Encoding.UTF8.GetBytes(path);
            codegen_consts(bytes, (UIntPtr)bytes.Length).ThrowIfError();
        }

        [DllImport("kataru_ffi")]
        static extern bool codegen_was_updated();
        public static bool CodegenWasUpdated() => codegen_was_updated();
        #endregion
    }
}