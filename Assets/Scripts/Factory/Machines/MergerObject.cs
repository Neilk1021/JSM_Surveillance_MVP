using UnityEngine;

namespace JSM.Surveillance
{
    public class MergerObject : MachineObject
    {
        [SerializeField] private Maintainable data;
        public Maintainable Data => data;

        public override void Sell()
        {
            SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(data.UpfrontCost);
            base.Sell();
        }

        public override MachineInstance BuildInstance()
        {
            return new MergerInstance(inventorySize, data);
        }


    }
}