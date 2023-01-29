using System.Collections.Generic;
using UnityEngine;

static public class Action
{
    static public void PickupAction(Actor actor)
    {
        for (int i = 0; i < GameManager.instance.Entities.Count; i++)
        {
            if (GameManager.instance.Entities[i].GetComponent<Actor>() || actor.transform.position != GameManager.instance.Entities[i].transform.position)
            {
                continue;
            }

            if (actor.Inventory.Items.Count >= actor.Inventory.Capacity)
            {
                UIManager.instance.AddMessage($"Don't be so greedy.", "#FF0000");
                return;
            }

            Item item = GameManager.instance.Entities[i].GetComponent<Item>();
            actor.Inventory.Add(item);

            UIManager.instance.AddMessage($"You found a {item.name}!", "#FFFFFF");

            actor.PickupFeedbacks?.PlayFeedbacks();

            GameManager.instance.EndTurn();
        }
    }

    static public void DropAction(Actor actor, Item item)
    {
        actor.Inventory.Drop(item);

        UIManager.instance.ToggleDropMenu();
        GameManager.instance.EndTurn();
    }

    static public void UseAction(Actor consumer, Item item)
    {
        bool itemUsed = false;

        if (item.GetComponent<Consumable>())
        {
            itemUsed = item.GetComponent<Consumable>().Activate(consumer);
            consumer.PickupFeedbacks?.PlayFeedbacks();
        }

        UIManager.instance.ToggleInventory();

        if (itemUsed)
        {
            GameManager.instance.EndTurn();
        }
    }

    static public bool BumpAction(Actor actor, Vector2 direction, bool isSonarActivate = false)
    {
        //check if another actor is on the destination
        Actor target = GameManager.instance.GetActorAtLocation(actor.transform.position + (Vector3)direction);

        if (target)
        {
            MeleeAction(actor, target);
            if (isSonarActivate)
                SonarAction(actor.transform.position);
            return false;
        }
        else
        {
            MovementAction(actor, direction);
            if (isSonarActivate)
                SonarAction(actor.transform.position);
            return true;
        }
    }

    static public void MeleeAction(Actor actor, Actor target)
    {
        int damage = actor.GetComponent<Fighter>().Power - target.GetComponent<Fighter>().Defense;

        string attackDesc = $"{actor.name} attacks {target.name}";

        string colorHex = "";

        if (actor.GetComponent<Player>())
        {
            colorHex = "#ffffff";
        }
        else
        {
            colorHex = "#d1a3a4";
        }

        if (damage > 0)
        {
            //Debug.Log($"{attackDesc} for {damage} hit points.");
            UIManager.instance.AddMessage($"{attackDesc} for {damage} hit points.", colorHex);
            target.GetComponent<Fighter>().Hp -= damage;
        }
        else
        {
            //Debug.Log($"{attackDesc} but nothing happened.");
            UIManager.instance.AddMessage($"{attackDesc} but nothing happened.", colorHex);
        }

        actor.GetComponent<Fighter>().MeleeFeedbacks?.PlayFeedbacks();

        GameManager.instance.EndTurn();
    }

    static public void MovementAction(Actor actor, Vector2 direction)
    {
        actor.Move(direction);
        actor.UpdateFieldOfView();
        actor.MoveFeedbacks?.PlayFeedbacks();
        GameManager.instance.EndTurn();
    }

    static public void SonarAction(Vector3 originPos)
    {
        SonarManager.instance.SonarDetect(originPos);
    }

    static public void WaitAction()
    {
        GameManager.instance.EndTurn();
    }

    static public void CastAction(Actor consumer, Actor target, Consumable consumable)
    {
        bool castSuccess = consumable.Cast(consumer, target);

        if (castSuccess)
        {
            GameManager.instance.EndTurn();
        }
    }

    static public void CastAction(Actor consumer, List<Actor> targets, Consumable consumable)
    {
        bool castSuccess = consumable.Cast(consumer, targets);

        if (castSuccess)
        {
            GameManager.instance.EndTurn();
        }
    }
}