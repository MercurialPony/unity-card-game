using UnityEngine;

public interface ICardFactory
{
    int GetId();

    int GetCost();

    Card Create();

    Card CreateUi(Transform t);
}
