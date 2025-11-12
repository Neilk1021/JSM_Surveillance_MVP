namespace JSM.Surveillance.Surveillance.Data
{
    public class HalfEdgeData
    {
        public int Id { get; private set; }
        public int TwinIndex { get; private set; }
        public int NextVertexIndex { get; private set; }
        public int LastVertexIndex { get; private set; }
        public int FaceIndex { get; private set; }
        
        
        public HalfEdgeData(int id, int twinIndex, int nextVertexIndex, int lastVertexIndex, int faceIndex)
        {
            Id = id;
            TwinIndex = twinIndex;
            NextVertexIndex = nextVertexIndex;
            LastVertexIndex = lastVertexIndex;
            FaceIndex = faceIndex;
        }
    }
}