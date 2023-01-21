using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class ProcGen : MonoBehaviour
{
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, List<RectangularRoom> rooms)
    {
        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            if (newRoom.Overlaps(rooms))
            {
                //if overlap exists, just skip
                continue;
            }

            //Dig out this room's inner area and build the walls
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        //if is the boundary of the room, set walls
                        if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
                        {
                            //if tile already existed, skip
                            continue;
                        }
                    }
                    else
                    {
                        //if not the boundary of the room, set floors
                        if (MapManager.instance.ObstacleMap.GetTile(new Vector3Int(x, y, 0)))
                        {
                            //if tile already existed, clear it
                            MapManager.instance.ObstacleMap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                        //set floors
                        MapManager.instance.FloorMap.SetTile(new Vector3Int(x, y, 0), MapManager.instance.FloorTile);
                    }
                }
            }

            if (MapManager.instance.Rooms.Count == 0)
            {
                //The first room, where the player starts.
                MapManager.instance.CreatePlayer(newRoom.Center());
            }
            else
            {
                //Dig out a tunnel between this room and the previous one.
                TunnelBetween(MapManager.instance.Rooms[MapManager.instance.Rooms.Count - 1], newRoom);
            }

            rooms.Add(newRoom);
        }
    }

    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnerCorner;

        //decide the corner for the L shape tunnel
        if (Random.value < 0.5f)
        {
            tunnerCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y); //newx oldy
        }
        else
        {
            tunnerCorner= new Vector2Int(oldRoomCenter.x, newRoomCenter.y); //oldx newy
        }

        //Generate the coordinates for this tunnel.
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine(oldRoomCenter, tunnerCorner, tunnelCoords); //draw the first pixel line
        BresenhamLine(tunnerCorner, newRoomCenter, tunnelCoords); //draw the second pixel line

        //Set the tiles for this tunnel.
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            if (MapManager.instance.ObstacleMap.HasTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0)))
            {
                MapManager.instance.ObstacleMap.SetTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0), null);
            }

            //Set the floor tile.
            MapManager.instance.FloorMap.SetTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0), MapManager.instance.FloorTile);

            //Set the wall tiles around this tile to be walls.
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }

    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.instance.FloorMap.GetTile(new Vector3Int(pos.x, pos.y, 0)))
        {
            //if the tile already existed
            return true;
        }
        else
        {
            //if empty, set a wall tile
            MapManager.instance.ObstacleMap.SetTile(new Vector3Int(pos.x, pos.y, 0), MapManager.instance.WallTile);
            return false;
        }
    }

    private void BresenhamLine(Vector2Int roomCenter, Vector2Int tunnelCorner, List<Vector2Int> tunnelCoords)
    {
        //pixelly connect two points
        int x = roomCenter.x, y = roomCenter.y;
        int dx = Mathf.Abs(tunnelCorner.x - roomCenter.x), dy = Mathf.Abs(tunnelCorner.y - roomCenter.y);
        int sx = roomCenter.x < tunnelCorner.x ? 1 : -1, sy = roomCenter.y < tunnelCorner.y ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            tunnelCoords.Add(new Vector2Int(x, y));
            if (x == tunnelCorner.x && y == tunnelCorner.y)
            {
                break;
            }
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }
}
