using UnityEngine;

namespace JSM.Surveillance
{
    public class SplitterObject : MachineObject
    {
        public Maintainable Data => data;
        [SerializeField] private Maintainable data;
        public override void Sell()
        {
            SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(data.UpfrontCost);
            base.Sell();
        }

        public override MachineInstance BuildInstance()
        {
            return new SplitterInstance(inventorySize, data);
        }


    }
}