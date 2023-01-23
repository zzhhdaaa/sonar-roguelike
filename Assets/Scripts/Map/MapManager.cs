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

    public TileBase FloorTile { get { return floorTile; } }
    public TileBase WallTile { get { return wallTile; } }
    public Tilemap FloorMap { get { return floorMap; } }
    public Tilemap ObstacleMap { get { return obstacleMap; } }
    public Tilemap FogMap { get { return fogMap; } }
    public List<RectangularRoom> Rooms { get { return rooms; } }


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
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, rooms);

        AddTilemapToDictionary(floorMap);
        AddTilemapToDictionary(obstacleMap);

        SetupFogMap();

        Instantiate(Resources.Load<GameObject>("NPC"), new Vector3(40 - 5.5f, 25 + 0.5f, 0), Quaternion.identity).name = "NPC";

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;

    }

    public bool InBounds(int x, int y) => (0 <= x) && (x < width) && (0 <= y) && (y < height);

    public void CreatePlayer(Vector2 position)
    {
        Instantiate(Resources.Load<GameObject>("Player"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Player";
    }

    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        foreach (Vector3Int pos in visibleTiles)
        {
            if (!tiles[pos].isExplored)
            {
                tiles[pos].isExplored = true;
            }

            tiles[pos].isVisible = false;
            fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }

        visibleTiles.Clear();

        foreach (Vector3Int pos in playerFOV)
        {
            tiles[pos].isVisible = true;
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
}
