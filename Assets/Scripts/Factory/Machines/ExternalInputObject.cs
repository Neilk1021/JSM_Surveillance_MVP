using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance
{
    public class ExternalInputObject : MachineObject
    {
        [SerializeField] private TextMeshProUGUI debugLabel;
        private Source _parentSource;
        private int _incomingSourceIndex; 

        protected override void OnMouseDown()
        {
            if (Placed) {
                Grid.UIManager.SwitchUI(this);
            }

            base.OnMouseDown();
        }

        public void Initialize(Source source, int index)
        {
            _parentSource = source;
            _incomingSourceIndex = index;
            var incoming = _parentSource.IncomingSourceLinks[index];
            
            if(debugLabel != null)
                debugLabel.text = incoming != null ? $"{incoming.SourceName}" : "Nothing";
        }

        public override MachineInstance BuildInstance()
        {
            return new ExternalInputInstance(_parentSource, _incomingSourceIndex, inventorySize);
        }

    }
}