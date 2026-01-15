using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance.UI
{
    [CreateAssetMenu(fileName = "ShopBank", menuName = "JSM/Surveillance/ShopBank")]
    public class ShopBank : ScriptableObject
    {
        [SerializeField] private List<MachineObject> buyableProcessors;

        public IReadOnlyList<MachineObject> BuyableProcessors => buyableProcessors;
    }
}