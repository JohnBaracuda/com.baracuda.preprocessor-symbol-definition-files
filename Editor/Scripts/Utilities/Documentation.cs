using UnityEngine;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities
{
    internal static class Documentation
    {
        private const string OnlineDocs = "https://johnbaracuda.com/ppsdf.html";
        private static GUIContent DocsButtonGUIContent(string selector) => 
            new GUIContent("Documentation", $"Open the online documentation for this asset.\n{OnlineDocs}#{selector}");
        
        internal static void OpenOnlineDocumentation(string selector = null)
        {
            if (selector is null)
            {
                Application.OpenURL(OnlineDocs);
            }
            else
            {
                Application.OpenURL($"{OnlineDocs}#{selector}");   
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