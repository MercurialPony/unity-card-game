public interface IObservable
{
    void Subscribe(IObserver o);

    void Unsubscribe(IObserver o);

    void Notify(string property);
}