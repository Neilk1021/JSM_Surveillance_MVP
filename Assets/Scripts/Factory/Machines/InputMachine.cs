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
                FactoryGrid.ActiveGrid.UIManager.SwitchUI(this);
            }

            base.OnMouseDown();
        }

        
        //TODO make it read from the game.
    }
}