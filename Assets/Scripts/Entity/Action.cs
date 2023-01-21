using UnityEngine;

static public class Action
{
    static public void EscapeAction()
    {
        //Application.Quit();
        Debug.Log("Quit");
    }

    static public void MovementAction(Entity entity, Vector2 direction)
    {
        entity.Move(direction);
        GameManager.instance.EndTurn();
    }

    static public void SkipAction(Entity entity)
    {
        GameManager.instance.EndTurn();
    }
}