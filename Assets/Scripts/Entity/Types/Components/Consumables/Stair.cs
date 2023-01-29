using UnityEngine;

public class Stair : Consumable
{
    public override bool Activate(Actor consumer)
    {
        return true;
    }
}
