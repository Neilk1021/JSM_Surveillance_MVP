using JSM.Surveillance.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSM.Surveillance
{
    public class ExternalInputObject : MachineObject
    {
        [SerializeField] private Image iconImage;
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
                debugLabel.text = incoming != null ? $"{incoming.SourceName}\n[{incoming.GetOutputResourceType()?.ResourceName}]" : "No linked source";

            if (iconImage != null)
                iconImage.sprite = incoming == null || incoming.resource == null ?  null : incoming.resource.Sprite;

            if (iconImage.sprite != null) {
                iconImage.color = new Color(1,1,1, 1);
                debugLabel.color = new Color(debugLabel.color.r, debugLabel.color.g, debugLabel.color.b, 0);
            }
            else {
                iconImage.color = new Color(0, 0, 0, 0);
                debugLabel.color = new Color(debugLabel.color.r, debugLabel.color.g, debugLabel.color.b, 1);
            }
        }

        public override MachineInstance BuildInstance()
        {
            return new ExternalInputInstance(_parentSource, _incomingSourceIndex, inventorySize, GetRootPosition());
        }
        public override Resource GetResource()
        {
            var incoming = _parentSource.IncomingSourceLinks[_incomingSourceIndex];
            if (incoming == null) {
                return null;
            }

            return incoming.resource;
        }
    }
}