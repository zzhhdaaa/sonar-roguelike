using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

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
    [SerializeField] private TileBase upStairsTile;
    [SerializeField] private TileBase downStairsTile;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<Vector3Int> visibleTiles;
    [SerializeField] private List<RectangularRoom> rooms;
    [SerializeField] private Dictionary<Vector3Int, TileData> tiles;
    [SerializeField] private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();
    
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public TileBase FloorTile { get { return floorTile; } }
    public TileBase WallTile { get { return wallTile; } }
    public TileBase UpStairsTile { get { return upStairsTile; } }
    public TileBase DownStairsTile { get { return downStairsTile; } }
    public Tilemap FloorMap { get { return floorMap; } }
    public Tilemap ObstacleMap { get { return obstacleMap; } }
    public Tilemap FogMap { get { return fogMap; } }
    public List<RectangularRoom> Rooms { get { return rooms; } }
    public List<Vector3Int> VisibleTiles { get { return visibleTiles; } }
    public Dictionary<Vector2Int, Node> Nodes { get { return nodes; } set { nodes = value; } }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneState sceneState = SaveManager.instance.Save.Scenes.Find(x => x.FloorNumber == SaveManager.instance.CurrentFloor);

        if (sceneState is not null)
        {
            LoadState(sceneState.MapState);
        }
        else
        {
            GenerateDungeon(true);
        }
    }

    private void Start()
    {

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;

    }
    public void GenerateDungeon(bool isNewGame = false)
    {
        if (floorMap.cellBounds.size.x > 0)
        {
            Reset();
        }
        else
        {
            rooms = new List<RectangularRoom>();
            tiles = new Dictionary<Vector3Int, TileData>();
            visibleTiles = new List<Vector3Int>();
        }

        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, maxItemsPerRoom, rooms, isNewGame);

        AddTilemapToDictionary(floorMap);
        AddTilemapToDictionary(obstacleMap);
        SetupFogMap();

        if (!isNewGame)
        {
            GameManager.instance.RefreshPlayer();
        }
    }

    public bool InBounds(int x, int y) => (0 <= x) && (x < width) && (0 <= y) && (y < height);

    public GameObject CreateEntity(string entity, Vector2 position)
    {
        GameObject entityObject = Instantiate(Resources.Load<GameObject>($"{entity}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        entityObject.name = entity;
        return entityObject;
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

            TileData tile = new TileData(
                name: tilemap.GetTile(pos).name,
                isExplored: false,
                isVisible: false
            );
            tiles.Add(pos, tile);
        }
    }

    private void SetupFogMap()
    {
        foreach (Vector3Int pos in tiles.Keys)
        {
            if (!fogMap.HasTile(pos))
            {
                fogMap.SetTile(pos, fogTile);
                fogMap.SetTileFlags(pos, TileFlags.None);
            }

            if (tiles[pos].IsExplored)
            {
                fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
            }
            else
            {
                fogMap.SetColor(pos, Color.white);
            }
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

    private void Reset()
    {
        rooms.Clear();
        tiles.Clear();
        visibleTiles.Clear();
        nodes.Clear();

        floorMap.ClearAllTiles();
        obstacleMap.ClearAllTiles();
        fogMap.ClearAllTiles();
    }

    public MapState SaveState() => new MapState(tiles, rooms);

    public void LoadState(MapState mapState)
    {
        if (floorMap.cellBounds.size.x > 0)
        {
            Reset();
        }

        rooms = mapState.StoredRooms;
        tiles = mapState.StoredTiles.ToDictionary(x => new Vector3Int((int)x.Key.x, (int)x.Key.y, (int)x.Key.z), x => x.Value);
        if (visibleTiles.Count > 0)
        {
            visibleTiles.Clear();
        }

        foreach (Vector3Int pos in tiles.Keys)
        {
            if (tiles[pos].Name == floorTile.name)
            {
                floorMap.SetTile(pos, floorTile);
            }
            else if (tiles[pos].Name == wallTile.name)
            {
                obstacleMap.SetTile(pos, wallTile);
            }
            else if (tiles[pos].Name == upStairsTile.name)
            {
                floorMap.SetTile(pos, upStairsTile);
            }
            else if (tiles[pos].Name == downStairsTile.name)
            {
                floorMap.SetTile(pos, downStairsTile);
            }
        }
        SetupFogMap();
    }
}

[System.Serializable]
public class MapState
{
    [SerializeField] private Dictionary<Vector3, TileData> storedTiles;
    [SerializeField] private List<RectangularRoom> storedRooms;
    public Dictionary<Vector3, TileData> StoredTiles
    {
        get => storedTiles; set => storedTiles = value;
    }
    public List<RectangularRoom> StoredRooms
    {
        get => storedRooms; set => storedRooms = value;
    }

    public MapState(Dictionary<Vector3Int, TileData> tiles, List<RectangularRoom> rooms)
    {
        storedTiles = tiles.ToDictionary(x => (Vector3)x.Key, x => x.Value);
        storedRooms = rooms;
    }
}