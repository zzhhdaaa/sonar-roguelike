using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Consumable;
using static UnityEditor.Progress;

public class Stair : Consumable
{
    public override bool Activate(Actor consumer)
    {
        return true;
    }
}
