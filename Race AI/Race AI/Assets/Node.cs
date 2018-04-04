using UnityEngine;

public class Node
{

    [HideInInspector]
    public bool Walkable;
    [HideInInspector]
    public bool Terrain;
    [HideInInspector]
    public bool InRange = false;
    [HideInInspector]
    public Vector3 WorldPosition;
    [HideInInspector]
    public int gridX;
    [HideInInspector]
    public int gridY;
    [HideInInspector]
    public int gridZ;

    [HideInInspector]
    public float gCost;
    [HideInInspector]
    public float hCost;
    [HideInInspector]
    public Node parent;

    public Object obj;
    public GameObject mesh;

    public Node(bool walkable, bool _terrain,  Vector3 worldPos, int _gridX, int _gridY, int _gridZ, Object _obj)
    {
        Walkable = walkable;
        Terrain = _terrain;
        WorldPosition = worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;

        obj = _obj;
       
    }

    public float fCost
    {
        get { return gCost + hCost; }
    }
}
