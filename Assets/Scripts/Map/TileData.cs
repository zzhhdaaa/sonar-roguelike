using UnityEngine;

[System.Serializable]
public class TileData
{
    [SerializeField] private bool isExplored, isVisible;

    public bool IsExplored { get { return isExplored; } set { isExplored = value; } }
    public bool IsVisible { get { return isVisible; } set { isVisible = value; } }
}
