using System;
using System.Threading.Tasks;
using JSM.Surveillance.Game;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class SurveillanceShop : MonoBehaviour
    {
        private bool _buying = false;
        private static void CheckSourceDataValidity(SourceData data)
        {
            if (data == null) {
                throw new ArgumentException("Data was not provided.");
            }

            if (data.Source == null) {
                throw new ArgumentException("Data has no source");
            }
        }

        
        public bool BuySource(SourceData data)
        {
            if (_buying) return false;
            
            CheckSourceDataValidity(data);
            if (!SurveillanceGameManager.ChangeMoneyBy(-data.UpfrontCost))
            {
                return false;
            }

            BuyProcess(data);
            return true;
        }


        private async Task BuyProcess(SourceData data)
        {
            _buying = true;
            Source source = SurveillanceGameManager.SpawnSource(data);
            var task = source.PlacementProcess();
            await task;
            if (task.IsCompletedSuccessfully)
            {
                if (task.Result == false) {
                    SurveillanceGameManager.ChangeMoneyBy(data.UpfrontCost);
                    source.Destroy();
                }
            }

            _buying = false;
        }
    }
}