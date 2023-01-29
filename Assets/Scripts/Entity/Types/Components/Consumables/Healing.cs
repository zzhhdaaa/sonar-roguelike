using UnityEngine;

public class Healing : Consumable
{
    [SerializeField] private int amount = 0;
    
    public int Amount { get { return amount; } }

    public override bool Activate(Actor consumer)
    {
        int amountRecovered = consumer.GetComponent<Fighter>().Heal(amount);

        if (amountRecovered > 0)
        {
            UIManager.instance.AddMessage($"You use the {name}, and recover {amountRecovered} HP!", "#00FF00");
            Consume(consumer);
            return true;
        }
        else
        {
            UIManager.instance.AddMessage("What a waste.", "#00FF00");
            return false;
        }
    }
}
