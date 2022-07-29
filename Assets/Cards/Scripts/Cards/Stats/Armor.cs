using System;
using UnityEngine;

[Serializable]
public class Armor
{
    public string name { get; private set; } = "default";

    [field: SerializeField]
    public int defense { get; private set; }

    public Armor(string name, int amount)
    {
        this.name = name;
        this.defense = amount;
    }

    public int AbsorbDamage(int damage)
    {
        this.defense -= damage;

        if(this.defense < 0)
        {
            int damageTaken = this.defense * -1;
            this.defense = 0;
            return damageTaken;
        }

        return 0;
    }

    public object Clone()
    {
        return new Armor(this.name, this.defense);
    }
}
