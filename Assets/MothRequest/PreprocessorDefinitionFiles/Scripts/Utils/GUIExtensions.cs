using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MothRequest.PreprocessorDefinitionFiles.Utils
{
    internal static class GUIExtensions
    {
        #region --- [DRAW SYMBOLS] ---

        private static ReorderableList globalCustomList;
        private static ReorderableList versionDefines;
        private static ReorderableList compilerDefines;
        private static ReorderableList platformDefines;

        private static readonly GUIContent CopyA = new GUIContent("Copy Preset", "Copy To Clipboard");
        private static readonly GUIContent CopyB = new GUIContent("Copy", "Copy To Clipboard");

        /// <summary>
        /// Draw GUI elements displaying every active global symbol.
        /// </summary>
        internal static void DrawGlobalSymbols()
        {
            DrawGUISpace(40);
            DrawGUILine();

            EditorGUILayout.LabelField("Globally Defined Symbol", new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            }, GUILayout.Height(25));

            DrawGUIMessage("Note that lists might not contain every available define!");
            DrawGUISpace();

            globalCustomList.DoLayoutList();
            DrawGUIMessage("Only version defines are of the <b>current version</b> are listed. " +
                           "Older version defines with the <b>OR_NEWER suffix</b> are also viable!");
            versionDefines.DoLayoutList();
            compilerDefines.DoLayoutList();
            platformDefines.DoLayoutList();
        }

        private static Rect ButtonRectA(Rect rect) => new Rect(rect.width - 40, rect.y, 65, rect.height);
        private static Rect ButtonRectB(Rect rect) => new Rect(rect.width - 135, rect.y, 95, rect.height);

        private static void DrawElement(Rect rect, int index, ref string[] element)
        {
            EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, rect.width - 5, rect.height), element[index]);
            // --- 
            CopyA.tooltip = $"Copy the following to your clipboard:\n #if {element[index]} \n\n#endif";
            if (GUI.Button(ButtonRectB(rect), CopyA))
            {
                EditorGUIUtility.systemCopyBuffer = $"#if {element[index]}\n\n#endif";
            }

            // --- 
            CopyB.tooltip = $"Copy '{element[index]}' to clipboard";
            if (GUI.Button(ButtonRectA(rect), CopyB))
            {
                EditorGUIUtility.systemCopyBuffer = element[index];
            }
        }


        [InitializeOnLoadMethod]
        private static void InitializeGUI()
        {
            // --- CUSTOM SYMBOLS
            var symbols = PreprocessorDefineUtilities.GetCustomDefinesOfActiveTargetGroup().ToArray();
            globalCustomList = new ReorderableList(symbols, typeof(string), false, true, false, false);
            globalCustomList.drawElementCallback +=
                (rect, index, active, focused) => DrawElement(rect, index, ref symbols);
            globalCustomList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Custom Defines");

            // --- VERSION SYMBOLS
            var version = PreprocessorDefineUtilities.VersionDefines.ToArray();
            versionDefines = new ReorderableList(version, typeof(string), false, true, false, false);
            versionDefines.drawElementCallback +=
                (rect, index, active, focused) => DrawElement(rect, index, ref version);
            versionDefines.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Version Defines");


            // --- COMPILER SYMBOLS
            var compiler = PreprocessorDefineUtilities.CompilerDefines.ToArray();
            ;
            compilerDefines = new ReorderableList(compiler, typeof(string), false, true, false, false);
            compilerDefines.drawElementCallback +=
                (rect, index, active, focused) => DrawElement(rect, index, ref compiler);
            compilerDefines.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Compiler Defines");


            // --- PLATFORM SYMBOLS
            var platform = PreprocessorDefineUtilities.PlatformDefines.ToArray();
            ;
            platformDefines = new ReorderableList(platform, typeof(string), false, true, false, false);
            platformDefines.drawElementCallback +=
                (rect, index, active, focused) => DrawElement(rect, index, ref platform);
            platformDefines.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Platform Defines");
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [GENERIC GUI ELEMENTS] ---

        private static readonly Color DefaultLightColor = new Color(.8f, .8f, .9f, .5f);
        private static readonly Color DefaultDarkColor = new Color(.2f, .2f, .2f, .5f);

        /// <summary>
        /// Draw Line in Inspector
        /// </summary>
        internal static void DrawGUILine(Color? color = null, int thickness = 1, int padding = 1, bool space = false)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += (float) padding / 2;
            rect.x -= 2;
            rect.width += 4;
            EditorGUI.DrawRect(rect, color ?? (EditorGUIUtility.isProSkin ? DefaultLightColor : DefaultDarkColor));
        }

        internal static void DrawGUISpace(int space = 10) => GUILayout.Space(space);

        internal static void DrawGUIMessage(string message, int size = 12)
        {
            var style = GUI.skin.GetStyle("HelpBox");
            style.fontSize = size;
            style.richText = true;
            EditorGUILayout.TextArea(message, style);
        }

        internal static void DrawGUIMessage(Rect rect, string message, int size = 12)
        {
            var style = GUI.skin.GetStyle("HelpBox");
            style.fontSize = size;
            style.richText = true;
            GUI.TextArea(rect, message, style);
        }

        #endregion
    }
}