using UnityEngine;

namespace JSM.Surveillance
{
    public class SplitterObject : MachineObject
    {
        public Maintainable Data => data;
        [SerializeField] private Maintainable data;
        

        protected override void OnMouseDown()
        {
            if (Placed) {
                Grid.UIManager.SwitchUI(this);
            }

            base.OnMouseDown();
        }

        
        public override void Sell()
        {
            SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(data.UpfrontCost);
            base.Sell();
        }

        public override MachineInstance BuildInstance()
        {
            return new SplitterInstance(inventorySize, data, GetRootPosition());
        }

        public override string GetMachineName()
        {
            return data.ShopInfo.name;
        }

        public override int GetMachineCost()
        {
            return data.UpfrontCost;
        }

        public override string GetMachineDesc()
        {
            return data.ShopInfo.desc;
        }
    }
}