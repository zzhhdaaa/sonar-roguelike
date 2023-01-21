using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float time = 0.1f;
    [SerializeField] private bool isPlayerTurn = true;

    [SerializeField] private int entityNum = 0;
    [SerializeField] private List<Entity> entities = new List<Entity>();

    public bool IsPlayerTurn { get { return isPlayerTurn; } }
    public List<Entity> Entities { get { return entities; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StartTurn()
    {
        if (entities[entityNum].GetComponent<Player>())
            isPlayerTurn = true; //is player turn
        else if (entities[entityNum].IsSentient)
            Action.SkipAction(entities[entityNum]); //is other entity turn
    }

    public void EndTurn()
    {
        if (entities[entityNum].GetComponent<Player>())
            isPlayerTurn = false; //stop player turn

        if (entityNum == entities.Count - 1)
            entityNum = 0; //rolling back to the start of the entity list
        else
            entityNum++; //next entity in the list

        StartCoroutine(TurnDelay()); //each turn has 0.1 sec delay, and then start next turn
    }

    private IEnumerator TurnDelay()
    {
        yield return new WaitForSeconds(time);
        StartTurn();
    }

    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
    }

    public void InsertEntity(Entity entity, int index)
    {
        entities.Insert(index, entity);
    }
}
