using UnityEngine;
using UnityEditor;

namespace Surveillance.MoneyGraph
{
    [CustomEditor(typeof(MoneyGraphUI))]
    public class MoneyGraphEditor : Editor
    {
        private Vector2 pointInput;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MoneyGraphUI ui = (MoneyGraphUI)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Reset Money Graph"))
            {
                ui.ResetGraph();
                EditorUtility.SetDirty(ui);
            }

            if (GUILayout.Button("Clear Points"))
            {
                ui.ClearPoints();
                EditorUtility.SetDirty(ui);
            }

            if (GUILayout.Button("Initialize Lists(Debug only)"))
            {
                ui.DebugInitializeLists();
                EditorUtility.SetDirty(ui);
            }

            if (GUILayout.Button("ReDraw Lines"))
            {
                ui.DrawLines();
                EditorUtility.SetDirty(ui);
            }

            if (GUILayout.Button("Play Animation(runtime only)"))
            {
                ui.PlayAnimation();
            }

            GUILayout.Space(20);

            pointInput = EditorGUILayout.Vector2Field("New Point Coords", pointInput);

            if (GUILayout.Button("Add Point"))
            {
                ui.AddPoint(pointInput);
                EditorUtility.SetDirty(ui);
            }

        }
    }
}
