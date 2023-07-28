using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : SimpleRandomWalkGenerator
{
    [Header("The Big Stuff")]
   
    [SerializeField]
    private int dungeonWidth;
    [SerializeField]
    private int dungeonHeight;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField]
    private int maxRoomCount;

    [Header("Room Stuff")]
    
    [SerializeField]
    bool createBasicRooms = false;

    [SerializeField]
    private int minRoomWidth;
    [SerializeField]
    private int minRoomHeight;
    [SerializeField]
    private GameObject roomObject;

    [Header("FloorStuff")]

    [SerializeField]
    private FloorType floorType;


    [Header("Other")]

    [SerializeField]
    private GameObject playerObject;

    bool player_exist = false;


    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPos, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight, maxRoomCount);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        

        if(createBasicRooms == true)
            floor = CreateSimpleRooms(roomsList);
        else
            floor = CreateComplexRooms(roomsList);


        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        mapVisualiser.SetFloorType(floorType);
        mapVisualiser.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, mapVisualiser, true);
    }

    private HashSet<Vector2Int> CreateComplexRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {



            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = randomWalk(walkParameters, roomCenter);

            List<Vector2Int> posList = new List<Vector2Int>();

            foreach (var pos in roomFloor)
            {
                if (pos.x >= (roomBounds.xMin + offset) && pos.x <= (roomBounds.xMax - offset) && pos.y >= (roomBounds.yMin - offset) && pos.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(pos);
                    posList.Add(pos);
                }
            }

            //----Create-Room--------//
            GameObject roomGameObject = Instantiate(roomObject, new Vector3Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y),0), Quaternion.identity);
            Room roomScript = roomGameObject.GetComponent<Room>();
            roomScript.id = i+1;
            roomScript.RoomCenterPos = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            roomScript.size = new Vector2Int(roomsList[i].size.x - offset, roomsList[i].size.y - offset);
            //----Generate Decorations------------------//

            int numberOfDecorations = Random.Range(floorType.minDec, floorType.maxDec + 1);
            for (int j = 0; j < numberOfDecorations; j++)
            {
                Vector2Int decPos = posList[Random.Range(0, posList.Count)];

                GameObject decoration = Instantiate(floorType.decorations[Random.Range(0, floorType.decorations.Length)],
                    new Vector3(decPos.x, decPos.y), Quaternion.identity);
                decoration.transform.parent = roomGameObject.transform;
            }
            //----Generate Enemies-----------------------//

            if (player_exist)
            {
                int numberOfEnemies = Random.Range(floorType.minEnemies, floorType.maxEnemies + 1);
                GameObject[] enemies = new GameObject[numberOfEnemies + 1];
                for (int j = 0; j < enemies.Length; j++)
                {
                    Vector2Int enemyPos = posList[Random.Range(0, posList.Count)];
                    enemies[j] = Instantiate(floorType.enemies[Random.Range(0, floorType.enemies.Length)],
                        new Vector3(enemyPos.x,
                                    enemyPos.y), Quaternion.identity);
                    enemies[j].transform.parent = roomGameObject.transform;
                    roomScript.enemies.Add(enemies[j].GetComponent<Enemy>());


                }
            }

            CreatePlayer(roomGameObject.transform.position);

        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count >0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            HashSet<Vector2Int> newCorridor1 = CreateCorridor(new Vector2Int(currentRoomCenter.x - 1, currentRoomCenter.y), new Vector2Int(closest.x, closest.y - 1));
            HashSet<Vector2Int> newCorridor2 = CreateCorridor(new Vector2Int(currentRoomCenter.x + 1, currentRoomCenter.y), new Vector2Int(closest.x, closest.y + 1));
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
            corridors.UnionWith(newCorridor1);
            corridors.UnionWith(newCorridor2);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var pos = currentRoomCenter;
        corridor.Add(pos);
        while (pos.y != destination.y)
        {
            if(destination.y > pos.y)
            {
                pos += Vector2Int.up;
            }else if(destination.y < pos.y)
            {
                pos += Vector2Int.down;
            }
            corridor.Add(pos);
        }
        while (pos.x != destination.x)
        {
            if(destination.x > pos.x)
            {
                pos += Vector2Int.right;
            }else if (destination.x < pos.x)
            {
                pos += Vector2Int.left;
            }
            corridor.Add(pos);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var pos in roomCenters)
        {
            float currentDistance = Vector2.Distance(pos, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = pos;

            }
            
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        int id = 1;
        foreach (var room in roomsList)
        {
            

            HashSet<Vector2Int> roomfloor = new HashSet<Vector2Int>();
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                    roomfloor.Add(position);
                }
            }
            GameObject roomGameObject = Instantiate(roomObject, new Vector3Int((int)room.center.x, (int)room.center.y), Quaternion.identity);
            Room roomScript = roomGameObject.GetComponent<Room>();
            roomScript.id = id;
            id++;
            roomScript.RoomCenterPos = new Vector2Int((int)room.center.x, (int)room.center.y);
            roomScript.size = new Vector2Int(room.size.x - offset, room.size.y - offset);

            //----Generate Decorations------------------//

            int numberOfDecorations = Random.Range(floorType.minDec, floorType.maxDec + 1);
            for (int i = 0; i < numberOfDecorations; i++)
            {
                GameObject decoration = Instantiate(floorType.decorations[Random.Range(0, floorType.decorations.Length)], 
                    new Vector3(roomGameObject.transform.position.x + Random.Range(-(room.size.x) / 2 + offset, (room.size.x) / 2 - offset),
                                roomGameObject.transform.position.y + Random.Range(-(room.size.y) / 2 + offset, (room.size.y) / 2 - offset)), Quaternion.identity);
                decoration.transform.parent = roomGameObject.transform;
            }
            //----Generate Enemies-----------------------//

            if (player_exist)
            {
                int numberOfEnemies = Random.Range(floorType.minEnemies, floorType.maxEnemies + 1);
                GameObject[] enemies = new GameObject[numberOfEnemies + 1];
                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i] = Instantiate(floorType.enemies[Random.Range(0, floorType.enemies.Length)],
                        new Vector3(roomGameObject.transform.position.x + Random.Range(-(room.size.x) / 2 + offset, (room.size.x) / 2 - offset),
                                    roomGameObject.transform.position.y + Random.Range(-(room.size.y) / 2 + offset, (room.size.y) / 2 - offset)), Quaternion.identity);
                    enemies[i].transform.parent = roomGameObject.transform;
                    enemies[i].transform.parent = roomGameObject.transform;
                    roomScript.enemies.Add(enemies[i].GetComponent<Enemy>());

                }
            }

            CreatePlayer(roomGameObject.transform.position);

        }
        return floor;
    }
    private void CreatePlayer(Vector2 pos)
    {
        if (!player_exist)
        {
            Instantiate(playerObject, pos, Quaternion.identity);
            player_exist = true;
        }
    }
}
