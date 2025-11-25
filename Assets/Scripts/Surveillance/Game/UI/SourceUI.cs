using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class SourceUI : MonoBehaviour
    {
        //TODO make it actually load the needed data.
        public void Init()
        {
            if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
        }
    }
}
