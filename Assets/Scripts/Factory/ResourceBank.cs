using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewResourceBank", menuName = "JSM/Surveillance/ResourceBank")]
    public class ResourceBank : ScriptableObject
    {
        public Resource[] resources;

        /// <summary>
        /// Finds a resource by its unique ID
        /// </summary>
        public Resource GetResourceById(string guid)
        {
            return System.Array.Find(resources, r => r.Guid == guid);
        }

        /// <summary>Finds a resource by its name
        /// </summary>
        public Resource GetResourceByName(string name)
        {
            return System.Array.Find(resources, r => r.ResourceName == name);
        }
    }
}

