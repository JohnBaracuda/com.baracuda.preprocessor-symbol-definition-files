using System.Collections.Generic;
using System.Linq;
using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts
{
    /// <summary>
    /// Settings file managing and elevated symbols and options regarding definition files.
    /// </summary>
    // [CreateAssetMenu(menuName = "Preprocessor Definition Settings (Debug)", fileName = FILENAME, order = 90)]
    internal sealed class PreprocessorSymbolDefinitionSettings : ScriptableObject
    {
        #region --- [SERIALIZE] ---

        [SerializeField] [HideInInspector] private bool removeSymbolsOnDelete = true;
        
        [SerializeField] [HideInInspector] private List<string> elevatedSymbols = new List<string>();
        [SerializeField] [HideInInspector] private List<PreprocessorSymbolDefinitionFile> scriptDefineSymbolFiles = null;
#if UNITY_2020_2_OR_NEWER
        [SerializeField] [HideInInspector] private bool saveOnCompile = true;
#endif
        [SerializeField] [HideInInspector] private bool logMessages = true;

        [SerializeField] [HideInInspector] private bool showAllDefinedSymbols = true;
        #endregion
        
        //---------

        #region --- [PROPERTIES] ---

        internal const string NAME_ELEVATED_SYMBOLS = nameof(elevatedSymbols);
        internal const string NAME_SDS_FILES = nameof(scriptDefineSymbolFiles);

        /// <summary>
        /// Get a list of currently elevated symbols.
        /// </summary>
        internal static List<string> ElevatedSymbols => Instance.elevatedSymbols;
        
        /// <summary>
        /// Removes the content of a Preprocessor Symbol Definition File when it is deleted.
        /// If this option is not enabled the symbols of a deleted file will be elevated and must be removed manually
        /// </summary>
        internal static bool RemoveSymbolsOnDelete
        {
            get => Instance.removeSymbolsOnDelete;
            set => Instance.removeSymbolsOnDelete = value;
        } 
        
        /// <summary>
        /// When enabled, lists of all defined symbols will be displayed in the inspector of the settings file as well as
        /// the inspector of Preprocessor Symbol Definition Files
        /// </summary>
        internal static bool ShowAllDefinedSymbols
        {
            get => Instance.showAllDefinedSymbols;
            set => Instance.showAllDefinedSymbols = value;
        } 
        
#if UNITY_2020_2_OR_NEWER
        /// <summary>
        /// When enabled, unsaved changes will be applied when scripts are recompiling.
        /// </summary>
        internal static bool SaveOnCompile
        {
            get => Instance.saveOnCompile;
            set => Instance.saveOnCompile = value;
        }
#endif

        /// <summary>
        /// When enabled, messages will be logged when symbols are removed, added or elevated.
        /// </summary>
        internal static bool LogMessages
        {
            get => Instance.logMessages;
            set => Instance.logMessages = value;
        }
        
        /// <summary>
        /// Get a list of all ScriptDefineSymbolFiles located in the project.
        /// </summary>
        internal static ICollection<PreprocessorSymbolDefinitionFile> ScriptDefineSymbolFiles =>
            Instance.scriptDefineSymbolFiles.IsNullOrIncomplete()
                ? Instance.scriptDefineSymbolFiles = Extensions.FindAllAssetsOfType<PreprocessorSymbolDefinitionFile>()
                : Instance.scriptDefineSymbolFiles;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [ELEVATED SYMBOLS] ---

        internal static void RemoveElevatedSymbol(string symbol)
        {
            Instance.elevatedSymbols.TryRemove(symbol);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [FILECACHING] ---

        internal static void AddScriptDefineSymbolFile(PreprocessorSymbolDefinitionFile file)
        {
#if UNITY_2020_1_OR_NEWER
            (Instance.scriptDefineSymbolFiles ??= new List<PreprocessorSymbolDefinitionFile>()).TryAdd(file);
#else
             (Instance.scriptDefineSymbolFiles ?? (Instance.scriptDefineSymbolFiles = new List<PreprocessorSymbolDefinitionFile>())).TryAdd(file);
#endif
        }
        
        internal static void RemoveScriptDefineSymbolFile(PreprocessorSymbolDefinitionFile file)
        {
#if UNITY_2020_1_OR_NEWER
            (Instance.scriptDefineSymbolFiles ??= new List<PreprocessorSymbolDefinitionFile>()).TryRemove(file);
#else
             (Instance.scriptDefineSymbolFiles ?? (Instance.scriptDefineSymbolFiles = new List<PreprocessorSymbolDefinitionFile>())).TryRemove(file);
#endif
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [SINGLETON] ---

        internal static PreprocessorSymbolDefinitionSettings Instance
#if UNITY_2020_1_OR_NEWER
            => _instance ??= Extensions.FindAllAssetsOfType<PreprocessorSymbolDefinitionSettings>().FirstOrDefault() ?? CreateInstanceAsset();
#else
            => _instance ? _instance : _instance = Extensions.FindAllAssetsOfType<PreprocessorSymbolDefinitionSettings>().FirstOrDefault() ?? CreateInstanceAsset();
#endif
        
        
        private static PreprocessorSymbolDefinitionSettings _instance = null;

        private static PreprocessorSymbolDefinitionSettings CreateInstanceAsset()
        {
            var asset = CreateInstance<PreprocessorSymbolDefinitionSettings>();
            AssetDatabase.CreateAsset(asset, CreateFilePath());
            AssetDatabase.SaveAssets();
            return asset;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [MISCELLANEOUS] ---

        public static void Select()
        {
            Selection.activeObject = Instance;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [FILECREATION] ---
     
        /// <summary>
        /// Returns the best fitting filepath for the configuration asset.
        /// </summary>
        /// <returns></returns>
        private static string CreateFilePath()
        {
            foreach (var path in Paths)
            {
                if (Directory.Exists(path))
                    return $"{path}/{FILENAME_ASSET}";
            }

            return DefaultPath;
        }
        
        private const string FILENAME       = "Preprocessor-Definition-Settings";
        private const string FILENAME_ASSET = "Preprocessor-Definition-Settings.asset";
        private static readonly string DefaultPath = $"Assets/{FILENAME_ASSET}";
        
        
        private static readonly string[] Paths =
        {
            "Assets/Plugins/Baracuda/PreprocessorDefinitionFiles/Config",
            "Assets/Baracuda/PreprocessorDefinitionFiles/Config",
            "Assets/PreprocessorDefinitionFiles/Config",
            "Assets/Plugins/Baracuda/PreprocessorDefinitionFiles/Configurations",
            "Assets/Baracuda/PreprocessorDefinitionFiles/Configurations",
            "Assets/PreprocessorDefinitionFiles/Configurations",
            "Assets/Plugins/Baracuda/PreprocessorDefinitionFiles/Configuration",
            "Assets/Baracuda/PreprocessorDefinitionFiles/Configuration",
            "Assets/PreprocessorDefinitionFiles/Configuration",
            "Assets/Plugins/Baracuda/PreprocessorDefinitionFiles/Settings",
            "Assets/Baracuda/PreprocessorDefinitionFiles/Settings",
            "Assets/PreprocessorDefinitionFiles/Settings",
            "Assets/Plugins/Baracuda/PreprocessorDefinitionFiles",
            "Assets/Baracuda/PreprocessorDefinitionFiles",
            "Assets/PreprocessorDefinitionFiles",
            "Assets/Config",
            "Assets/Configurations",
            "Assets/Configuration",
            "Assets/Settings",
            "Assets/Baracuda",
            "Assets/Baracuda/Config",
            "Assets/Baracuda/Configuration",
            "Assets/Baracuda/Configurations",
            "Assets/Baracuda/Settings",
        };
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [ON BEFORE COMPILE: AUTOSAVE] ---
        
        private void OnEnable()
        {
            if (AssetDatabase.GetAssetPath(this) == DefaultPath)
            {
                AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(this), CreateFilePath());
            }
#if UNITY_2020_2_OR_NEWER
            UnityEditor.Compilation.CompilationPipeline.compilationStarted += OnCompilationStarted;
#endif
        }

        private void OnDisable()
        {
#if UNITY_2020_2_OR_NEWER
            UnityEditor.Compilation.CompilationPipeline.compilationStarted -= OnCompilationStarted;
#endif
        }

#if UNITY_2020_2_OR_NEWER
        private static void OnCompilationStarted(object obj)
        {
            if(SaveOnCompile)
                PreprocessorDefineUtilities.ApplyAndUpdateAllDefinitionFiles();
        }
#endif
        #endregion

    }

    internal static class Documentation
    {
        private const string ONLINE_DOCS = "https://johnbaracuda.com/ppsdf.html";
        private static GUIContent DocsButtonGUIContent(string selector) => 
            new GUIContent("Documentation", $"Open the online documentation for this asset.\n{ONLINE_DOCS}#{selector}");
        
        internal static void OpenOnlineDocumentation(string selector = null)
        {
            if (selector is null)
            {
                Application.OpenURL(ONLINE_DOCS);
            }
            else
            {
                Application.OpenURL($"{ONLINE_DOCS}#{selector}");   
            }
        }

        /// <summary>
        /// Draw a button that will open the online documentation for this asset.
        /// </summary>
        internal static void DrawDocumentationButton(string selector = null)
        {
            if (GUILayout.Button(DocsButtonGUIContent(selector)))
            {
                OpenOnlineDocumentation(selector);
            }
        }
    }
}
