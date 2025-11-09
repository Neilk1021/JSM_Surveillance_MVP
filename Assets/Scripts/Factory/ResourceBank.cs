using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewResourceBank", menuName = "Factory/ResourceBank")]
    public class ResourceBank : ScriptableObject
    {
        public Resource[] resources;

        public Resource GetResourceById(string guid)
        {
            return System.Array.Find(resources, r => r.guid == guid);
        }

        public Resource GetResourceByName(string name)
        {
            return System.Array.Find(resources, r => r.resourceName == name);
        }
    }
}

