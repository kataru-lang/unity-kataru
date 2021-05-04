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
        const string SettingsDirectory = "Assets/Kataru";
        const string SettingsFile = "Kataru Settings";

        [SerializeField]
        public string storyPath;

        [SerializeField]
        [Tooltip("File path of bookmark relative to StreamingAssets folder")]
        public string bookmarkPath;

        [SerializeField]
        [Tooltip("File name of compiled Kataru relative to StreamingAssets folder")]
        public string targetPath;

        [SerializeField]
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
                AssetDatabase.CreateAsset(settings, SettingsDirectory + "/Resources/" + SettingsFile);
                AssetDatabase.SaveAssets();
            }
#endif
            return settings;
        }

        // private void OnEnable()
        // {
        //     if (instance == null) instance = Load();
        //     Refresh(this);
        // }

        // // Force refresh the contents of the instance 
        // void Refresh(KataruSettings instance)
        // {
        //     var settings = Get();
        //     settings.storyPath = instance.storyPath;
        //     settings.bookmarkPath = instance.bookmarkPath;
        //     settings.targetPath = instance.targetPath;
        //     settings.savePath = instance.savePath;
        // }
    }

#if UNITY_EDITOR
    // Create KataruSettingsProvider by deriving from SettingsProvider:
    public class KataruSettingsProvider : SettingsProvider
    {
        // private KataruSettings settings;
        private SerializedObject serializedSettings;

        class Styles
        {
            public static GUIContent storyPath = new GUIContent("Story Path");
            public static GUIContent bookmarkPath = new GUIContent("Bookmark Path");
            public static GUIContent targetPath = new GUIContent("Target Path");
            public static GUIContent savePath = new GUIContent("Save Path");
        }

#if UNITY_EDITOR
        public KataruSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
            : base(path, scope) { }
#endif
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            Debug.LogWarning("OnActivate");
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            serializedSettings = new SerializedObject(KataruSettings.Get(createIfMissing: true));
        }

        public override void OnGUI(string searchContext)
        {
            // Use IMGUI to display UI:
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("storyPath"), Styles.storyPath);
            EditorGUILayout.HelpBox(
                "The path to the story folder containing Kataru YAML files (.yml). " +
                "Should be in an Editor directory to avoid inclusion in build.",
                MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("bookmarkPath"), Styles.bookmarkPath);
            EditorGUILayout.HelpBox("The path to the default bookmark file (.yml or .bin).",
                MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("targetPath"), Styles.targetPath);
            EditorGUILayout.HelpBox("Path to output compiled Kataru script (.bin).", MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("savePath"), Styles.savePath);
            EditorGUILayout.HelpBox("Path relative to Application.persistentDataPath to store the player's save data.", MessageType.None);
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
            Debug.LogWarning("CreateKataruSettingsProvider");
            KataruSettings.Load();
            var provider = new KataruSettingsProvider("Project/Kataru Settings", SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }
    }
#endif
}