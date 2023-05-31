using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraBoundaryBox : MonoBehaviour
{
    public float xUpperBounds;
    public float xLowerBounds;
    public float zUpperBounds;
    public float zLowerBounds;

    public Tilemap playableMap;
    public TileBase testTile;

    private void Start()
    {
        if (playableMap != null)
        {
            playableMap.ResizeBounds();
            playableMap.CompressBounds();
            Debug.Log(playableMap.localBounds);
            Debug.Log("Tilemap size: " + playableMap.ContainsTile(testTile));
            Bounds bounds = playableMap.localBounds;
            xLowerBounds = bounds.min.x;
            xUpperBounds = bounds.max.x;
            zLowerBounds = bounds.min.z;
            zUpperBounds = bounds.max.z;
            Debug.Log(bounds);
        }
    }
}
