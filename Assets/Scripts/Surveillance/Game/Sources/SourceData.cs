using UnityEngine;

namespace JSM.Surveillance.Game
{
    
    [CreateAssetMenu(fileName = "Source", menuName = "JSM/Surveillance/Maintainable/Source")]
    public class SourceData : Maintainable
    {
        [SerializeField] private Source source;
        public Source Source => source;
    }
}