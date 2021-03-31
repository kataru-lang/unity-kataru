using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Collections.Concurrent;

namespace Kataru
{
    /// <summary>
    /// The Kataru Runner serves as the high level interface with the Kataru Rust FFI module.
    /// </summary>
    public static class Runner
    {
        static string bookmarkPath;
        static string savePath;
        static string storyPath;

        // Events to listen to.
        public static event Action<Choices> OnChoices;
        public static event Action OnInvalidChoice;
        public static event Action OnDialogueEnd;
        public static event Action<InputCommand> OnInputCommand;

        public static DelegateMap CommandDelegates = new DelegateMap();
        public static DelegateMap CharacterDelegates = new DelegateMap();

        private static LineTag Tag = LineTag.None;

        /// <summary>
        /// Initialize the story, bookmark and internal runner.
        /// This method should only be called once.
        /// </summary>
        public static void Init(string storyPath, string bookmarkPath, string savePath)
        {
#if UNITY_EDITOR
            Debug.Log(
                $@"Kataru.Init(StoryPath: '{storyPath}', 
                StoryPath: '{bookmarkPath}', 
                SavePath: '{savePath}')");
#endif
            Runner.storyPath = storyPath;
            Runner.bookmarkPath = bookmarkPath;
            Runner.savePath = savePath;

            FFI.LoadStory(storyPath);
            FFI.LoadBookmark(bookmarkPath);
            FFI.Validate();
            FFI.InitRunner();
        }

        /// <summary>
        /// Save the bookmark to path.
        /// </summary>
        public static void Save()
        {
#if UNITY_EDITOR
            Debug.Log($"Kataru.Save('{savePath}')");
#endif
            FFI.SaveBookmark(savePath);
        }

        /// <summary>
        /// Returns true if a save file exists.
        /// </summary>
        public static bool SaveExists() => File.Exists(savePath);

        /// <summary>
        /// Deletes the save file.
        /// </summary>
        public static void DeleteSave() => File.Delete(savePath);

        /// <summary>
        /// /// Load bookmark from the save path path.
        /// </summary>
        public static void Load()
        {
            FFI.LoadBookmark(savePath);
            FFI.InitRunner();
        }

        public static void SaveSnapshot(string name) => FFI.SaveSnapshot(name);
        public static void LoadSnapshot(string name) => FFI.LoadSnapshot(name);
        public static void SetLine(int line) => FFI.SetLine(line);
        public static void GotoPassage(string passage) => FFI.GotoPassage(passage);
        public static void SetState(string key, string value) => FFI.SetState(key, value);
        public static void SetState(string key, double value) => FFI.SetState(key, value);
        public static void SetState(string key, bool value) => FFI.SetState(key, value);
        public static string GetPassage() => FFI.GetPassage();

        /// <summary>
        /// Goto a passage and run the first line.
        /// </summary>
        /// <param name="passage"></param>
        public static void RunPassage(string passage)
        {
            GotoPassage(passage);
            Next();
        }

        /// <summary>
        /// Goto a passage and run until choices are required.
        /// </summary>
        /// <param name="passage"></param>
        public static void RunPassageUntilChoice(string passage)
        {
            GotoPassage(passage);
            Next();

            while (Tag == LineTag.Dialogue || Tag == LineTag.Commands)
            {
                Next();
            }
        }

        /// <summary>
        /// Progress the story using the given input.
        /// This yields line data from internal dialogue runner, whose data is passed via invoking actions.
        /// </summary>
        /// <param name="input"></param>
        public static void Next(string input = "")
        {
#if UNITY_EDITOR
            Debug.Log($"Kataru.Next('" + input + "')");
#endif
            FFI.Next(input);
            Tag = FFI.Tag();
            Debug.Log($"Tag: {Tag}");
            switch (Tag)
            {
                case LineTag.Choices:
                    OnChoices.Invoke(FFI.LoadChoices());
                    break;

                case LineTag.InvalidChoice:
                    Debug.LogError("Invalid choice");
                    OnInvalidChoice.Invoke();
                    break;

                case LineTag.Dialogue:
                    Dialogue dialogue = FFI.LoadDialogue();
                    Debug.Log($"{dialogue.name}: '{dialogue.text}'");
                    CharacterDelegates.Invoke(dialogue.name, new object[] { dialogue });
                    break;

                case LineTag.Commands:
                    Command command = FFI.LoadCommand();
                    Debug.Log($"Calling command {command.name}");
                    ConcurrentDictionary<Delegate, byte> delegates;
                    if (CommandDelegates.TryGetValue(command.name, out delegates))
                    {
                        object[] @params = new object[] { };
                        foreach (var @delegate in delegates.Keys)
                        {
                            @params = command.Params(@delegate.Method);
                            break;
                        }
                        CommandDelegates.Invoke(command.name, @params);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"No Kataru command named '{command.name}'");
                    }
                    break;

                case LineTag.InputCommand:
                    OnInputCommand.Invoke(FFI.LoadInputCommand());
                    break;

                case LineTag.None:
                    OnDialogueEnd.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Calls next after waiting a bit.
        /// Must be called using GameObject.StartCoroutine.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator DelayedNext(float seconds, string input = "")
        {
            yield return new WaitForSeconds(seconds);
            Next(input);
        }
    }
}