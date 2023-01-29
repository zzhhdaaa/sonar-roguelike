using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confusion : Consumable
{
    [SerializeField] private int numberOfTurns = 10;
    [SerializeField] private int radius = 3;

    public int NumberOfTurns { get { return numberOfTurns; } }
    public int Radius { get { return radius; } }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode(true, radius);
        UIManager.instance.AddMessage($"Select to place the Sonar Bait.", "#0000ff");
        return false;
    }

    public override bool Cast(Actor consumer, List<Actor> targets)
    {
        foreach (Actor target in targets)
        {
            if (target.GetComponent<Player>())
            {
                continue;
            }

            //if (target.TryGetComponent(out ConfusedEnemy confusedEnemy))
            //{
            //    if (confusedEnemy.TurnsRemaining > 0)
            //    {
            //        UIManager.instance.AddMessage($"The {target.name} is already confused.", "#0000ff");
            //        consumer.GetComponent<Inventory>().SelectedConsumable = null;
            //        consumer.GetComponent<Player>().ToggleTargetMode();
            //        return false;
            //    }
            //}
            //else
            //{
            //    confusedEnemy = target.gameObject.AddComponent<ConfusedEnemy>();
            //}

            ConfusedEnemy confusedEnemy = target.gameObject.GetComponent<ConfusedEnemy>();
            //confusedEnemy.PreviousAI = target.AI;
            confusedEnemy.TurnsRemaining = NumberOfTurns;
            confusedEnemy.BaitTargetPosition = transform.position;

            UIManager.instance.AddMessage($"\"Who's there!\" the {target.name} yells.", "#0000ff");
            target.AI = confusedEnemy;
        }
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
