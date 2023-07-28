using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int id;
    public HashSet<Vector2Int> FloorTiles = new HashSet<Vector2Int>();
    public enum roomType
    {
        Start,
        Side
    }
    public Vector2Int RoomCenterPos { get; set; }

    public Vector2Int size;

    public List<Enemy> enemies;


    private bool playerDetected = false;

    public Room(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles, Vector2Int size)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
        this.size = size;
    }


    private void Update()
    {
        

    }

    private void FixedUpdate()
    {
        if (playerDetected)
            return;

        RaycastHit2D[] boxes = Physics2D.BoxCastAll(RoomCenterPos, size, 0, Vector2.zero);
        foreach (RaycastHit2D box in boxes)
        {
            if (!box)
                return;

            Player p = box.collider.GetComponent<Player>();

            if (p != null)
            {

                playerDetected = true;
                foreach (Enemy en in enemies)
                {
                    en.Activate(p.transform);
                }
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(new Vector3(RoomCenterPos.x, RoomCenterPos.y), new Vector3(size.x, size.y));
    }




}
