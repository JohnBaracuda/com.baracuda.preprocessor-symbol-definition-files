using Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities;
using UnityEditor;
using UnityEngine;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Inspector
{
    /// <summary>
    /// Custom property drawer class for PreprocessorSymbolData objects.
    /// </summary>
    [CustomPropertyDrawer(typeof(PreprocessorSymbolData))]
    public sealed class PreprocessorSymbolDataDrawer : PropertyDrawer
    {
        /*
         *  Private Fields   
         */

        private static readonly Color inactiveColor = new Color(0.53f, 0.53f, 0.53f);

        /*
         *  GUI   
         */
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);
            
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            // Calculate rects
            var enabledRect     = new Rect(position.x, position.y, 20, position.height);
            var appliedRect     = new Rect(position.x + 18, position.y, 18, position.height);
            var textRect        = new Rect(position.x + 30, position.y, 290, position.height);
            var targetLabelRect = new Rect(position.x + 330, position.y, 45, position.height);
            var targetRect      = new Rect(position.x + 375, position.y, position.width - 375, position.height);

            var targetGroup = (FlagsBuildTargetGroup)property.FindPropertyRelative("targetGroup").intValue;
            var color = GUI.contentColor;

            var targetFile = property.serializedObject.targetObject as PreprocessorSymbolDefinitionFile;
            var enabled = property.FindPropertyRelative("enabled").boolValue;
            var symbol = property.FindPropertyRelative("symbol").stringValue;
            var activeAndEnabled = enabled && targetGroup.HasFlag(PreprocessorDefineUtilities.FlagsBuildTargetCache);
            var isDefined = PreprocessorDefineUtilities.IsSymbolDefined(symbol);
            
            GUI.contentColor = activeAndEnabled ? color : inactiveColor;

            // Draw fields - pass GUIContent.none to each so they are drawn without labels.
            // Draw empty label fields to display tooltips. 
            
            // Draw enabled field
            EditorGUI.PropertyField(enabledRect, property.FindPropertyRelative("enabled"), GUIContent.none);
            EditorGUI.LabelField(enabledRect, new GUIContent("", Extensions.GetTooltipOfField<PreprocessorSymbolData>("enabled")));
            
            // Draw symbol * to indicate that changes need to be applied
            DrawChangesIndicationGUI(targetGroup, isDefined, activeAndEnabled, symbol, appliedRect, color);

            // Draw symbol field
            DrawSymbolGUI(property, textRect);

            // Draw build target field
            DrawBuildTargetGUI(property, color, targetLabelRect, targetRect);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        /*
         *  GUI Elements   
         */
        
        private static void DrawSymbolGUI(SerializedProperty property, Rect textRect)
        {
            EditorGUI.PropertyField(textRect, property.FindPropertyRelative("symbol"),
                GUIContent.none);
            EditorGUI.LabelField(textRect,
                new GUIContent("", Extensions.GetTooltipOfField<PreprocessorSymbolData>("symbol")));
        }

        private void DrawBuildTargetGUI(SerializedProperty property, Color color, Rect targetLabelRect, Rect targetRect)
        {
            GUI.contentColor = color;
            EditorGUI.LabelField(targetLabelRect,
                new GUIContent("Target", Extensions.GetTooltipOfField<PreprocessorSymbolData>("targetGroup")));
            EditorGUI.PropertyField(targetRect, property.FindPropertyRelative("targetGroup"),
                GUIContent.none);
            EditorGUI.LabelField(targetRect,
                new GUIContent("", Extensions.GetTooltipOfField<PreprocessorSymbolData>("targetGroup")));
        }
        
        /// <summary>
        /// Draws the GUI element that displays if unsaved changes are present. 
        /// </summary>
        private void DrawChangesIndicationGUI(FlagsBuildTargetGroup targetGroup, bool isDefined, bool activeAndEnabled, string symbol, Rect appliedRect, Color color)
        {
            if (isDefined && !activeAndEnabled && targetGroup.HasFlag(PreprocessorDefineUtilities.FlagsBuildTargetCache))
            {
                GUI.contentColor = color;
                EditorGUI.LabelField(appliedRect, new GUIContent("*", $"Changes must be applied!"));
                GUI.contentColor = inactiveColor;
            }
            else if (!isDefined && activeAndEnabled && targetGroup.HasFlag(PreprocessorDefineUtilities.FlagsBuildTargetCache))
            {
                EditorGUI.LabelField(appliedRect, new GUIContent("*", $"{symbol} is not Defined! Apply to define the symbol"));
            }
            else
            {
                EditorGUI.LabelField(appliedRect, GUIContent.none);
            }
        }
    }
}