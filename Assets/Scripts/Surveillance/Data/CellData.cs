namespace JSM.Surveillance.Surveillance.Data
{
    public class CellData
    {
        public int Id { get; private set; }
        public HalfEdgeData HalfEdge { get; private set; }
        
        public CellData(int id, HalfEdgeData halfEdge)
        {
            Id = id;
            HalfEdge = halfEdge;
        }
    }
}