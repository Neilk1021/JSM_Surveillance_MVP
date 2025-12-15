using UnityEditor;

namespace JSM.Surveillance
{
    [CustomEditor(typeof(ProcessorNode))]
    public class ProcessorNodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get a reference to the target script instance
            ProcessorNode nodeScript = (ProcessorNode)target;
            DrawDefaultInspector();
            SerializedProperty posX = serializedObject.FindProperty("posX");
            SerializedProperty posY = serializedObject.FindProperty("posY");
            
            serializedObject.Update();
            var draggable = nodeScript.GetComponentInParent<Draggable>();
            if (draggable != null)
            {
                posX.intValue = EditorGUILayout.IntSlider(
                    "Pos X",
                    posX.intValue,
                    -1,
                    draggable.Size.x
                );
                
                posY.intValue = EditorGUILayout.IntSlider(
                    "Pos Y",
                    posY.intValue,
                    -1,
                    draggable.Size.y
                );
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}