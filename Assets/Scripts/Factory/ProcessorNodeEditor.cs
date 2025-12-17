using UnityEditor;

namespace JSM.Surveillance
{
    [CustomEditor(typeof(ProcessorPort))]
    public class ProcessorNodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get a reference to the target script instance
            ProcessorPort portScript = (ProcessorPort)target;
            DrawDefaultInspector();
            SerializedProperty posX = serializedObject.FindProperty("posX");
            SerializedProperty posY = serializedObject.FindProperty("posY");
            
            serializedObject.Update();
            var draggable = portScript.GetComponentInParent<Draggable>();
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