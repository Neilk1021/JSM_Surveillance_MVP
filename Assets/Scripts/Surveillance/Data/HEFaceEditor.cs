


namespace JSM.Surveillance.Surveillance
{
    using UnityEngine;
    #if UNITY_EDITOR
    using UnityEditor;
    public class HEFaceEditor : PopupWindowContent
    {
    private Vector2 _size = new Vector2(260, 245);

        private HEFaceGameData _state;
        private SerializedObject _so;
        private SerializedProperty _dailyPopulationProp;
        private SerializedProperty _isStreetProp;
        private SerializedProperty _ratioProp;
        private SerializedProperty _companyIdProp;
        private SerializedProperty _riskFactorProp;
        private HEFace _face;
        
        public HEFaceEditor(HEFace face)
        {
            _state = face.Data;
            _face = face;
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
            _riskFactorProp = _so.FindProperty("riskFactor");
        }

        public override Vector2 GetWindowSize() => _size;

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Face Editor", EditorStyles.boldLabel);

            _so.Update();

            EditorGUILayout.PropertyField(_isStreetProp, new GUIContent("Is Street"));
            EditorGUILayout.PropertyField(_dailyPopulationProp, new GUIContent("Daily Population"));
            EditorGUILayout.PropertyField(_riskFactorProp , new GUIContent("Risk factor (0-1)"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_ratioProp, new GUIContent("Information Ratio"), true);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_companyIdProp, new GUIContent("Company ID"));

            EditorGUILayout.Space();
            if (GUILayout.Button("Save"))
            {
                _so.ApplyModifiedProperties();
                _face.SyncDataJsonFromSO();
                editorWindow.Close();
            }

            _so.ApplyModifiedProperties();
        }
    }
    #endif 
    public class HEFaceGameData : ScriptableObject
    {
        public int dailyPopulation;
        public float riskFactor = 0;
        public bool isStreet = true;
        public InformationRatio ratio;
        public string companyId;
    }

}