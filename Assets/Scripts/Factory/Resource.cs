using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewResource", menuName = "Factory/Resource")]
    public class Resource : ScriptableObject
    {
        public string guid;
        public string resourceName;
        public Sprite sprite;
    }
}
