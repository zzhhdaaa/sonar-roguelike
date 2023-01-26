using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The GameManager manages all turns and entities in the game. 
    /// It check each entity's status and allow them to play term-by-term
    /// </summary>
    public static GameManager instance;

    [Header("Time")]
    [SerializeField] private float baseTime = 0.1f;
    [SerializeField] private float delayTime;
    [SerializeField] private bool isPlayerTurn = true;

    [Header("Entities")]
    [SerializeField] private int actorNum = 0;
    [SerializeField] private List<Entity> entities = new List<Entity>();
    [SerializeField] private List<Actor> actors = new List<Actor>();

    [Header("Death")]
    [SerializeField] private Sprite deadSprite;

    public bool IsPlayerTurn { get { return isPlayerTurn; } }
    public List<Entity> Entities { get { return entities; } }
    public List<Actor> Actors { get { return actors; } }
    public Sprite DeadSprite { get { return deadSprite; } }

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
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = true;
        }
        else
        {
            if (actors[actorNum].GetComponent<HostileEnemy>())
            {
                actors[actorNum].GetComponent<HostileEnemy>().RunAI();
            }
            else
            {
                Action.SkipAction();
            }
        }
    }

    public void EndTurn()
    {
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = false; //stop player turn
        }

        if (actorNum == actors.Count - 1)
        {
            actorNum = 0; //rolling back to the start of the entity list
        }
        else
        {
            actorNum++; //next entity in the list
        }

        StartCoroutine(TurnDelay()); //each turn has 0.1 sec delay, and then start next turn
    }

    private IEnumerator TurnDelay()
    {
        yield return new WaitForSeconds(delayTime);
        StartTurn();
    }

    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        entities.Remove(entity);
    }

    public void AddActor(Actor actor)
    {
        actors.Add(actor);
        delayTime = SetTime();
    }

    public void InsertActor(Actor actor, int index)
    {
        actors.Insert(index, actor);
        delayTime = SetTime();
    }

    public void RemoveActor(Actor actor)
    {
        actors.Remove(actor);
        delayTime = SetTime();
    }

    public Actor GetBlockingActorAtLocation(Vector3 location)
    {
        foreach (Actor actor in actors)
        {
            if (actor.BlocksMovement && actor.transform.position == location)
            {
                return actor;
            }
        }
        return null;
    }

    private float SetTime() => baseTime / actors.Count;
}
