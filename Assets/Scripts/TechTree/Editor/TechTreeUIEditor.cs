#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Surveillance.TechTree
{
    [CustomEditor(typeof(TechTreeUI))]
    public class TechTreeUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TechTreeUI ui = (TechTreeUI)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Example Tech Tree#1"))
            {
                ui.techTree.FillWithExampleData();
                ui.Build();
                EditorUtility.SetDirty(ui);
            }

            if (GUILayout.Button("Generate Example Tech Tree#2"))
            {
                ui.techTree.FillWithExampleData2();
                ui.Build();
                EditorUtility.SetDirty(ui);
            }

            if (GUILayout.Button("Clear Tech Tree"))
            {
                ui.techTree.Nodes = new Node[0];
                ui.Build();
                EditorUtility.SetDirty(ui);
            }
        }
    }
}
#endif
