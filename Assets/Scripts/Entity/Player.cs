using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
    /// <summary>
    /// The Player class listens to input events from Controls, and plays related Action.
    /// Here is the only Update method in the game.
    /// </summary>
    private Controls controls;
    [SerializeField] private bool moveKeyDown;
    [SerializeField] private bool targetMode;
    [SerializeField] private bool isSingleTarget;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject targetNormalObject;

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        SonarManager.instance.SonarDetect(transform.position); //detect sonar
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Player.Disable();
    }

    void Controls.IPlayerActions.OnMovement(InputAction.CallbackContext context)
    {
        if (context.started && GetComponent<Actor>().IsAlive)
        {
            moveKeyDown = true;
        }
        else if (context.canceled)
        {
            moveKeyDown = false;
        }
    }

    void Controls.IPlayerActions.OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (UIManager.instance.IsMenuOpen)
            {
                UIManager.instance.ToggleMenu();
            }
            else if (targetMode)
            {
                ToggleTargetMode();
            }
        }
    }

    public void OnHistory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!UIManager.instance.IsMenuOpen || UIManager.instance.IsMessageHistoryOpen)
            {
                UIManager.instance.ToggleMessageHistory();
            }
        }
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct())
            {
                Action.PickupAction(GetComponent<Actor>());
            }
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsInventoryOpen)
            {
                if (GetComponent<Inventory>().Items.Count > 0)
                {
                    UIManager.instance.ToggleInventory(GetComponent<Actor>());
                }
                else
                {
                    UIManager.instance.AddMessage("You have no items.", "#ffffff");
                }
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsDropMenuOpen)
            {
                if (GetComponent<Inventory>().Items.Count > 0)
                {
                    UIManager.instance.ToggleDropMenu(GetComponent<Actor>());
                }
                else
                {
                    UIManager.instance.AddMessage("You have no items.", "#ffffff");
                }
            }
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (targetMode)
            {
                if (isSingleTarget)
                {
                    Actor target = SingleTargetChecks(targetObject.transform.position);

                    if (target != null)
                    {
                        Action.CastAction(GetComponent<Actor>(), target, GetComponent<Inventory>().SelectedConsumable);
                    }
                }
                else
                {
                    List<Actor> targets = AreaTargetChecks(targetObject.transform.position);
                    Consumable consumable = GetComponent<Inventory>().SelectedConsumable;

                    consumable.gameObject.transform.position = targetObject.transform.position;

                    Action.CastAction(GetComponent<Actor>(), targets, consumable);
                    Action.SonarAction(targetObject.transform.position);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!UIManager.instance.IsMenuOpen)
        {
            if (GameManager.instance.IsPlayerTurn && moveKeyDown && GetComponent<Actor>().IsAlive)
                Move();
        }
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Vector3 futurePosition;

        if (targetMode)
        {
            futurePosition = targetObject.transform.position + (Vector3)roundedDirection;
        }
        else
        {
            futurePosition = transform.position + (Vector3)roundedDirection;
        }

        if (targetMode)
        {
            Vector3Int targetGridPosition = MapManager.instance.FloorMap.WorldToCell(futurePosition);
            targetObject.transform.position = futurePosition;
            moveKeyDown = true;
        }
        else
        {
            moveKeyDown = Action.BumpAction(GetComponent<Actor>(), roundedDirection, true); //if bump into an entity, moveKeyDown is set to false
        }
    }

    public void ToggleTargetMode(bool isArea = false, int radius = 1)
    {
        targetMode = !targetMode;

        if (targetMode)
        {
            if (targetObject.transform.position != transform.position)
            {
                targetObject.transform.position = transform.position;
            }

            if (isArea)
            {
                isSingleTarget = false;
                targetObject.transform.GetChild(0).GetComponent<SpriteRenderer>().size = Vector2.one * (radius * 2 + 1);
                targetObject.transform.GetChild(0).gameObject.SetActive(true);
                targetObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = GetComponent<Inventory>().SelectedConsumable.GetComponent<SpriteRenderer>().color;
                targetObject.GetComponent<SpriteRenderer>().color = GetComponent<Inventory>().SelectedConsumable.GetComponent<SpriteRenderer>().color;
            }
            else
            {
                isSingleTarget = true;
            }

            targetObject.SetActive(true);
            targetNormalObject.SetActive(false);
        }
        else
        {
            if (targetObject.transform.GetChild(0).gameObject.activeSelf)
            {
                targetObject.transform.GetChild(0).gameObject.SetActive(false);
            }

            targetObject.SetActive(false);
            targetNormalObject.SetActive(true);
            GetComponent<Inventory>().SelectedConsumable = null;
        }
    }

    private bool CanAct()
    {
        if (targetMode || UIManager.instance.IsMenuOpen || !GetComponent<Actor>().IsAlive)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Actor SingleTargetChecks(Vector3 targetPosition)
    {
        Actor target = GameManager.instance.GetActorAtLocation(targetPosition);

        if (target == null)
        {
            UIManager.instance.AddMessage("No target at location.", "#ffffff");
            return null;
        }

        if (target == GetComponent<Actor>())
        {
            UIManager.instance.AddMessage("Stupid choice.", "#ffffff");
            return null;
        }

        return target;
    }

    private List<Actor> AreaTargetChecks(Vector3 targetPosition)
    {
        int radius = (int)(targetObject.transform.GetChild(0).GetComponent<SpriteRenderer>().size.x - 1) / 2;

        Bounds targetBounds = new Bounds(targetPosition, Vector3.one * radius * 2);
        List<Actor> targets = new List<Actor>();

        foreach (Actor target in GameManager.instance.Actors)
        {
            if (targetBounds.Contains(target.transform.position))
            {
                targets.Add(target);
            }
        }

        if (targets.Count == 0)
        {
            UIManager.instance.AddMessage("No targets in the area.", "#ffffff");
            return targets;
        }

        return targets;
    }
}
