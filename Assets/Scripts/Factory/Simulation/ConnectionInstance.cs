namespace JSM.Surveillance
{
    public class ConnectionInstance
    {
        public MachineInstance Start { get; private set; }
        public MachineInstance End { get; private set; }

        public ConnectionInstance(MachineInstance start, MachineInstance end)
        {
            Start = start;
            End = end;
        }
    }
}