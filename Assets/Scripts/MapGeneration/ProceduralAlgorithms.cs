using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralAlgorithms
{
    //We create an "agent" which moves in random directions. we repeat the moving for walkLenght amount of time. Then we return each position our "agent" has visited.
    public static HashSet<Vector2Int> simpleRandomWalk(Vector2Int startpos, int walklenght)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startpos);
        var previousPos = startpos;

        for (int i = 0; i < walklenght; i++)
        {
            var newPos = previousPos + Direction2D.GetRandomCardinalDiretion();
            path.Add(newPos);
            previousPos = newPos;
        }
        return path;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minimumHeight, int maxRoomCount) //This is BSP - the main algorithm I will be using to create the map, it functions by splitting the defined space into 2 randomly, I will use it to figure out where the rooms should be generated
    {
        Queue<BoundsInt> roomQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomQueue.Enqueue(spaceToSplit);
        int currentRooms = 0;
        while(roomQueue.Count > 0)
        {
            var room = roomQueue.Dequeue();
            if (room.size.y >= minimumHeight && room.size.x >= minWidth && currentRooms < maxRoomCount)
            {
                if (Random.value < 0.5) //We use Random.value to randomize the result so it doesnt always split horizontally first.
                {
                    if (room.size.y >= minimumHeight * 2)
                    {
                        SplitHorizontally(minWidth, roomQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVetically(minimumHeight, roomQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minimumHeight)
                    {
                        roomsList.Add(room);
                        currentRooms++;
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVetically(minWidth, roomQueue, room);
                    }
                    else if (room.size.y >= minimumHeight * 2)
                    {
                        SplitHorizontally( minimumHeight, roomQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minimumHeight)
                    {
                        roomsList.Add(room);
                        currentRooms++;
                    }
                }
                
            }
        }
        return roomsList;
    }

    private static void SplitVetically(int minWidth, Queue<BoundsInt> roomQueue, BoundsInt room) //Spliting the rooms Vertically
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minimumHeight, Queue<BoundsInt> roomQueue, BoundsInt room) //Spliting the rooms Horizontally
    {
        var YSplit = Random.Range(1, room.size.y); 
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x,YSplit, room.min.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x,room.min.y + YSplit, room.min.z), new Vector3Int(room.size.x, room.size.y - YSplit, room.size.z));
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }
}

//A list of 4 cardinal directions
public static class Direction2D
{


    public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>
    {

        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0)

    };

    public static List<Vector2Int> diagonalDirectionList = new List<Vector2Int>
    {

        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1)

    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>(){

        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(1,0),
        new Vector2Int(1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,-1)
    };


    public static Vector2Int GetRandomCardinalDiretion()
    {

        return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];



    }
}
