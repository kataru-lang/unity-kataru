using UnityEngine;
using System.IO;

namespace Kataru
{
    /// <summary>
    /// Kataru Manager class.
    /// This class is in charge of initializing the Runner.
    /// Also includes examples of reacting to Runner's events.
    /// </summary>
    public class Manager : Handler
    {
        [Header("Paths")]
        [SerializeField] string savePath;
        [SerializeField] string bookmarkPath;
        [SerializeField] string storyPath;
        static bool initialized = false;

        string absoluteSavePath;
        string absoluteBookmarkPath;
        string absoluteStoryPath;

        protected virtual void OnValidate()
        {
            absoluteBookmarkPath = Path.Combine(Application.dataPath, bookmarkPath);
            absoluteStoryPath = Path.Combine(Application.dataPath, storyPath);
            absoluteSavePath = Path.Combine(Application.persistentDataPath, savePath);
        }

        protected virtual void Start()
        {
            if (initialized) Debug.LogError(@"A Kataru Manager was already initialized.
                You should only have one Kataru Manager in your scene.");
            Runner.Init(
                storyPath: absoluteStoryPath,
                bookmarkPath: absoluteBookmarkPath,
                savePath: absoluteSavePath);
            initialized = true;
        }
    }
}