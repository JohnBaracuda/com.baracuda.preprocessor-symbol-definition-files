using MothRequest.PreprocessorDefinitionFiles.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MothRequest.PreprocessorDefinitionFiles.InspectorGUI
{
    [CustomEditor(typeof(PreprocessorSymbolDefinitionSettings))]
    public class PreprocessorSymbolDefinitionSettingsInspector : Editor
    {
        #region --- [FIELDS] ---

        
        private SerializedProperty securedSymbolProperty;
        private SerializedProperty sdsFileProperty;
        private ReorderableList securedSymbolList;
        private ReorderableList sdsFileList;
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [ENABLE / DISABLE] ---

        private void OnEnable()
        {
            securedSymbolProperty = serializedObject.FindProperty(PreprocessorSymbolDefinitionSettings.NAME_ELEVATED_SYMBOLS);
            sdsFileProperty = serializedObject.FindProperty(PreprocessorSymbolDefinitionSettings.NAME_SDS_FILES);
            
            securedSymbolList = new ReorderableList(serializedObject, securedSymbolProperty, true, true, true, true);
            securedSymbolList.drawHeaderCallback += rect => DrawHeader(rect, "Elevated Symbols");
            securedSymbolList.drawElementCallback += DrawSecuredSymbols;
            
            sdsFileList = new ReorderableList(serializedObject, sdsFileProperty, false, true, false, false);
            sdsFileList.drawHeaderCallback += rect => DrawHeader(rect, "Preprocessor Symbol Definition Files");
            sdsFileList.drawElementCallback += DrawFiles;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [ON GUI] ---

        private const string INFO_MESSAGE =
            "<b>Symbols in this list are considered elevated and will " +
            "not be handled by definition files. If you would like to manage elevated symbols from a definition file, " +
            "you can do so by removing them from this list and adding them to a definition file.</b> " +
            "Note that active symbols, that are not listed in any definition file will automatically be elevated. " +
            "This means, that if you've just installed this plugin, any previously set symbol will be elevated and must " +
            "first be removed from this list if you want to manage it from a definition file.";
        
        public override void OnInspectorGUI()
        {
            DrawTitle();
            GUIExtensions.DrawGUILine();
            GUIExtensions.DrawGUISpace();
            
            DrawToggleControls();
            
            GUIExtensions.DrawGUISpace();

            GUIExtensions.DrawGUIMessage(INFO_MESSAGE);
            GUIExtensions.DrawGUISpace(3);
            serializedObject.Update();
            DrawElevatedSymbols();
            GUIExtensions.DrawGUISpace();
            
            sdsFileList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            DrawSaveAll();
            if(PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols)
                GUIExtensions.DrawGlobalSymbols();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [DRAWING GUI ELEMENTS] ---
        
        private void DrawFiles(Rect rect, int index, bool isActive, bool isFocused)
        {
            var file = (PreprocessorSymbolDefinitionFile)
                sdsFileList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
            
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), new GUIContent($"{file.name}"), style);
            var path = AssetDatabase.GetAssetPath(file).Replace($"{file.name}.asset", "");
            
            EditorGUI.LabelField(new Rect(rect.x + 300, rect.y, rect.width - 370, rect.height), $"Path: {path}");
            if (GUI.Button(new Rect(rect.x + rect.width -60, rect.y, 60, rect.height), "Select"))
            {
                Selection.activeObject = file;
            }
        }


        private static readonly GUIContent CheckForSymbolsContent = 
            new GUIContent(
                "Check For Elevated Symbols",
                "Scan the project for symbols that should be elevated.");
        
        private void DrawElevatedSymbols()
        {
            securedSymbolList.DoLayoutList();
            
            if (GUILayout.Button(CheckForSymbolsContent, GUILayout.Width(200)))
            {
                PreprocessorDefineUtilities.ElevateIndependentSymbols();
            }
        }
        
        
        private void DrawSaveAll()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save All", GUILayout.Width(120)))
            {
                PreprocessorDefineUtilities.ApplyAndUpdateAllDefinitionFiles();
            }
            GUILayout.EndHorizontal();
        }
        
        //---------
        
        private void DrawSecuredSymbols(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = securedSymbolList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        //---------
        
        private void DrawHeader(Rect rect, string header)
        {
            EditorGUI.LabelField(rect, header);
        }
        
        //---------
        
        
        private static readonly GUIContent RemoveSymbolsOnDeleteContent = new GUIContent(
            "Remove Symbols On Delete",
            "Removes the symbols of a Preprocessor Symbol Definition File when it is deleted. " +
            "If this option is not enabled the symbols of a deleted file will be elevated and must be removed manually");

        private static readonly GUIContent LogMessagesContent = new GUIContent(
            "Log Messages",
            "When enabled messages will be logged when symbols are removed, added or elevated.");

        private static readonly GUIContent SaveOnCompileContent = new GUIContent(
            "Save And Apply On Load",
            "When enabled unsaved changes will be applied when scripts are recompiling.");
        
        private static readonly GUIContent ShowAllDefinedSymbols = new GUIContent(
            "Show All Defined Symbols",
            "When enabled, lists of all defined symbols will be displayed in the inspector of the settings file " +
            "as well as the inspector of Preprocessor Symbol Definition Files.");
        
        private static void DrawToggleControls()
        {
            
            EditorGUILayout.BeginHorizontal();
            // --- Draw RemoveSymbolsOnDelete Toggle
            PreprocessorSymbolDefinitionSettings.RemoveSymbolsOnDelete
                = EditorGUILayout.ToggleLeft(RemoveSymbolsOnDeleteContent, PreprocessorSymbolDefinitionSettings.RemoveSymbolsOnDelete);
            GUILayout.FlexibleSpace();
            
            // --- Draw Build Target
            GUILayout.Label("Current Build Target:");
            GUILayout.Label($"{PreprocessorDefineUtilities.BuildTarget}",
                new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold, fontSize = 13});
            EditorGUILayout.EndHorizontal();
            
            // --- Draw LogMessages Toggle 
            PreprocessorSymbolDefinitionSettings.LogMessages = EditorGUILayout.ToggleLeft(
                LogMessagesContent,
                PreprocessorSymbolDefinitionSettings.LogMessages);
            
            // --- Draw SaveOnCompile Toggle
#if UNITY_2020_2_OR_NEWER
            PreprocessorSymbolDefinitionSettings.SaveOnCompile = EditorGUILayout.ToggleLeft(
                SaveOnCompileContent,
                PreprocessorSymbolDefinitionSettings.SaveOnCompile);
#endif
            
            // --- Draw ShowAllDefinedSymbols Toggle
            PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols = EditorGUILayout.ToggleLeft(
                ShowAllDefinedSymbols,
                PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols);
        }
        
        //---------
        
        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preprocessor Symbol Settings", new GUIStyle(GUI.skin.label)
            {
                fontSize = 16, 
                fontStyle = FontStyle.Bold
            }, GUILayout.Height(25));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Player Settings", GUILayout.Width(200)))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
