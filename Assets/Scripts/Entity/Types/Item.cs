using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Entity
{
    [SerializeField] private Consumable consumable;
    public Consumable Consumable { get { return consumable; } }

    private void OnValidate()
    {
        if (GetComponent<Consumable>())
        {
            consumable = GetComponent<Consumable>();
        }
    }

    private void Start()
    {
        AddToGameManager();
    }
}
