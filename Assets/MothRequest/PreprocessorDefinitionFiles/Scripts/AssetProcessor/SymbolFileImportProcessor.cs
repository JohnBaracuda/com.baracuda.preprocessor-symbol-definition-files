using MothRequest.PreprocessorDefinitionFiles.Utils;
using UnityEditor;

namespace MothRequest.PreprocessorDefinitionFiles.AssetProcessor
{
    /// <summary>
    /// AssetPostprocessor responsible to apply and update Preprocessor Defines when an asset has been imported.
    /// </summary>
    internal class SymbolFileImportProcessor : AssetPostprocessor
    {
#if !PREPROCESSOR_DEFINITION
        
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AssetDatabase.importPackageCompleted -= OnPackageImportCompleted;
            AssetDatabase.importPackageCompleted += OnPackageImportCompleted;
        }

        private static void OnPackageImportCompleted(string name)
        {
            PreprocessorDefineUtilities.ApplyAndUpdateAllDefinitionFiles();
        }
#endif
    }
}
