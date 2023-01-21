using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private bool isSentient = false;

    [SerializeField] private int fieldOfViewRange = 8;
    [SerializeField] private List<Vector3Int> fieldOfView;

    private AdamMilVisibility adamMilVisibility;

    public bool IsSentient { get { return isSentient; } }

    private void Start()
    {
        if (IsSentient)
        {
            if (GetComponent<Player>())
            {
                GameManager.instance.InsertEntity(this, 0);
            }
            else
            {
                GameManager.instance.AddEntity(this);
            }
        }

        fieldOfView = new List<Vector3Int>();
        adamMilVisibility = new AdamMilVisibility();
        UpdateFieldOfView();
    }

    public void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction;
    }

    public void UpdateFieldOfView()
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);

        fieldOfView.Clear();
        adamMilVisibility.Compute(gridPosition, fieldOfViewRange, fieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.instance.UpdateFogMap(fieldOfView);
            MapManager.instance.SetEntitiesVisibilities();
        }
    }
}
