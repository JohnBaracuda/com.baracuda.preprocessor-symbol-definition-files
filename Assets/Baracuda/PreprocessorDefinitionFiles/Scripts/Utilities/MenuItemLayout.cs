using UnityEditor;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities
{
    internal static class MenuItemLayout
    {
        [MenuItem("Baracuda/Documentation/Preprocessor-Symbol-Definition-File", priority = 20000)]
        [MenuItem("Baracuda/Preprocessor-Symbol-Definition-File/Documentation", priority = 1001)]
        private static void OpenDefaultOnlineDocumentation()
        {
            Documentation.OpenOnlineDocumentation();
        }

        [MenuItem("Baracuda/Preprocessor-Symbol-Definition-File/Settings", priority = 1000)]
        public static void SelectPreprocessorSymbolDefinitionSettings()
        {
            PreprocessorSymbolDefinitionSettings.Select();
        }
    }
}
