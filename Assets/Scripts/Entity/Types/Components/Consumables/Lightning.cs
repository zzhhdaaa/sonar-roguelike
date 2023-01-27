using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : Consumable
{
    [SerializeField] private int damage = 20;
    [SerializeField] private int maximumRange = 5;

    public int Damage { get { return damage; } }
    public int MaximumRange { get { return maximumRange; } }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage("Strike something!", "#0000ff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.instance.AddMessage($"A lightning strikes {damage} damage to {target.name}!", "#ff0000");
        target.GetComponent<Fighter>().Hp -= damage;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
