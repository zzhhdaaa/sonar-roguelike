using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

sealed class ProcGen
{
    /// <summary>
    /// The Procedural Generator generates each level of the game.
    /// Including placing rooms, creating tunnels, placing player and monsters.
    /// </summary>
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, int maxMonstersPerRoom, int maxItemsPerRoom, List<RectangularRoom> rooms, bool isNewGame)
    {
        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

            if (roomNum == 0)
            {
                roomWidth = 15;
                roomHeight = 15;
            }

            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            if (rooms.Count != 0)
            {
                float distance = Vector2.Distance(new Vector2(roomX, roomY), new Vector2(rooms[rooms.Count - 1].X, rooms[rooms.Count - 1].Y));
                float disTolerance = Mathf.Max((roomWidth + rooms[rooms.Count - 1].Width), (roomHeight + rooms[rooms.Count - 1].Height));
                if (distance > disTolerance * 1.4f || distance < disTolerance * 1.1f)
                {
                    continue;
                }
            }

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
                        //if is the boundary of the room, then set walls if the tile is empty
                        if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        //not boundary, then set floor tile
                        SetFloorTile(new Vector3Int(x, y));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                //Dig out a tunnel between this room and the previous one. And place monsters.
                TunnelBetween(rooms[rooms.Count - 1], newRoom);
                //if (Random.value > 0.75f && rooms.Count > 2)
                //{
                //    TunnelBetween(rooms[Random.Range(0, rooms.Count - 1)], newRoom);
                //}
                PlaceEntities(newRoom, maxMonstersPerRoom, maxItemsPerRoom);
            }
            else
            {
                //The first room, where the player starts.
                Vector3Int playerPos = (Vector3Int)newRoom.Center();

                //MapManager.instance.FloorMap.SetTile(playerPos, MapManager.instance.UpStairsTile);

                if (isNewGame)
                {
                    MapManager.instance.CreateEntity("Player", (Vector2Int)playerPos);
                }
                else
                {
                    GameManager.instance.Actors[0].GetComponent<Player>().StairUpFeedbacks?.PlayFeedbacks();
                    GameManager.instance.Actors[0].transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0);
                    GameManager.instance.Actors[0].GetComponent<Player>().StartCoroutine(GameManager.instance.Actors[0].GetComponent<Player>().WaitToBorn(0.8f));
                }

                //MapManager.instance.CreateEntity("Player", newRoom.Center());
                MapManager.instance.CreateEntity("Sonar Explosion", newRoom.Center() + new Vector2Int(4, 4));
                MapManager.instance.CreateEntity("Sonar Bait", newRoom.Center() + new Vector2Int(-4, -4));
                MapManager.instance.CreateEntity("Healing Potion", newRoom.Center() + new Vector2Int(4, -4));
                MapManager.instance.CreateEntity("Healing Potion", newRoom.Center() + new Vector2Int(-4, 4));
                //SonarManager.instance.SonarDetect((Vector3Int)newRoom.Center());
            }

            rooms.Add(newRoom);
        }

        //Add the stairs to the last room.
        MapManager.instance.CreateEntity("Stair", rooms[rooms.Count - 1].Center());
    }

    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnerCorner;

        tunnerCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y); //newx oldy

        //Generate the coordinates for this tunnel.
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnerCorner, tunnelCoords); //draw the first pixel line of L
        BresenhamLine.Compute(tunnerCorner, newRoomCenter, tunnelCoords); //draw the second pixel line of L

        //Set the tiles for this tunnel.
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            //Set the floor tile inside the tunnel
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

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
        if (MapManager.instance.FloorMap.GetTile(pos))
        {
            //if the tile already existed
            return true;
        }
        else
        {
            //if empty, set a wall tile
            MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.WallTile);
            return false;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        if (MapManager.instance.ObstacleMap.GetTile(pos))
        {
            //if obstacle tile already existed, clear it
            MapManager.instance.ObstacleMap.SetTile(pos, null);
        }

        MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);
    }

    private void PlaceEntities(RectangularRoom newRoom, int maximumMonsters, int maximumItems)
    {
        int numberOfMonsters = Random.Range(0, maximumMonsters + 1);
        int numberOfItems = Random.Range(0, maximumItems + 1);

        for (int monster = 0; monster < numberOfMonsters; monster++)
        {
            //random x & y for new monster
            int monsterX = Random.Range(newRoom.X + 1, newRoom.X + newRoom.Width - 1);
            int monsterY = Random.Range(newRoom.Y + 1, newRoom.Y + newRoom.Height - 1);

            for (int entity = 0; entity < GameManager.instance.Entities.Count; entity++)
            {
                //don't overlap any existing entity
                Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(GameManager.instance.Entities[entity].transform.position);

                if (pos.x == monsterX && pos.y == monsterY)
                {
                    return;
                }
            }

            if (Random.value < 0.8f)
            {
                MapManager.instance.CreateEntity("Orc", new Vector2(monsterX, monsterY));
            }
            else
            {
                MapManager.instance.CreateEntity("Troll", new Vector2(monsterX, monsterY));
            }
        }

        for (int item = 0; item < numberOfItems; item++)
        {
            //random x & y for new item
            int itemX = Random.Range(newRoom.X + 1, newRoom.X + newRoom.Width - 1);
            int itemY = Random.Range(newRoom.Y + 1, newRoom.Y + newRoom.Height - 1);

            for (int entity = 0; entity < GameManager.instance.Entities.Count; entity++)
            {
                //don't overlap any existing entity
                Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(GameManager.instance.Entities[entity].transform.position);

                if (pos.x == itemX && pos.y == itemY)
                {
                    return;
                }
            }

            float randomValue = Random.value;
            Debug.Log(randomValue);
            if (randomValue < 0.4f)
            {
                MapManager.instance.CreateEntity("Healing Potion", new Vector2(itemX, itemY));
            }
            else if (randomValue < 0.7f)
            {
                MapManager.instance.CreateEntity("Sonar Explosion", new Vector2(itemX, itemY));
            }
            else
            {
                MapManager.instance.CreateEntity("Sonar Bait", new Vector2(itemX, itemY));
            }
            item++;
        }
    }
}
