using System;
using System.Threading.Tasks;

namespace JSM.Surviellance.Saving
{
    public interface ISavable
    {
        public object CaptureState();

        public Task LoadState(object state);

    }

    public class SaveFailedException : Exception
    {
        public SaveFailedException() { }

        public SaveFailedException(string message) : base(message) { }

        public SaveFailedException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class LoadFailedException : Exception
    {
        public LoadFailedException() {}
        
        public LoadFailedException(string message) : base(message) {}

        public LoadFailedException(string message, Exception innerException) : base(message, innerException) {}
    }
}