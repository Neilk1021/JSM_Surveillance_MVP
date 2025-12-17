using UnityEngine;

namespace JSM.Surveillance
{
    public class FactoryCell
    {
        public FactoryCell(Vector2Int pos)
        {
            _pos = pos;
        }

        public FactoryCell(int x, int y)
        {
            _pos = new Vector2Int(x, y);
        }
        
        
        private Vector2Int _pos;
        private CellOccupier _occupier = null;

        public bool IsOccupied => _occupier != null;

        public void Clear()
        {
            if(!IsOccupied){return;}

            _occupier.Clear();
            _occupier = null;
        }

        public bool SetOccupier(CellOccupier occupier)
        {
            if (_occupier != null)
            {
                return false;
            }

            _occupier = occupier;
            return true;

        }

        public void ExitHover()
        {
            _occupier?.Exited();
        }

        public void EnterHover()
        {
            _occupier?.Entered();
        }
    }
}