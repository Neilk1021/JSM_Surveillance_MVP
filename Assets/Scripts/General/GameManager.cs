using System;
using UnityEngine;

namespace JSM.Surveillance
{
    public class GameManager : MonoBehaviour
    {
        private Maintainable[] _machines;
        private UnlockedMachines _unlockedMachines;
        private MoneyManager _moneyManager;
        
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}