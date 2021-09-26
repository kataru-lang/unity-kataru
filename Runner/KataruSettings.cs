using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace Kataru
{
    // Create a new type of Settings Asset.
    public class KataruSettings : ScriptableObject
    {
        // Constants
        public const string SettingsDirectory = "Assets/Kataru",
            SettingsFile = "Kataru Settings";

        // Tooltips
        public const string codegenPathTooltip = "The path where Kataru generated C# code should be saved, relative to project root. Standard installations use Assets/Scripts/Kataru/Constants.Generated.cs.",
            storyPathTooltip = "The path to the story folder containing Kataru YAML files (.yml), relative to Application.dataPath(Assets / when in editor). Should be in an Editor directory to avoid inclusion in build.",
            bookmarkPathTooltip = "The path to the default bookmark file (.yml or .bin), relative to Application.dataPath(Assets / when in editor).",
            targetPathTooltip = "Path to output compiled Kataru script (.bin), relative to Application.dataPath (Assets/ when in editor).",
            savePathTooltip = "Path to store the player's save data, relative to Application.persistentDataPath.";

        [SerializeField]
        [Tooltip(codegenPathTooltip)]
        public string codegenPath;

        [SerializeField]
        [Tooltip(storyPathTooltip)]
        public string storyPath;

        [SerializeField]
        [Tooltip(bookmarkPathTooltip)]
        public string bookmarkPath;

        [SerializeField]
        [Tooltip(targetPathTooltip)]
        public string targetPath;

        [SerializeField]
        [Tooltip(savePathTooltip)]
        public string savePath;

        private static KataruSettings instance = null;
        public static KataruSettings Get(bool createIfMissing = false)
        {
            if (instance != null) return instance;
            instance = Load(createIfMissing);
            return instance;
        }

        internal static KataruSettings Load(bool createIfMissing = false)
        {
            KataruSettings settings = Resources.Load<KataruSettings>(SettingsFile);
            Debug.Log($@"KataruSettings.Load(SettingsFile: '{SettingsFile}')");
#if UNITY_EDITOR
            if (createIfMissing && settings == null)
            {
                Debug.LogWarning($"Could not find settings at '{SettingsFile}', creating defaults...");
                if (!Directory.Exists(SettingsDirectory))
                {
                    Directory.CreateDirectory(SettingsDirectory);
                }

                settings = ScriptableObject.CreateInstance<KataruSettings>();
                settings.storyPath = "Kataru/Editor/Story";
                settings.bookmarkPath = "Kataru/Bookmark.yml";
                settings.targetPath = "Kataru/story.bin";
                settings.savePath = "Kataru/bookmark.bin";
                settings.codegenPath = "Assets/Scripts/Kataru/Constants.Generated.cs";
                AssetDatabase.CreateAsset(settings, SettingsDirectory + "/Resources/" + SettingsFile);
                AssetDatabase.SaveAssets();
            }
#endif
            return settings;
        }
    }

#if UNITY_EDITOR
    // Create KataruSettingsProvider by deriving from SettingsProvider:
    public class KataruSettingsProvider : SettingsProvider
    {
        private SerializedObject serializedSettings;

        class Styles
        {
            public static GUIContent codegenPath = new GUIContent("Kataru Source Path"),
                storyPath = new GUIContent("Story Path"),
                bookmarkPath = new GUIContent("Bookmark Path"),
                targetPath = new GUIContent("Target Path"),
                savePath = new GUIContent("Save Path");
        }

#if UNITY_EDITOR
        public KataruSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope) { }
#endif
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            serializedSettings = new SerializedObject(KataruSettings.Get(createIfMissing: true));
        }

        public override void OnGUI(string searchContext)
        {
            // Use IMGUI to display UI:
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("codegenPath"), Styles.codegenPath);
            EditorGUILayout.HelpBox(KataruSettings.codegenPathTooltip, MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("storyPath"), Styles.storyPath);
            EditorGUILayout.HelpBox(KataruSettings.storyPathTooltip, MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("bookmarkPath"), Styles.bookmarkPath);
            EditorGUILayout.HelpBox(KataruSettings.bookmarkPathTooltip, MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("targetPath"), Styles.targetPath);
            EditorGUILayout.HelpBox(KataruSettings.targetPathTooltip, MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("savePath"), Styles.savePath);
            EditorGUILayout.HelpBox(KataruSettings.savePathTooltip, MessageType.None);
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplyModifiedProperties();
            }
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateKataruSettingsProvider()
        {
            KataruSettings.Load();
            var provider = new KataruSettingsProvider("Project/Kataru Settings", SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }
    }
#endif
}