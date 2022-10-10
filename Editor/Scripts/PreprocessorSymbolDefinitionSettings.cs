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
    public sealed class PreprocessorSymbolDefinitionSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        #region --- Data ---

        /*
         *  Inspector Fields   
         */

        [SerializeField] [HideInInspector] private bool removeSymbolsOnDelete = true;

        [SerializeField] [HideInInspector] private List<string> elevatedSymbols = new List<string>();
        [SerializeField] [HideInInspector]
        private List<PreprocessorSymbolDefinitionFile> scriptDefineSymbolFiles = null;
#if UNITY_2020_2_OR_NEWER
        [SerializeField] [HideInInspector] private bool saveOnCompile = true;
#endif
        [SerializeField] [HideInInspector] private bool logMessages = true;

        [SerializeField] [HideInInspector] private bool showAllDefinedSymbols = true;

        /*
         *  Fields (Inspector)   
         */

        internal const string NameElevatedSymbols = nameof(elevatedSymbols);
        internal const string NameSdsFiles = nameof(scriptDefineSymbolFiles);


        private const string FilenameAsset = "Preprocessor-Definition-Settings.asset";
        private static readonly string defaultPath = $"Assets/{FilenameAsset}";


        private static readonly string[] preferredPaths =
        {
            "Assets/PreprocessorDefinitionFiles",
            "Assets/Baracuda/PreprocessorDefinitionFiles",
            "Assets/Modules/PreprocessorDefinitionFiles",
            "Assets/Plugins/PreprocessorDefinitionFiles",
            "Assets/Plugins/Baracuda/PreprocessorDefinitionFiles",
        };

        /*
         *  Properties   
         */

        /// <summary>
        /// Get a list of currently elevated symbols.
        /// </summary>
        public static List<string> ElevatedSymbols => FindOrCreateSettingsAsset().elevatedSymbols;

        /// <summary>
        /// Removes the content of a Preprocessor Symbol Definition File when it is deleted.
        /// If this option is not enabled the symbols of a deleted file will be elevated and must be removed manually
        /// </summary>
        public static bool RemoveSymbolsOnDelete
        {
            get => FindOrCreateSettingsAsset().removeSymbolsOnDelete;
            set => FindOrCreateSettingsAsset().removeSymbolsOnDelete = value;
        }

        /// <summary>
        /// When enabled, lists of all defined symbols will be displayed in the inspector of the settings file as well as
        /// the inspector of Preprocessor Symbol Definition Files
        /// </summary>
        public static bool ShowAllDefinedSymbols
        {
            get => FindOrCreateSettingsAsset().showAllDefinedSymbols;
            set => FindOrCreateSettingsAsset().showAllDefinedSymbols = value;
        }

#if UNITY_2020_2_OR_NEWER
        /// <summary>
        /// When enabled, unsaved changes will be applied when scripts are recompiling.
        /// </summary>
        public static bool SaveOnCompile
        {
            get => FindOrCreateSettingsAsset().saveOnCompile;
            set => FindOrCreateSettingsAsset().saveOnCompile = value;
        }
#endif

        /// <summary>
        /// When enabled, messages will be logged when symbols are removed, added or elevated.
        /// </summary>
        public static bool LogMessages
        {
            get => FindOrCreateSettingsAsset().logMessages;
            set => FindOrCreateSettingsAsset().logMessages = value;
        }

        /// <summary>
        /// Get a list of all ScriptDefineSymbolFiles located in the project.
        /// </summary>
        public static ICollection<PreprocessorSymbolDefinitionFile> ScriptDefineSymbolFiles =>
            FindOrCreateSettingsAsset().scriptDefineSymbolFiles.IsNullOrIncomplete()
                ? FindOrCreateSettingsAsset().scriptDefineSymbolFiles = Extensions.FindAllAssetsOfType<PreprocessorSymbolDefinitionFile>()
                : FindOrCreateSettingsAsset().scriptDefineSymbolFiles;

        #endregion

        #region --- Singleton ---

        public static PreprocessorSymbolDefinitionSettings FindOrCreateSettingsAsset()
            => instance
                ? instance
                : instance = Extensions.FindAllAssetsOfType<PreprocessorSymbolDefinitionSettings>()
                    .FirstOrDefault() ?? CreateInstanceAsset();

        private static PreprocessorSymbolDefinitionSettings instance = null;

        private static PreprocessorSymbolDefinitionSettings CreateInstanceAsset()
        {
            var asset = CreateInstance<PreprocessorSymbolDefinitionSettings>();
            AssetDatabase.CreateAsset(asset, CreateFilePath());
            AssetDatabase.SaveAssets();
            return asset;
        }

        private static string CreateFilePath()
        {
            foreach (var path in preferredPaths)
            {
                if (Directory.Exists(path))
                {
                    return $"{path}/{FilenameAsset}";
                }
            }

            return defaultPath;
        }
        
        #endregion

        /*
         *  Symbol Handling   
         */

        public static void RemoveElevatedSymbol(string symbol)
        {
            FindOrCreateSettingsAsset().elevatedSymbols.TryRemove(symbol);
        }

        public static void FindAllPreprocessorSymbolDefinitionFiles()
        {
            FindOrCreateSettingsAsset().scriptDefineSymbolFiles?.Clear();
            foreach (var file in Extensions.FindAllAssetsOfType<PreprocessorSymbolDefinitionFile>())
            {
                if (file == null)
                {
                    continue;
                }

                AddScriptDefineSymbolFile(file);
            }
        }

        public static void AddScriptDefineSymbolFile(PreprocessorSymbolDefinitionFile file)
        {
            (FindOrCreateSettingsAsset().scriptDefineSymbolFiles ?? (FindOrCreateSettingsAsset().scriptDefineSymbolFiles =
                new List<PreprocessorSymbolDefinitionFile>())).AddUnique(file);
        }

        public static void RemoveScriptDefineSymbolFile(PreprocessorSymbolDefinitionFile file)
        {
            (FindOrCreateSettingsAsset().scriptDefineSymbolFiles ?? (FindOrCreateSettingsAsset().scriptDefineSymbolFiles =
                new List<PreprocessorSymbolDefinitionFile>())).TryRemove(file);
        }
        
        /*
         *  Before Auto Save
         */

        private void OnEnable()
        {
            if (AssetDatabase.GetAssetPath(this) == defaultPath)
            {
                AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(this), CreateFilePath());
            }
            
#if UNITY_2020_2_OR_NEWER
            UnityEditor.Compilation.CompilationPipeline.compilationStarted += OnCompilationStarted;
#endif
        }

#if UNITY_2020_2_OR_NEWER
        private void OnDisable()
        {
            UnityEditor.Compilation.CompilationPipeline.compilationStarted -= OnCompilationStarted;
        }

        private static void OnCompilationStarted(object obj)
        {
            if (SaveOnCompile && !BuildPipeline.isBuildingPlayer)
            {
                PreprocessorDefineUtilities.ApplyAndUpdateAllDefinitionFiles();
            }
        }
#endif

        /*
         *  Misc   
         */

        public static void Select()
        {
            Selection.activeObject = FindOrCreateSettingsAsset();
        }
        
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            instance = this;
        }
    }
}