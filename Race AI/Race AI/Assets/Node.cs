using UnityEngine;

public class Node
{

    public bool Walkable;
    public Vector3 WorldPosition;
    public int gridX;
    public int gridY;
    public int gridZ;

    public float gCost;
    public float hCost;
    public Node parent;

    public Node(bool walkable, Vector3 worldPos, int _gridX, int _gridY, int _gridZ)
    {
        Walkable = walkable;
        WorldPosition = worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;
    }

    public float fCost
    {
        get { return gCost + hCost; }
    }
}
