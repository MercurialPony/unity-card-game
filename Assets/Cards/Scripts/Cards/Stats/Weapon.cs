using System;
using UnityEngine;

[Serializable]
public class Weapon : ICloneable
{
    public string name { get; private set; } = "default";

    [field: SerializeField]
    public int damage { get; private set; }

    public Weapon(string name, int damage) // TODO: sound
    {
        this.name = name;
        this.damage = damage;
    }

    public void Attack(Card target)
    {
        target.Damage(this.damage); // TODO play sound
    }

    public object Clone()
    {
        return new Weapon(this.name, this.damage);
    }
}