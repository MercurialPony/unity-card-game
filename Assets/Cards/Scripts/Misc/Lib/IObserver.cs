public interface IObserver
{
    void Notify(IObservable o, string property);
}