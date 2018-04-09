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
    public int GridX;
    [HideInInspector]
    public int GridY;
    [HideInInspector]
    public int GridZ;

    [HideInInspector]
    public float GCost;
    [HideInInspector]
    public float HCost;
    [HideInInspector]
    public Node Parent;

    public Object Obj;
    public GameObject Mesh;

    public Node(bool walkable, bool terrain,  Vector3 worldPos, int gridX, int gridY, int gridZ, Object obj)
    {
        Walkable = walkable;
        Terrain = terrain;
        WorldPosition = worldPos;
        GridX = gridX;
        GridY = gridY;
        GridZ = gridZ;

        Obj = obj;
       
    }

    public float FCost
    {
        get { return GCost + HCost; }
    }
}
