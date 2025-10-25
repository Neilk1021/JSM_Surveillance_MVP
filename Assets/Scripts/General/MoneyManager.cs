using UnityEngine;

namespace JSM.Surveillance
{
    public class MoneyManager : MonoBehaviour
    {
        [SerializeField] private int money;

        public int Money => money;
        
        /// <summary>
        /// Changes the amount of money the player has. Player cannot have negative money.
        /// </summary>
        /// <param name="amnt">Amount to add or remove.</param>
        /// <returns>True if success, false if failure.</returns>
        public bool ChangeMoneyBy(int amnt)
        {
            if (money < amnt) {
                return false;
            }

            money -= amnt;
            return true;
        }
        
        /// <summary>
        /// Sets the amount of money the player has. Player cannot have negative money.
        /// </summary>
        /// <param name="amnt">New amount of money the player should have.</param>
        /// <returns>True if success, false if failure.</returns>
        public bool SetMoney(int amnt)
        {
            if (amnt < 0)
            {
                return false;
            }
            
            money = amnt;
            return true;
        }
    }
}