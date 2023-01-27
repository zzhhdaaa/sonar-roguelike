using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Consumable
{
    [SerializeField] private int damage = 12;
    [SerializeField] private int radius = 3;

    public int Damage { get { return damage; } }
    public int Radius { get { return radius; } }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode(true, radius);
        UIManager.instance.AddMessage($"Time to create an Explosion!", "#0000ff");
        return false;
    }

    public override bool Cast(Actor consumer, List<Actor> targets)
    {
        foreach (Actor target in targets)
        {
            UIManager.instance.AddMessage($"The explosion caused {damage} damage to {target.name}!", "ff0000");
            target.GetComponent<Fighter>().Hp -= damage;
        }

        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
