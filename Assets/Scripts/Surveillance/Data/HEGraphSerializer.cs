using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.Surveillance
{
    
    public class HEGraphSerializer  
    {
        public static string ToJson(HEGraphData data)
        {
            Debug.Log(JsonUtility.ToJson(data.faces.ToArray()));
            return JsonUtility.ToJson(data, true);
        }
    }
}
