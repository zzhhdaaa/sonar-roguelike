using UnityEngine;

static public class Action
{
    static public void EscapeAction()
    {
        //Application.Quit();
        Debug.Log("Quit");
    }

    static public bool BumpAction(Entity entity, Vector2 direction)
    {
        //check if another entity is on the destination
        Entity target = GameManager.instance.GetBlockingEntityAtLocation(entity.transform.position + (Vector3)direction);

        if (target)
        {
            MeleeAction(target);
            return false;
        }
        else
        {
            MovementAction(entity, direction);
            return true;
        }
    }

    static public void MeleeAction(Entity target)
    {
        Debug.Log($"You kick the {target.name}, much to its annoyance!");
        GameManager.instance.EndTurn();
    }

    static public void MovementAction(Entity entity, Vector2 direction)
    {
        entity.Move(direction);
        entity.UpdateFieldOfView();
        GameManager.instance.EndTurn();
    }

    static public void SkipAction(Entity entity)
    {
        if (entity.GetComponent<Player>())
        {
            Debug.Log("You decided to skip your turn.");
        }
        else
        {
            Debug.Log($"The {entity.name} wonders when it will get to take a real turn.");
        }

        GameManager.instance.EndTurn();
    }
}