﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class Runner
    {
        [SerializeField] private static string storyPath;
        [SerializeField] private static string targetPath;
        [SerializeField] private static string bookmarkPath;
        [SerializeField] private static string savePath;
        [SerializeField] private static string codegenPath;

        // Events to listen to.
        public static event Action<LineTag> OnLine;
        public static event Action<Choices> OnChoices;
        public static event Action OnInvalidChoice;
        public static event Action OnDialogueEnd;
        public static event Action<InputCommand> OnInputCommand;

        public static DelegateMap CommandDelegates = new DelegateMap();
        public static DelegateMap CharacterDelegates = new DelegateMap();

        public static LineTag Tag = LineTag.End;
        public static bool isRunning { get; private set; }
        public static bool isInitialized { get; private set; }
        private static bool isWaiting = false;


        /// <summary>
        /// Initialize the story, bookmark and internal runner.
        /// This method should only be called once.
        /// </summary>
        public static void Init()
        {
            isInitialized = false;
            var settings = KataruSettings.Get(createIfMissing: true);
            targetPath = Application.streamingAssetsPath + "/" + settings.targetPath;
            bookmarkPath = Application.streamingAssetsPath + "/" + settings.bookmarkPath;
            savePath = Application.persistentDataPath + "/" + settings.savePath;
            codegenPath = settings.codegenPath;

            Debug.Log(
                    $@"Kataru.Init(StoryPath: '{targetPath}', 
                BookmarkPath: '{bookmarkPath}', 
                SavePath: '{savePath}')");

#if UNITY_EDITOR
            if (!File.Exists(targetPath))
            {
                Debug.LogWarning("Missing target. Retriggering compilation...");
                storyPath = Application.dataPath + "/" + settings.storyPath;

                Compile(storyPath, bookmarkPath, targetPath, codegenPath);
            }
#endif

            isRunning = false;
            isWaiting = false;

            // Only load the story on Init.
            FFI.InitRunner(targetPath, bookmarkPath, validate: true);

            isInitialized = true;
        }

        /// <summary>
        /// Save the bookmark to save path.
        /// </summary>
        public static void Save()
        {
            if (!isInitialized)
            {
                Debug.LogError($"Have not initialized. Call Runner.Init() before calling this.");
                return;
            }

            var parent = Directory.GetParent(savePath);
            if (!parent.Exists)
            {
                parent.Create();
            }
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
        public static void DeleteSave()
        {
            if (SaveExists())
            {
#if UNITY_EDITOR
                Debug.Log($"Kataru.DeleteSave() at '{savePath}'");
#endif
                File.Delete(savePath);
            }
        }

        /// <summary>
        /// Load bookmark from the save path path.
        /// Call this when wanting to load save file.
        /// </summary>
        public static void Load()
        {
            if (!isInitialized)
            {
                Debug.LogError($"Have not initialized. Call Runner.Init() before calling this.");
                return;
            }

            if (SaveExists())
            {
                FFI.LoadBookmark(savePath);
            }
            else
            {
                Debug.Log($"Loading bookmark {bookmarkPath}");
                FFI.LoadBookmark(bookmarkPath);
            }
        }

        public static void SaveSnapshot(string name) => FFI.SaveSnapshot(name);
        public static void LoadSnapshot(string name) => FFI.LoadSnapshot(name);
        public static void SetLine(int line) => FFI.SetLine(line);
        public static int GetLine() => FFI.GetLine();
        public static void GotoPassage(string passage) => FFI.GotoPassage(passage);
        public static void SetState(string key, string value) => FFI.SetState(key, value);
        public static void SetState(string key, double value) => FFI.SetState(key, value);
        public static void SetState(string key, bool value) => FFI.SetState(key, value);
        public static T GetState<T>(string key) => FFI.GetState<T>(key);
        public static string GetNamespace() => FFI.GetNamespace();
        public static string GetPassage() => FFI.GetPassage();

        public static void RunSnapshot(string snapshot)
        {
            isRunning = true;
            LoadSnapshot(snapshot);
            SetLine(Mathf.Max(0, GetLine() - 1));
            Next();
        }

        public static void RunPassageAtLine(string passage, int line)
        {
            isRunning = true;
            GotoPassage(passage);
            SetLine(line);
            Next();
        }

        /// <summary>
        /// Goto a passage and run the first line.
        /// </summary>
        /// <param name="passage"></param>
        public static void RunPassage(string passage)
        {
            isRunning = true;
            GotoPassage(passage);
            Next();
        }

        /// <summary>
        /// Goto a passage and run until choices are required.
        /// Returns true if choices encountered; returns false if choices are not.
        /// </summary>
        /// <param name="passage"></param>
        public static bool RunPassageUntilChoice(string passage)
        {
            RunPassage(passage);

            while (Tag == LineTag.Dialogue || Tag == LineTag.Command)
            {
                Next();
            }

            Exit();

            return Tag == LineTag.Choices;
        }

        /// <summary>
        /// Progress the story using the given input.
        /// This yields line data from internal dialogue runner, whose data is passed via invoking actions.
        /// If delayed next was called previously, this runner is current waiting for that to finish and won't run the next line.
        /// </summary>
        /// <param name="input"></param>
        public static LineTag Next(string input = "", bool auto = false)
        {
#if UNITY_EDITOR
            string caller = (new System.Diagnostics.StackTrace()).GetFrame(auto ? 2 : 1).GetMethod().Name;
            Debug.Log($"Kataru.Runner.Next('{input}') from {caller}.");
#endif
            if (isWaiting)
            {
#if UNITY_EDITOR
                Debug.LogWarning($@"Called Runner.Next while runner was busy waiting.
                                    Don't call Runner.Next until Runner.DelayedNext has finished.");
#endif
                return LineTag.End;
            }

            FFI.Next(input);
            Tag = FFI.Tag();
#if UNITY_EDITOR
            Debug.Log($"Tag: {Tag}");
#endif
            switch (Tag)
            {
                case LineTag.Choices:
                    OnChoices.Invoke(FFI.LoadChoices());
                    break;

                case LineTag.InvalidChoice:
                    OnInvalidChoice.Invoke();
                    break;

                case LineTag.Dialogue:
                    Dialogue dialogue = FFI.LoadDialogue();
#if UNITY_EDITOR
                    Debug.Log($"{dialogue.name}: '{dialogue.text}'");
#endif
                    CharacterDelegates.Invoke(dialogue.name, new object[] { dialogue });
                    break;

                case LineTag.Command:
                    Command command = FFI.GetCommand();
#if UNITY_EDITOR
                    Debug.Log($"Calling command '{command.name}'");
                    if (string.IsNullOrEmpty(command.name))
                    {
                        throw new KeyNotFoundException($"Received empty Kataru command. Did you use a global command as a character command?");
                    }
#endif
                    ConcurrentDictionary<Delegate, bool> delegates;
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
                        throw new KeyNotFoundException($"No Kataru command named '{command.name}' was registered in Unity. Are you missing a command definition, attribute, or reference?");
                    }
                    break;

                case LineTag.InputCommand:
                    OnInputCommand?.Invoke(FFI.LoadInputCommand());
                    break;

                case LineTag.End:
                    Exit();
                    break;
            }
            OnLine?.Invoke(Tag);
            return Tag;
        }

        /// <summary>
        /// Calls next after waiting a bit.
        /// Must be called using MonoBehaviour.StartCoroutine.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator DelayedNext(float seconds, string input = "")
        {
            Debug.Log("Calling Runner.DelayedNext");
            isWaiting = true;
            yield return new WaitForSecondsRealtime(seconds);
            while (Time.timeScale != 1f)
            {
                yield return null;
            }

            isWaiting = false;
            Next(input);
        }

        /// <summary>
        /// Calls next after waiting a bit.
        /// Must be called using GameObject.StartCoroutine.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator DelayedNext(Func<bool> predicate, string input = "")
        {
            isWaiting = true;
            yield return new WaitUntil(predicate);

            isWaiting = false;
            Next(input);
        }

        /// <summary>
        /// Exit out of the current passage. 
        /// Can be used to forcibly exit out of a running, incompleted passage.
        /// </summary>
        public static void Exit()
        {
            isRunning = false;
            OnDialogueEnd.Invoke();
        }

        /// <summary>
        /// Calls exit after waiting a bit.
        /// Must be called using MonoBehaviour.StartCoroutine.
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static IEnumerator DelayedExit(int frames)
        {
            Debug.Log("Calling Runner.DelayedExit");
            isWaiting = true;
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
            while (Time.timeScale != 1f)
            {
                yield return null;
            }

            isWaiting = false;
            Exit();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Compiles the story at `storyPath` with bookmark at `bookmarkPath` to `targetPath`.
        /// Generates a constants file and saves it to the Kataru source directory.
        /// </summary>
        /// <param name="storyPath"></param>
        /// <param name="bookmarkPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="codegenPath"></param>
        public static void Compile(string storyPath, string bookmarkPath, string targetPath, string codegenPath)
        {
            Debug.Log($@"Runner.Compile(storyPath: '{storyPath}'
                bookmarkPath: '{bookmarkPath}'
                targetPath: '{targetPath}'
                codegenPath: '{codegenPath}')");
            try
            {
                bool validate = true;
                FFI.InitRunner(storyPath, bookmarkPath, validate);

                Debug.Log($"Story at '{storyPath}' validated. Saving compiled story to '{targetPath}'.");
                FFI.SaveStory(targetPath);
                FFI.CodegenConsts(codegenPath);

                // Force unity to recompile using the newly generated source code.
                if (FFI.CodegenWasUpdated())
                {
                    Debug.Log($"Constants file generated at {targetPath}");
                    UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
                }
            }
            catch (System.EntryPointNotFoundException e)
            {
                Debug.LogError($"Kataru error: could not find FFI command named '{e.Message}'");
            }
            catch (Exception e)
            {
                Debug.LogError($"Kataru error: {e.ToString()}");
            }
        }
#endif
    }
}