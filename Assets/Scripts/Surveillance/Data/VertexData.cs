using UnityEngine;

namespace JSM.Surveillance.Surveillance.Data
{
    public class VertexData
    {
        public int Id { get; private set; }

        private float _x;
        private float _y;
        
        public Vector2 Position => new Vector2(_x, _y);

        public int HalfEdge { get; private set; }


        public VertexData(int id, Vector2 pos, int halfEdge) : this(id, pos.x, pos.y, halfEdge){}
        
        public VertexData(int id, float x, float y, int halfEdge)
        {
            Id = id;
            _x = x;
            _y = y;
            HalfEdge = halfEdge;
        }
    }
}