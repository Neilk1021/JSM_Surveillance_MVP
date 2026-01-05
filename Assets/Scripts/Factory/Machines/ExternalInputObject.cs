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
            ReloadText();
            _parentSource.OnIncomingSourcesChanged.AddListener(ReloadText);
        }

        public void ReloadText()
        {
            var incoming = _parentSource.IncomingSourceLinks[_incomingSourceIndex];
            if(debugLabel != null)
                debugLabel.text = incoming != null ? $"{incoming.SourceName}\n[{incoming.GetOutputResourceType()?.ResourceName}]" : "No incoming\nsource";
        }

        public override MachineInstance BuildInstance()
        {
            return new ExternalInputInstance(_parentSource, _incomingSourceIndex, inventorySize);
        }

    }
}