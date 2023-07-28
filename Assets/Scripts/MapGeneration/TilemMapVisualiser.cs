using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemMapVisualiser : MonoBehaviour
{
    public Tilemap floorTilemap, wallTilemap;


    [SerializeField]
    private TileBase[] wallTop, wallSideRight, wallSideLeft, WallBottom, wallFull, wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    private FloorType floorType;

    public void SetFloorType(FloorType ft)
    {
        floorType = ft;

    }

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {

        PaintTiles(floorPositions, floorTilemap, floorType.floorTile);



    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {

        int typeAsInt = Convert.ToInt32(binaryType, 2);

        TileBase[] tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;

        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;

        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;

        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = WallBottom;

        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;

        }


        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }



    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase[] tile)
    {

        foreach (var position in positions)
        {


            PaintSingleTile(tilemap, tile, position);



        }





    }

    private void PaintSingleTile(Tilemap tilemap, TileBase[] tile, Vector2Int position)
    {
        var tilePos = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePos, tile[Random.Range(0, tile.Length)]);
    }




    public void Clear()
    {

        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {

        int typeASInt = Convert.ToInt32(binaryType, 2);

        TileBase[] tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt))
        {

            tile = wallFull;

        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeASInt))
        {
            tile = WallBottom;
        }



















        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }








    }
}
