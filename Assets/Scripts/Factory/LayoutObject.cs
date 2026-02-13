using System.IO;
using JSM.Surveillance.Game;
using JSM.Surveillance.Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class LayoutObject : ScriptableObject
    {
        [SerializeField] private SourceDTO sourceDto;
        public SourceDTO SourceDto => sourceDto;
        
        public void Init(SourceDTO sourceDto)
        {
            this.sourceDto = sourceDto;
        }
    }
}