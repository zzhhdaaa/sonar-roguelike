﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

static public class Action
{
    static public void TakeStairsAction(Actor actor)
    {
        Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(actor.transform.position);
        string tileName = MapManager.instance.FloorMap.GetTile(pos).name;

        //if (tileName != MapManager.instance.UpStairsTile.name && tileName != MapManager.instance.DownStairsTile.name)
        //{
        //    UIManager.instance.AddMessage("There are no stairs here.", "#0da2ff");
        //    return;
        //}

        //if (SaveManager.instance.CurrentFloor == 1 && tileName == MapManager.instance.UpStairsTile.name)
        //{
        //    UIManager.instance.AddMessage("A mysterious force prevents you from going back.", "#0da2ff");
        //    return;
        //}

        SaveManager.instance.SaveGame();
        SaveManager.instance.CurrentFloor += tileName == MapManager.instance.UpStairsTile.name ? -1 : 1;

        if (SaveManager.instance.Save.Scenes.Exists(x => x.FloorNumber == SaveManager.instance.CurrentFloor))
        {
            SaveManager.instance.LoadScene(false);
        }
        else
        {
            GameManager.instance.Reset(false);
            MapManager.instance.GenerateDungeon();
        }

        UIManager.instance.AddMessage("You take the stairs.", "#0da2ff");
        UIManager.instance.SetDungeonFloorText(SaveManager.instance.CurrentFloor);

        SonarManager.instance.SonarDownGrade?.Invoke();
    }

    static public void PickupAction(Actor actor)
    {
        for (int i = 0; i < GameManager.instance.Entities.Count; i++)
        {
            if (GameManager.instance.Entities[i].GetComponent<Actor>() || actor.transform.position != GameManager.instance.Entities[i].transform.position)
            {
                continue;
            }

            if (GameManager.instance.Entities[i].GetComponent<Stair>())
            {
                GameManager.instance.Entities[i].GetComponent<Stair>().Consume(GameManager.instance.Actors[0]);
                Action.TakeStairsAction(actor);
                return;
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
                SonarAction(actor.transform.position, SonarManager.instance.Distance);
            return false;
        }
        else
        {
            MovementAction(actor, direction);
            if (isSonarActivate)
                SonarAction(actor.transform.position, SonarManager.instance.Distance);
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

    static public void SonarAction(Vector3 originPos, float distance)
    {
        SonarManager.instance.SonarDetect(originPos, distance);
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