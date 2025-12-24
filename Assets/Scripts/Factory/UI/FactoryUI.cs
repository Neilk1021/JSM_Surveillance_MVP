using UnityEngine;

namespace JSM.Surveillance.UI
{
    public abstract class FactoryUI : MonoBehaviour
    {
        public abstract void Initialize(CellOccupier occupier);
    }
}