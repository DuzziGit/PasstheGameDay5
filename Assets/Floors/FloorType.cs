using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Floor", menuName = "ScriptableObjects/Floor", order = 1)]
public class FloorType : ScriptableObject
{
    public string FloorName;

    [Header("Enemy Settings")]

    public int minEnemies;
    public int maxEnemies;

    public GameObject[] enemies;

    [Header("decoration Settings")]
    public int minDec;
    public int maxDec;

    public GameObject[] decorations;

    [Header("Tiles")]

    public TileBase[] floorTile;


}
