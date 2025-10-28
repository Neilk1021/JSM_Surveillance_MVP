using System;
using System.Collections.Generic;
using JSM.Surveillance.Surveillance;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;



namespace JSM.Surveillance.Surveillance
{
    public class HEFaceEditor : PopupWindowContent 
    {
        private Vector2 _size = new Vector2(220, 140);
        private int _dailyPopulation;

        public override Vector2 GetWindowSize() => _size;

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Face Editor", EditorStyles.boldLabel);
            
            _dailyPopulation = EditorGUILayout.IntField("Daily Population:", _dailyPopulation);

            EditorGUILayout.Space();
            if (GUILayout.Button("Do Thing"))
            {
                Debug.Log($"Did thing for: {_dailyPopulation}");
                editorWindow.Close(); // close when clicked
            }
        }
    }
}