using System;
using System.Collections.Generic;
using JSM.Surveillance.Surveillance;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;



namespace JSM.Surveillance.Surveillance
{
    using UnityEditor;
    using UnityEngine;
    public class HEFaceGameData : ScriptableObject
    {
        public int dailyPopulation;
        public bool isStreet = true;
        public InformationRatio ratio;
        public string companyId;
    }
    
    public class HEFaceEditor : PopupWindowContent
    {
        private Vector2 _size = new Vector2(260, 230);

        private HEFaceGameData _state;
        private SerializedObject _so;
        private SerializedProperty _dailyPopulationProp;
        private SerializedProperty _isStreetProp;
        private SerializedProperty _ratioProp;
        private SerializedProperty _companyIdProp;

        // Optional: let a caller pass in an existing state object
        public HEFaceEditor(HEFaceGameData state = null)
        {
            _state = state;
        }

        public override void OnOpen()
        {
            if (_state == null)
            {
                _state = ScriptableObject.CreateInstance<HEFaceGameData>();
            }

            _so = new SerializedObject(_state);
            _dailyPopulationProp = _so.FindProperty("dailyPopulation");
            _isStreetProp        = _so.FindProperty("isStreet");
            _ratioProp           = _so.FindProperty("ratio");       // the struct
            _companyIdProp       = _so.FindProperty("companyId");
        }

        public override Vector2 GetWindowSize() => _size;

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Face Editor", EditorStyles.boldLabel);

            _so.Update();

            EditorGUILayout.PropertyField(_isStreetProp, new GUIContent("Is Street"));
            EditorGUILayout.PropertyField(_dailyPopulationProp, new GUIContent("Daily Population"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_ratioProp, new GUIContent("Information Ratio"), true);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_companyIdProp, new GUIContent("Company ID"));

            EditorGUILayout.Space();
            if (GUILayout.Button("Close"))
            {
                _so.ApplyModifiedProperties();
                editorWindow.Close();
            }

            _so.ApplyModifiedProperties();
        }
    }

}