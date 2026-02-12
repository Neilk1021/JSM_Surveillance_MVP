namespace Surveillance.TechTree
{
    public interface IUnlockable
    {
        bool IsUnlocked { get; }
        void Unlock();
    }
}