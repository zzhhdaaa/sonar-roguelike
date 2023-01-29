using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ConfusedEnemy : AI
{
    [SerializeField] private AI previousAI;
    [SerializeField] private int turnsRemaining;
    [SerializeField] private Vector3 baitPosition;

    public AI PreviousAI { get { return previousAI; } set { previousAI = value; } }
    public int TurnsRemaining { get { return turnsRemaining; } set { turnsRemaining = value; } }
    public Vector3 BaitTargetPosition { get { return baitPosition; } set { baitPosition = value; } }

    private void OnValidate()
    {
        AStar = GetComponent<AStar>();
    }

    public override void RunAI()
    {
        if (turnsRemaining <= 0)
        {
            //change the AI back
            UIManager.instance.AddMessage($"The {gameObject.name} is no longer confused.", "#ff0000");
            GetComponent<Actor>().AI = previousAI;
            GetComponent<Actor>().AI.RunAI();
            //Destroy(this);
        }
        else
        {
            Vector3Int targetPosition = MapManager.instance.FloorMap.WorldToCell(baitPosition);
            Vector3Int currentPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);

            if (MapManager.instance.IsValidBaitPosition(baitPosition) && targetPosition != currentPosition)
            {
                MoveAlongPath(targetPosition);
            }
            else
            {
                Vector2Int direction = Random.Range(0, 8) switch
                {
                    0 => new Vector2Int(0, 1),
                    1 => new Vector2Int(0, -1),
                    2 => new Vector2Int(1, 0),
                    3 => new Vector2Int(-1, 0),
                    4 => new Vector2Int(1, 1),
                    5 => new Vector2Int(1, y: -1),
                    6 => new Vector2Int(-1, 1),
                    7 => new Vector2Int(-1, -1),
                    _ => new Vector2Int(0, 0)
                };
                Action.MovementAction(GetComponent<Actor>(), direction);
            }

            turnsRemaining--;
        }
    }

    public override AIState SaveState() => new ConfusedState(
        type: "ConfusedEnemy",
        previousAI: previousAI,
        turnsRemaining: turnsRemaining
    );

    public void LoadState(ConfusedState state)
    {
        if (state.PreviousAI == "HostileEnemy")
        {
            previousAI = GetComponent<HostileEnemy>();
        }
        turnsRemaining = state.TurnsRemaining;
    }
}

[System.Serializable]
public class ConfusedState : AIState
{
    [SerializeField] private string previousAI;
    [SerializeField] private int turnsRemaining;
    public string PreviousAI
    {
        get => previousAI; set => previousAI = value;
    }
    public int TurnsRemaining
    {
        get => turnsRemaining; set => turnsRemaining = value;
    }

    public ConfusedState(string type = "", AI previousAI = null, int turnsRemaining = 0) : base(type)
    {
        this.previousAI = previousAI.GetType().ToString();
        this.turnsRemaining = turnsRemaining;
    }
}