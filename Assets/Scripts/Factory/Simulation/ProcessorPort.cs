namespace JSM.Surveillance
{
    public class ProcessorPort
    {
        private MachineInstance _owner;

        public ConnectionInstance Connection { get; }

        private NodeType _type;
        
        public ProcessorPort(MachineInstance owner, ConnectionInstance connectionInstance, NodeType type)
        {
            _owner = owner;
            _type = type;
            Connection = connectionInstance;
        }
    }
}