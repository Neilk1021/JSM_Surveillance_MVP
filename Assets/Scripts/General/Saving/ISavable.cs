namespace JSM.Surviellance.Saving
{
    public interface ISavable
    {
        public object CaptureState();
        public void LoadState(object state);
    }
}