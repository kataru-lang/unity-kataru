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

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (initialized) Debug.LogError(@"A Kataru Manager was already initialized.
                You should only have one Kataru Manager in your scene.");
            if (string.IsNullOrEmpty(bookmarkPath)) Debug.LogError("bookmarkPath is null or empty.");
            if (string.IsNullOrEmpty(storyPath)) Debug.LogError("storyPath is null or empty.");
            if (string.IsNullOrEmpty(savePath)) Debug.LogError("savePath is null or empty.");
#endif
            Runner.Init(
                storyPath: absoluteStoryPath,
                bookmarkPath: absoluteBookmarkPath,
                savePath: absoluteSavePath);
            initialized = true;
        }
    }
}