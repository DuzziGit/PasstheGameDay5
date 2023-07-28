using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//IF YOU DELETE IT THE WHOLE GENERATOR WILL STOP WORKING
public class SimpleRandomWalkGenerator : AbstractDungeonGenerator
{

    [SerializeField]
    protected SimpleRandomWalkSO walkParameters;
    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = randomWalk(walkParameters, new Vector2Int());
        mapVisualiser.Clear();
        mapVisualiser.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, mapVisualiser, true);
    }

    protected HashSet<Vector2Int> randomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralAlgorithms.simpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }

}
