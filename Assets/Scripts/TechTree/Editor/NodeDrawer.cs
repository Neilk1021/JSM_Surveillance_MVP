#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Surveillance.TechTree
{
    [CustomPropertyDrawer(typeof(Node))]
    public class NodeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, property.FindPropertyRelative("Name").stringValue);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
            
                Rect nameRect = new Rect(position.x, position.y + 20, position.width * 0.7f, 18);
                Rect idRect = new Rect(position.x + (position.width * 0.75f), position.y + 20, position.width * 0.25f, 18);

                EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"), GUIContent.none);
                EditorGUI.LabelField(idRect, "ID: " + property.FindPropertyRelative("ID").intValue);

                Rect descRect = new Rect(position.x, position.y + 42, position.width, 40);
                SerializedProperty descProp = property.FindPropertyRelative("Description");
                descProp.stringValue = EditorGUI.TextArea(descRect, descProp.stringValue);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Adjust height based on whether the node is expanded
            return property.isExpanded ? 90f : EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif