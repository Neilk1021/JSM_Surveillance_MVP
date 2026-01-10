using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    public enum ResourceCategory
    {
        Basic,
        Consumer,
        Corp,
        Govt
    }
    
    [CreateAssetMenu(fileName = "NewResource", menuName = "JSM/Surveillance/Resource")]
    public class Resource : ScriptableObject
    {
        [SerializeField] private string guid = System.Guid.NewGuid().ToString();
        [SerializeField] private string resourceName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int value;
        [SerializeField] private ResourceCategory resourceCategory;
        
        public int Value => value;
        public string Guid => guid;
        public string ResourceName => resourceName;
        public Sprite Sprite => sprite;

        public ResourceCategory ResourceCategory => resourceCategory;
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();
        }
    }
}
