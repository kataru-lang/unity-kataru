using UnityEngine;
using UnityEngine.Events;
using System;

namespace Kataru
{
    /// <summary>
    /// The Kataru Runner serves as the high level interface with the Kataru Rust FFI module.
    /// This ScriptableObject 
    /// </summary>
    [CreateAssetMenu(fileName = "KataruRunner", menuName = "ScriptableObjects/Kataru/Runner", order = 1)]
    public class Runner : ScriptableObject
    {
        [SerializeField] public string bookmarkPath = "Kataru/Bookmark.yml";
        [SerializeField] public string savePath = "Bookmark.bin";
        [SerializeField] public string storyPath = "Kataru/Story";

        public string BookmarkPath { get => Application.dataPath + "/" + bookmarkPath; }
        public string SavePath { get => Application.persistentDataPath + "/" + savePath; }
        public string StoryPath { get => Application.dataPath + "/" + storyPath; }

        // Events to listen to.
        public event Action<Choices> OnChoices;
        public event Action OnInvalidChoice;
        public event Action OnDialogueEnd;
        public event Action<InputCommand> OnInputCommand;

        public EventMap<Command> Commands = new EventMap<Command>();
        public EventMap<Dialogue> Characters = new EventMap<Dialogue>();

        /// <summary>
        /// Initialize the story, bookmark and internal runner.
        /// This method should only be called once.
        /// </summary>
        public void Init()
        {
#if UNITY_EDITOR
            Debug.Log($"Kataru.Init('{StoryPath}')");
#endif
            FFI.LoadStory(StoryPath);
            FFI.Validate();
            FFI.LoadBookmark(BookmarkPath);
            FFI.InitRunner();
        }

        /// <summary>
        /// Save the bookmark to path.
        /// </summary>
        public void Save()
        {
#if UNITY_EDITOR
            Debug.Log($"Kataru.Save('{SavePath}')");
#endif
            FFI.SaveBookmark(SavePath);
        }

        /// <summary>
        /// Load bookmark from path.
        /// </summary>
        public void Load()
        {
            FFI.LoadBookmark(SavePath);
            FFI.InitRunner();
        }

        public void SetLine(int line) => FFI.SetLine(line);
        public void GotoPassage(string passage) => FFI.GotoPassage(passage);
        public void SetState(string key, string value) => FFI.SetState(key, value);
        public void SetState(string key, double value) => FFI.SetState(key, value);
        public void SetState(string key, bool value) => FFI.SetState(key, value);

        /// <summary>
        /// Progress the story using the given input.
        /// This yields line data from internal dialogue runner, whose data is passed via invoking actions.
        /// </summary>
        /// <param name="input"></param>
        public void Next(string input = "")
        {
#if UNITY_EDITOR
            Debug.Log($"Kataru.Next('" + input + "')");
#endif
            FFI.Next(input);
            Debug.Log($"tag: {FFI.Tag()}");
            switch (FFI.Tag())
            {
                case LineTag.Choices:
                    OnChoices.Invoke(FFI.LoadChoices());
                    break;

                case LineTag.InvalidChoice:
                    OnInvalidChoice.Invoke();
                    break;

                case LineTag.Dialogue:
                    Dialogue dialogue = FFI.LoadDialogue();
                    Characters.Invoke(dialogue.name, dialogue);
                    break;

                case LineTag.Commands:
                    foreach (Command command in FFI.LoadCommands())
                    {
                        Commands.Invoke(command.name, command);
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
    }
}