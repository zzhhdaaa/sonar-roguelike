using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Consumable : MonoBehaviour
{
    public virtual bool Activate(Actor consumer) => false;
    public virtual bool Cast(Actor consumer, Actor target) => false;
    public virtual bool Cast(Actor consumer, List<Actor> targets) => false;

    public void Consume(Actor consumer)
    {
        if (consumer.GetComponent<Inventory>().SelectedConsumable == this)
        {
            consumer.GetComponent<Inventory>().SelectedConsumable = null;
        }
        consumer.Inventory.Items.Remove(GetComponent<Item>());
        Destroy(gameObject);
    }
}
