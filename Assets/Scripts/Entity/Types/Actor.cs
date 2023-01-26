using System.Collections.Generic;
using UnityEngine;

public class Actor : Entity
{
    [SerializeField] private bool isAlive = true;
    [SerializeField] private int fieldOfViewRange = 8;
    [SerializeField] private List<Vector3Int> fieldOfView = new List<Vector3Int>();
    [SerializeField] private AI aI;

    private AdamMilVisibility adamMilVisibility;
    
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }
    public List<Vector3Int> FieldOfView { get { return fieldOfView; } }

    private void OnValidate()
    {
        if (GetComponent<AI>())
        {
            aI = GetComponent<AI>();
        }
    }

    private void Start()
    {
        AddToGameManager();
        if (GetComponent<Player>())
        {
            GameManager.instance.InsertActor(this, 0);
        }
        else
        {
            GameManager.instance.AddActor(this);
        }

        adamMilVisibility = new AdamMilVisibility();
        UpdateFieldOfView();
    }

    public void UpdateFieldOfView()
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);

        fieldOfView.Clear();
        adamMilVisibility.Compute(gridPosition, fieldOfViewRange, fieldOfView);

        if (GetComponent<Player>())
        {
            fieldOfViewRange = (int)SonarManager.instance.Distance;
            MapManager.instance.UpdateFogMap(fieldOfView);
            MapManager.instance.SetEntitiesVisibilities();
        }
    }
}
