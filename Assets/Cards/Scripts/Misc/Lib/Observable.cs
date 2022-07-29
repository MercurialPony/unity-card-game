using System.Collections.Generic;

public class Observable : IObservable
{
    private List<IObserver> observers = new List<IObserver>();

    public void Subscribe(IObserver o)
    {
        this.observers.Add(o);
    }

    public void Unsubscribe(IObserver o)
    {
        this.observers.Remove(o);
    }

    public void Notify(string property)
    {
        foreach(IObserver o in this.observers)
        {
            o.Notify(this, property);
        }
    }
}