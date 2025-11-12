using UnityEditor;
using UnityEngine;

namespace Surveillance.Game
{
    [CustomEditor(typeof(MapLoader))]
    public class MapLoaderEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MapLoader loader = (MapLoader)target;

            if (GUILayout.Button("Load Map"))
            {
                loader.LoadMap();
            }
        }
    }
}