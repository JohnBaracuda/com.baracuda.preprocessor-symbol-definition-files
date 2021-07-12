using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MothRequest.PreprocessorDefinitionFiles.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MothRequest.PreprocessorDefinitionFiles.InspectorGUI
{
    /// <summary>
    /// Editor class drawing a custom inspector for Preprocessor-Symbol-Definition-Files
    /// </summary>
    [CustomEditor(typeof(PreprocessorSymbolDefinitionFile))]
    internal class PreprocessorSymbolDefinitionFileInspector : Editor
    {
        #region --- [FIELDS] ---

        private PreprocessorSymbolDefinitionFile targetObject = null;
        private SerializedProperty serializedProperty;
        private ReorderableList localCustomList;

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [INIT] ---

        private void OnEnable()
        {
            // Cache the target as an PreprocessorSymbolDefinitionFile instance.
            targetObject = target as PreprocessorSymbolDefinitionFile;

            // Initialize the local reorderable preprocessor define list
            serializedProperty = serializedObject.FindProperty(PreprocessorSymbolDefinitionFile.SYMBOLS_PROPERTY_NAME);
            localCustomList = new ReorderableList(serializedObject, serializedProperty, true, true, true, true);
            localCustomList.drawElementCallback += DrawListItem;
            localCustomList.drawHeaderCallback += DrawLocalList;
        }

        private void OnDisable()
        {
            try
            {
                localCustomList.drawElementCallback -= DrawListItem;
                localCustomList.drawHeaderCallback -= DrawLocalList;
            }
            catch
            {
                // ignored
            }
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [ON GUI] ---

        public override void OnInspectorGUI()
        {
            DrawTitle();
            GUIExtensions.DrawGUILine();
            GUIExtensions.DrawGUISpace();
            DrawToggleButtons();
            GUIExtensions.DrawGUISpace();
            DrawSymbolList();
            DrawWarningMessages();
            GUIExtensions.DrawGUISpace();
            
            if(PreprocessorSymbolDefinitionSettings.ShowAllDefinedSymbols)
                GUIExtensions.DrawGlobalSymbols();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [DRAWING: LIST] ---
        private void DrawLocalList(Rect rect) => DrawListHeader(rect, "Locally Defined Symbols");

        private void DrawListHeader(Rect rect, string header)
        {
            EditorGUI.LabelField(rect, header);
        }

        private void DrawListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = localCustomList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element);
        }

        #endregion
        
        //---------
        
        #region --- [DRAWING: INSPECTOR] ---
        
        /// <summary>
        /// Draw the title and the settings button to enable easy access to the settings file from every definition file.
        /// </summary>
        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preprocessor Symbol Definition File",
                new GUIStyle(GUI.skin.label) {fontSize = 16, fontStyle = FontStyle.Bold}, GUILayout.Height(25));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Settings", GUILayout.Width(80)))
            {
                PreprocessorSymbolDefinitionSettings.Select();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw the three control buttons that will enable / disable or toggle all symbols.
        /// </summary>
        private void DrawToggleButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle", GUILayout.Width(60)))
            {
                foreach (var localSymbol in targetObject.LocalSymbols)
                {
                    localSymbol.SetEnabled(!localSymbol.Enabled);
                }
            }

            if (GUILayout.Button("Enable", GUILayout.Width(60)))
            {
                foreach (var localSymbol in targetObject.LocalSymbols)
                {
                    localSymbol.SetEnabled(true);
                }
            }

            if (GUILayout.Button("Disable", GUILayout.Width(60)))
            {
                foreach (var localSymbol in targetObject.LocalSymbols)
                {
                    localSymbol.SetEnabled(false);
                }
            }


            GUILayout.FlexibleSpace();
            GUILayout.Label("Current Build Target:");
            GUILayout.Label($"{PreprocessorDefineUtilities.BuildTarget}",
                new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold, fontSize = 13});
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw the list containing the local (file based) preprocessor symbols.
        /// </summary>
        private void DrawSymbolList()
        {
            serializedObject.Update();
            localCustomList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                targetObject.ApplyPreprocessorDefines();
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [WARNING MESSAGE] ---

        private readonly Stack<string> checkedSymbols = new Stack<string>();
        
        /// <summary>
        /// Draw the cached warning messages if necessary. 
        /// </summary>
        private void DrawWarningMessages()
        {
            // Set every symbol as valid before checking which are not. 
            foreach (var symbol in targetObject.LocalSymbols)
            {
                symbol.IsValid = true;
            }
            
            // Check for multiple entries in this file and draw warning message if necessary.  
            CheckMultipleEntriesLocal();

            // Check for multiple entries in other files and draw warning message if necessary.  
            CheckMultipleEntriesGlobal();

            // Check for usage of elevated symbols and draw warning message if necessary.  
            CheckForElevatedSymbols();
        }

        
        /// <summary>
        /// Check for multiple entries in this file and draw warning message if necessary.  
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckMultipleEntriesLocal()
        {
            foreach (var defineData in targetObject.LocalSymbols)
            {
                if (checkedSymbols.Contains(defineData.Symbol)) continue;
                if (targetObject.LocalSymbols.Count(other => defineData.Equals(other)) > 1)
                {
                    defineData.IsValid = false;
                    var messageMultipleInFile =
                        $"<b><color=#FF3333>WARNING!</color> [{defineData.Symbol}]</b> is defined multiple times! " +
                        $"Adding this symbol can lead to accidental loss or unexpected behaviour! " +
                        $"Please ensure that preprocessor symbols are not defined more than once.";
                    GUIExtensions.DrawGUIMessage(messageMultipleInFile);
                }

                checkedSymbols.Push(defineData.Symbol);
            }

            checkedSymbols.Clear();
        }
        
        
        /// <summary>
        /// Check for multiple entries in other files and draw warning message if necessary.  
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckMultipleEntriesGlobal()
        {
            foreach (var defineData in targetObject.LocalSymbols)
            {
                if (checkedSymbols.Contains(defineData.Symbol)) continue;
                if (PreprocessorDefineUtilities.IsSymbolDefinedElsewhere(defineData.Symbol, targetObject))
                {
                    defineData.IsValid = false;
                    var messageMultipleInFile =
                        $"<b><color=#FF3333>WARNING!</color> [{defineData.Symbol}]</b> is already defined in another file! " +
                        $"Adding this symbol can lead to accidental loss or unexpected behaviour! " +
                        $"Please ensure that preprocessor symbols are not defined more than once.";
                    GUIExtensions.DrawGUIMessage(messageMultipleInFile);
                }

                checkedSymbols.Push(defineData.Symbol);
            }

            checkedSymbols.Clear();
        }
        
        
        /// <summary>
        /// Check for usage of elevated symbols and draw warning message if necessary.  
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckForElevatedSymbols()
        {
            foreach (var defineData in targetObject.LocalSymbols)
            {
                if (checkedSymbols.Contains(defineData.Symbol)) continue;
                if (PreprocessorDefineUtilities.IsSymbolElevated(defineData.Symbol))
                {
                    defineData.IsValid = false;
                    var messageMultipleInFile =
                        $"<b><color=#FF3333>WARNING!</color> [{defineData.Symbol}]</b> has an elevated status! " +
                        "Adding this symbol can lead to accidental loss or unexpected behaviour, if it is used by a third-party plugin! " +
                        "if, however, this symbol was defined by you before this plugin was installed, " +
                        "feel free to remove it from the list of elevated symbols and manage it from this file!";
                    GUIExtensions.DrawGUIMessage(messageMultipleInFile);

                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button(new GUIContent(
                        $"Remove from this file", 
                        $"Remove [{defineData.Symbol}] from this file. " +
                        $"Chose this option, if this symbol is handled by a third party plugin."), 
                        GUILayout.Height(30), GUILayout.MinWidth(200)))
                    {
                        targetObject.RemovePreprocessorSymbol(defineData);
                        break; // break because the collection was modified
                    }
                    if(GUILayout.Button(new GUIContent(
                        $"Manage from this file (Caution)",
                        $"Remove [{defineData.Symbol}] from the list of elevated symbols, located in the settings file. " +
                        $"Chose this option, if you want to manage the symbol from this file."),
                        GUILayout.Height(30), GUILayout.MinWidth(200)))
                    {
                        PreprocessorSymbolDefinitionSettings.RemoveElevatedSymbol(defineData.Symbol);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                checkedSymbols.Push(defineData.Symbol);
            }

            checkedSymbols.Clear();
        }

        #endregion
    }
}