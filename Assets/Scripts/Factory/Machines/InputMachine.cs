using JSM.Surveillance.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace JSM.Surveillance
{
    public class InputMachine : MachineInstance
    {
        [SerializeField] private SourceData sourceData;
        [SerializeField] private Source source;

        public SourceData Data => sourceData;
        public Source Source => source;

        protected override void OnMouseDown()
        {
            if (Placed) {
                Grid.UIManager.SwitchUI(this);
            }

            base.OnMouseDown();
        }

        public void Initialize(Source newSource)
        {
            source = newSource;
            sourceData = newSource.Data;
        }

        
        //TODO make it read from the game.
    }
}