using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{   
    
    [SerializeField]
    protected TilemMapVisualiser mapVisualiser;
    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;

    public void GenerateDungeon()
    {
        mapVisualiser.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
    
}
