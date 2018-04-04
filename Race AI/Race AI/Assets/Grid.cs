using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{


    public Transform AI;
    public Transform Player;
    public LayerMask BlockedMask;
    public Vector3 GridWorldSize;
    public float NodeRadius;
    private Node[,,] _grid;

    public UnityEngine.Object obj;
    public Material startMaterial;
    public Material finnishMaterial;
    public Material positionMaterial;
    public Material neighbourMaterial;
    public Material defaultMaterial;

    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY, _gridSizeZ;
    void Start()
    {
        _nodeDiameter = NodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);
        _gridSizeZ = Mathf.RoundToInt(GridWorldSize.z / _nodeDiameter);
        CreateGrid();
        RenderGrid();
    }

    void Update()
    {
        UpdateGridColors();
    }

    void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY, _gridSizeZ];
        Vector3 worldBottomLeft = transform.position - 
                                  Vector3.right * GridWorldSize.x / 2 -
                                  Vector3.forward * GridWorldSize.y / 2 -
                                  Vector3.up * GridWorldSize.z / 2;

        for (var x = 0; x < _gridSizeX; x++)
        {
            for (var y = 0; y < _gridSizeY; y++)
            {
                for (var z = 0; z < _gridSizeZ; z++)
                {
                    var worldPoint = worldBottomLeft + 
                                         Vector3.right * (x * _nodeDiameter + NodeRadius) +
                                         Vector3.forward * (y * _nodeDiameter + NodeRadius) +
                                         Vector3.up * (z * _nodeDiameter + NodeRadius);
                    var walkable = true;
                    var terrain = false;

                    if (Physics.CheckSphere(worldPoint, NodeRadius, BlockedMask))
                    {
                        walkable = false;
                        terrain = true;
                    }
                    else if (!(Physics.CheckSphere(worldPoint + Vector3.down, NodeRadius, BlockedMask)))
                    {
                        walkable = false;
                    }

                    _grid[x, y, z] = new Node(walkable, terrain, worldPoint,x,y,z, obj);
                }
            }
        }
    }

    void RenderGrid()
    {
        if (_grid != null)
        {
            for (var x = 0; x < _gridSizeX; x++)
            {
                for (var y = 0; y < _gridSizeY; y++)
                {
                    for (var z = 0; z < _gridSizeZ; z++)
                    {

                        if (_grid[x, y, z].Terrain)
                        {
                            _grid[x, y, z].mesh =  PrefabUtility.InstantiatePrefab(obj) as GameObject;
                            _grid[x, y, z].mesh.transform.position = _grid[x, y, z].WorldPosition;
                        }
                          

                    }
                }
            }
        }
    }

    void UpdateGridColors()
    {
        if (_grid != null)
        {

            Node AINode = NodeFromWorldPosition(AI.position + Vector3.down);
            Node playerNode = NodeFromWorldPosition(Player.position + Vector3.down);


            foreach (var node in _grid)
            {
                if (node.Terrain)
                {
                    node.mesh.GetComponent<Renderer>().material = defaultMaterial;
                    node.InRange = false;
                }
            }

            foreach (var node in _grid)
            {
                if (playerNode == node && node.Terrain)
                {
                    var neighbours = GetNeighbours(node);
                    node.mesh.GetComponent<Renderer>().material = positionMaterial;
                    node.InRange = false;
                    foreach (var neighbour in neighbours)
                    {
                        if (neighbour.Terrain && NodeFromWorldPosition(neighbour.WorldPosition + Vector3.up).Walkable)
                        {
                            neighbour.mesh.GetComponent<Renderer>().material = finnishMaterial;
                            neighbour.InRange = true;
                        }
                    }
                }
                

                
            }

            foreach (var node in _grid)
            {
                if (AINode == node)
                {
                    node.mesh.GetComponent<Renderer>().material = positionMaterial;
                }
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        var percentX = (worldPosition.x ) / GridWorldSize.x;
        var percentY = (worldPosition.z ) / GridWorldSize.y;
        var percentZ = (worldPosition.y ) / GridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((_gridSizeX) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY) * percentY);
        int z = Mathf.RoundToInt((_gridSizeZ) * percentZ);

        return _grid[x, y, z];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1 ; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x==0 && y==0 && z==0)
                    {
                        continue;
                    }

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX <_gridSizeX && checkY >= 0 && checkY < _gridSizeY && checkZ >= 0 && checkZ < _gridSizeZ)
                    {
                        neighbours.Add(_grid[checkX,checkY,checkZ]);
                    }
                }
            }
        }

        return neighbours;
    }

    public List<Node> path;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.z, GridWorldSize.y));

        if (_grid != null)
        {
            Node AINode = NodeFromWorldPosition(AI.position);
            Node playerNode = NodeFromWorldPosition(Player.position);
            foreach (var node in _grid)
            {
                Gizmos.color = (node.Walkable) ? Color.green : Color.red;

                if (path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }

                if (playerNode == node || AINode == node)
                {
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawSphere(node.WorldPosition - new Vector3(0,0,0), 0.1f);
            }
        }
    }
}
