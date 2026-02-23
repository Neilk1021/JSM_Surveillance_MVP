using System;
using TMPro;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    public class MoneyCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;

        private void Start()
        {
            UpdateMoneyText(SurveillanceGameManager.instance.MoneyManager.Money);
            SurveillanceGameManager.instance.MoneyManager.MoneyChanged.AddListener(UpdateMoneyText);
        }

        private void OnDestroy()
        {
            SurveillanceGameManager.instance.MoneyManager.MoneyChanged.RemoveListener(UpdateMoneyText);
        }
        
        private void UpdateMoneyText(int newMoneyVal)
        {
            moneyText.text = $"{newMoneyVal:N0}";
        }
    }
}