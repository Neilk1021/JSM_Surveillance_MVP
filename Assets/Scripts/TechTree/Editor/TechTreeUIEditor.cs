#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace Surveillance.TechTree
{
    [CustomEditor(typeof(TechTreeUI))]
    public class TechTreeUIEditor : Editor
    {
        private ReorderableList nodeList;
        private SerializedProperty nodesProp;

        private void OnEnable()
        {
            var techTreeProp = serializedObject.FindProperty("techTree");
            nodesProp = techTreeProp.FindPropertyRelative("Nodes");

            nodeList = new ReorderableList(serializedObject, nodesProp, true, true, true, true);

            nodeList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
        
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
        
                // 1. Calculate the next uniqueID
                int nextID = 0;
                for (int i = 0; i < index; i++)
                {
                    int existingID = list.serializedProperty.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("ID").intValue;
                    if (existingID >= nextID) nextID = existingID + 1;
                }
        
                element.FindPropertyRelative("ID").intValue = nextID;
                element.FindPropertyRelative("Name").stringValue = "New Node";
                element.FindPropertyRelative("Description").stringValue = "";
                element.FindPropertyRelative("IsUnlocked").boolValue = false;
        
                element.FindPropertyRelative("ParentIDs").ClearArray();
                element.FindPropertyRelative("Unlockables").ClearArray();
            };

            
            nodeList.elementHeightCallback = (index) =>
            {
                if (nodeList.index == index)
                {
                    var element = nodesProp.GetArrayElementAtIndex(index);
                    
                    float baseHeight = 115f; // Info row + Desc + Buttons
                    float parentsHeight = EditorGUI.GetPropertyHeight(element.FindPropertyRelative("ParentIDs"), true);
                    float unlockablesHeight = EditorGUI.GetPropertyHeight(element.FindPropertyRelative("Unlockables"), true);
                    
                    return baseHeight + parentsHeight + unlockablesHeight + 10f; 
                }
                return EditorGUIUtility.singleLineHeight + 4;
            };

            nodeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = nodesProp.GetArrayElementAtIndex(index);
                rect.y += 2;

                if (nodeList.index == index)
                {
                    DrawExpandedNode(rect, element, index);
                }
                else
                {
                    DrawCompactNode(rect, element);
                }
            };
        }

        private void DrawCompactNode(Rect rect, SerializedProperty element)
        {
            float idWidth = 30;
            float nameWidth = 120;
            float descWidth = rect.width - idWidth - nameWidth - 80;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, idWidth, 18), element.FindPropertyRelative("ID"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + idWidth + 5, rect.y, nameWidth, 18), element.FindPropertyRelative("Name"), GUIContent.none);
            
            GUI.enabled = false;
            int parentCount = element.FindPropertyRelative("ParentIDs").arraySize;
            int unlockCount = element.FindPropertyRelative("Unlockables").arraySize;
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 80, rect.y, 80, 18), $"P:{parentCount} U:{unlockCount}");
            GUI.enabled = true;
        }

        private void DrawExpandedNode(Rect rect, SerializedProperty element, int index)
        {
            float y = rect.y;
            
            EditorGUI.LabelField(new Rect(rect.x, y, 50, 18), "Name:");
            EditorGUI.PropertyField(new Rect(rect.x + 50, y, rect.width - 110, 18), element.FindPropertyRelative("Name"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 50, y, 50, 18), $"ID: {element.FindPropertyRelative("ID").intValue}");
            
            y += 22;
            
            EditorGUI.LabelField(new Rect(rect.x, y, rect.width, 18), "Description:");
            y += 18;
            element.FindPropertyRelative("Description").stringValue = EditorGUI.TextArea(new Rect(rect.x, y, rect.width, 40), element.FindPropertyRelative("Description").stringValue);
            
            y += 45;

            SerializedProperty parents = element.FindPropertyRelative("ParentIDs");
            float parentsHeight = EditorGUI.GetPropertyHeight(parents, true);
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, parentsHeight), parents, true);
            y += parentsHeight + 2;

            if (GUI.Button(new Rect(rect.x, y, rect.width, 20), "Add Dependency (By Name)"))
            {
                ShowParentSelector(parents, element.FindPropertyRelative("ID").intValue);
            }
            y += 25;

            SerializedProperty unlockables = element.FindPropertyRelative("Unlockables");
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUI.GetPropertyHeight(unlockables, true)), unlockables, true);
        }

        private void ShowParentSelector(SerializedProperty parentArrayProp, int currentID)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < nodesProp.arraySize; i++)
            {
                var node = nodesProp.GetArrayElementAtIndex(i);
                int id = node.FindPropertyRelative("ID").intValue;
                string name = node.FindPropertyRelative("Name").stringValue;

                if (id == currentID) continue;

                menu.AddItem(new GUIContent(name), false, () => {
                    for (int j = 0; j < parentArrayProp.arraySize; j++)
                    {
                        if (parentArrayProp.GetArrayElementAtIndex(j).intValue == id) return;
                    }
                    
                    int newIndex = parentArrayProp.arraySize;
                    parentArrayProp.InsertArrayElementAtIndex(newIndex);
                    parentArrayProp.GetArrayElementAtIndex(newIndex).intValue = id;
                    parentArrayProp.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "techTree", "m_Script");
            
            EditorGUILayout.Space(10);
            nodeList.DoLayoutList();

            if (GUILayout.Button("Build Visuals", GUILayout.Height(30)))
            {
                ((TechTreeUI)target).Build();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
