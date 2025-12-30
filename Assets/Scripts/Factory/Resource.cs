using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewResource", menuName = "JSM/Surveillance/Resource")]
    public class Resource : ScriptableObject
    {
        [SerializeField] private string guid = System.Guid.NewGuid().ToString();
        [SerializeField] private string resourceName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int value;

        public int Value => value;
        public string Guid => guid;
        public string ResourceName => resourceName;
        public Sprite Sprite => sprite;
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();
        }
    }
}
