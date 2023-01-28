using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    /// <summary>
    /// The MapManager handles all tilemaps and tiles in the game. 
    /// It continuously update the "explored" and "now visible" area. 
    /// And thus handles whether a tile isExplored or isVisible, or an entity isVisible. 
    /// </summary>
    public static MapManager instance;

    [Header("Map Settings")]
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;
    [SerializeField] private int roomMaxSize = 10;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;
    [SerializeField] private int maxMonstersPerRoom = 2;
    [SerializeField] private int maxItemsPerRoom = 2;

    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase fogTile;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<Vector3Int> visibleTiles = new List<Vector3Int>();
    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();
    [SerializeField] private Dictionary<Vector3Int, TileData> tiles = new Dictionary<Vector3Int, TileData>();
    [SerializeField] private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();
    
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public TileBase FloorTile { get { return floorTile; } }
    public TileBase WallTile { get { return wallTile; } }
    public Tilemap FloorMap { get { return floorMap; } }
    public Tilemap ObstacleMap { get { return obstacleMap; } }
    public Tilemap FogMap { get { return fogMap; } }
    public List<RectangularRoom> Rooms { get { return rooms; } }
    public Dictionary<Vector2Int, Node> Nodes { get { return nodes; } set { nodes = value; } }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, maxItemsPerRoom, rooms);

        AddTilemapToDictionary(floorMap);
        AddTilemapToDictionary(obstacleMap);

        SetupFogMap();

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;

    }

    public bool InBounds(int x, int y) => (0 <= x) && (x < width) && (0 <= y) && (y < height);

    public void CreateEntity(string entity, Vector2 position)
    {
        switch (entity)
        {
            case "Player":
                Instantiate(Resources.Load<GameObject>("Player"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Player";
                break;
            case "Orc":
                Instantiate(Resources.Load<GameObject>("Orc"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Orc";
                break;
            case "Troll":
                Instantiate(Resources.Load<GameObject>("Troll"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Troll";
                break;
            case "Healing Potion":
                Instantiate(Resources.Load<GameObject>("Healing Potion"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Healing Potion";
                break;
            case "Sonar Explosion":
                Instantiate(Resources.Load<GameObject>("Sonar Explosion"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Sonar Explosion";
                break;
            case "Sonar Bait":
                Instantiate(Resources.Load<GameObject>("Sonar Bait"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Sonar Bait";
                break;
            case "Lightning":
                Instantiate(Resources.Load<GameObject>("Lightning"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Lightning";
                break;
            default:
                Debug.Log("Entity not found.");
                break;
        }
    }

    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        foreach (Vector3Int pos in visibleTiles)
        {
            if (!tiles[pos].IsExplored)
            {
                tiles[pos].IsExplored = true;
            }

            tiles[pos].IsVisible = false;
            fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }

        visibleTiles.Clear();

        foreach (Vector3Int pos in playerFOV)
        {
            tiles[pos].IsVisible = true;
            fogMap.SetColor(pos, Color.clear);
            visibleTiles.Add(pos);
        }
    }

    public void SetEntitiesVisibilities()
    {
        foreach (Entity entity in GameManager.instance.Entities)
        {
            if (entity.GetComponent<Player>())
            {
                continue;
            }

            Vector3Int entityPosition = floorMap.WorldToCell(entity.transform.position);

            if (visibleTiles.Contains(entityPosition))
            {
                entity.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                entity.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void AddTilemapToDictionary(Tilemap tilemap)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
            {
                continue;
            }

            TileData tile = new TileData();
            tiles.Add(pos, tile);
        }
    }

    private void SetupFogMap()
    {
        foreach (Vector3Int pos in tiles.Keys)
        {
            fogMap.SetTile(pos, fogTile);
            fogMap.SetTileFlags(pos, TileFlags.None);
        }
    }

    public bool IsValidPosition(Vector3 futurePosition)
    {
        Vector3Int gridPosition = floorMap.WorldToCell(futurePosition);

        if (!InBounds(gridPosition.x, gridPosition.y) || obstacleMap.HasTile(gridPosition))
            return false;

        return true;
    }

    public bool IsValidBaitPosition(Vector3 baitPosition)
    {
        Vector3Int gridPosition = floorMap.WorldToCell(baitPosition);

        if (floorMap.HasTile(gridPosition))
            return true;

        return false;
    }
}
