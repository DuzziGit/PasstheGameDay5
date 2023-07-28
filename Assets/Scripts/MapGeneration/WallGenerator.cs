using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public static class WallGenerator
{
    public static HashSet<HashSet<Vector2Int>> CreateWalls(HashSet<Vector2Int> floorPositions, TilemMapVisualiser tileMapVisualiser, bool paint)
    {

        HashSet<Vector2Int> basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionList);
        HashSet<Vector2Int> cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionList);
        CreateBasicWall(tileMapVisualiser, basicWallPositions, floorPositions, paint);
        CreateCornerWalls(tileMapVisualiser, cornerWallPositions, floorPositions, paint);
        HashSet<HashSet<Vector2Int>> set = new HashSet<HashSet<Vector2Int>>();
        set.Add(basicWallPositions);
        set.Add(cornerWallPositions);

        return set;

    }

    private static void CreateCornerWalls(TilemMapVisualiser tileMapVisualiser, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions, bool paint)
    {
        foreach (var position in cornerWallPositions)
        {
            string neigboursBinary = "";

            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neigbourPos = position + direction;
                if (floorPositions.Contains(neigbourPos))
                {
                    neigboursBinary += "1";


                }
                else
                {
                    neigboursBinary += "0";
                }


            }
            if (paint == true)
            {
                tileMapVisualiser.PaintSingleCornerWall(position, neigboursBinary);
            }
        }



    }

    private static void CreateBasicWall(TilemMapVisualiser tileMapVisualiser, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions, bool paint)
    {

        foreach (var position in basicWallPositions)
        {

            string neighboursBinary = "";

            foreach (var dir in Direction2D.cardinalDirectionList)
            {
                var neighbourPosition = position + dir;
                if (floorPositions.Contains(neighbourPosition))
                {


                    neighboursBinary += "1";


                }
                else
                {

                    neighboursBinary += "0";
                }
            }

            if (paint == true)
            {
                tileMapVisualiser.PaintSingleBasicWall(position, neighboursBinary);
            }
        }


    }


    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {

        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {

            foreach (var direction in directionList)
            {

                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                {
                    wallPositions.Add(neighbourPosition);



                }


            }


        }
        return wallPositions;

    }










}
