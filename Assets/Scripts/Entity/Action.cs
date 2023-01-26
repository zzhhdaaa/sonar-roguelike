using UnityEngine;

static public class Action
{
    static public void EscapeAction()
    {
        //Application.Quit();
        Debug.Log("Quit");
    }

    static public bool BumpAction(Actor actor, Vector2 direction)
    {
        //check if another actor is on the destination
        Actor target = GameManager.instance.GetBlockingActorAtLocation(actor.transform.position + (Vector3)direction);

        if (target)
        {
            MeleeAction(actor, target);
            return false;
        }
        else
        {
            MovementAction(actor, direction);
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
        GameManager.instance.EndTurn();
    }

    static public void MovementAction(Actor actor, Vector2 direction)
    {
        actor.Move(direction);
        actor.UpdateFieldOfView();
        GameManager.instance.EndTurn();
    }

    static public void SkipAction()
    {
        GameManager.instance.EndTurn();
    }
}