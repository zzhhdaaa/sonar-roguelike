using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The GameManager manages all turns and entities in the game. 
    /// It check each entity's status and allow them to play term-by-term
    /// </summary>
    public static GameManager instance;

    [Header("Time")]
    [SerializeField] private float baseTime;
    [SerializeField] private float delayTime;
    [SerializeField] private bool isPlayerTurn = true;

    [Header("Entities")]
    [SerializeField] private int actorNum = 0;
    [SerializeField] private List<Entity> entities;
    [SerializeField] private List<Actor> actors;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneState sceneState = SaveManager.instance.Save.Scenes.Find(x => x.FloorNumber == SaveManager.instance.CurrentFloor);

        if (sceneState is not null)
        {
            LoadState(sceneState.GameState, true);
        }
        else
        {
            entities = new List<Entity>();
            actors = new List<Actor>();
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
            if (actors[actorNum].AI != null)
            {
                actors[actorNum].AI.RunAI();
            }
            else
            {
                Action.WaitAction();
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

        StartCoroutine(TurnDelay(baseTime/actors.Count)); //each turn has 0.1 sec delay, and then start next turn
    }

    private IEnumerator TurnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTurn();
    }

    public void AddEntity(Entity entity)
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }
        entities.Add(entity);
    }
    public void InsertEntity(Entity entity, int index)
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }
        entities.Insert(index, entity);
    }

    public void RemoveEntity(Entity entity)
    {
        //entity.gameObject.SetActive(false);
        entities.Remove(entity);
    }

    public void AddActor(Actor actor)
    {
        actors.Add(actor);
        //delayTime = SetTime();
    }

    public void InsertActor(Actor actor, int index)
    {
        actors.Insert(index, actor);
        //delayTime = SetTime();
    }

    public void RemoveActor(Actor actor)
    {
        actors.Remove(actor);
        //delayTime = SetTime();
    }

    public void RefreshPlayer()
    {
        actors[0].UpdateFieldOfView();
    }

    public Actor GetActorAtLocation(Vector3 location)
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

    public GameState SaveState()
    {
        foreach (Item item in actors[0].Inventory.Items)
        {
            AddEntity(item);
        }

        GameState gameState = new GameState(entities: entities.ConvertAll(x => x.SaveState()));

        foreach (Item item in actors[0].Inventory.Items)
        {
            RemoveEntity(item);
        }

        return gameState;
    }

    public void LoadState(GameState state, bool canRemovePlayer)
    {
        isPlayerTurn = false; //Prevents player from moving during load

        Reset(canRemovePlayer);
        StartCoroutine(LoadEntityStates(state.Entities, canRemovePlayer));
    }

    private IEnumerator LoadEntityStates(List<EntityState> entityStates, bool canPlacePlayer)
    {
        int entityState = 0;
        while (entityState < entityStates.Count)
        {
            yield return new WaitForEndOfFrame();
            string entityName = entityStates[entityState].Name.Contains("Remains of") ?
            entityStates[entityState].Name.Substring(entityStates[entityState].Name.LastIndexOf(' ') + 1) : entityStates[entityState].Name;

            if (entityStates[entityState].Type == EntityState.EntityType.Actor)
            {
                ActorState actorState = entityStates[entityState] as ActorState;

                if (entityName == "Player" && !canPlacePlayer)
                {
                    actors[0].transform.position = entityStates[entityState].Position;
                    RefreshPlayer();
                    entityState++;
                    continue;
                }

                Actor actor = MapManager.instance.CreateEntity(entityName, actorState.Position).GetComponent<Actor>();

                actor.LoadState(actorState);
            }
            else if (entityStates[entityState].Type == EntityState.EntityType.Item)
            {
                ItemState itemState = entityStates[entityState] as ItemState;

                if (itemState.Parent == "Player" && !canPlacePlayer)
                {
                    entityState++;
                    continue;
                }

                Item item = MapManager.instance.CreateEntity(entityName, itemState.Position).GetComponent<Item>();

                item.LoadState(itemState);
            }

            entityState++;
        }
        isPlayerTurn = true; //Allows player to move after load
    }

    public void Reset(bool canRemovePlayer)
    {
        if (entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (!canRemovePlayer && entity.GetComponent<Player>())
                {
                    continue;
                }

                Destroy(entity.gameObject);
            }

            if (canRemovePlayer)
            {
                entities.Clear();
                actors.Clear();
            }
            else
            {
                entities.RemoveRange(1, entities.Count - 1);
                actors.RemoveRange(1, actors.Count - 1);
            }
        }
    }
}

[System.Serializable]
public class GameState
{
    [SerializeField] private List<EntityState> entities;

    public List<EntityState> Entities
    {
        get => entities; set => entities = value;
    }

    public GameState(List<EntityState> entities)
    {
        this.entities = entities;
    }
}