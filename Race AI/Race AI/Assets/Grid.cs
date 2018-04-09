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

    public UnityEngine.Object Obj;
    public Material StartMaterial;
    public Material FinnishMaterial;
    public Material PositionMaterial;
    public Material NeighbourMaterial;
    public Material DefaultMaterial;

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

    public int MaxSize
    {
        get { return _gridSizeX * _gridSizeY * _gridSizeZ; }
    }

    void Update()
    {
        UpdateGridColors();
    }

    private void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY, _gridSizeZ];
        var worldBottomLeft = transform.position -
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

                    _grid[x, y, z] = new Node(walkable, terrain, worldPoint, x, y, z, Obj);
                }
            }
        }
    }

    private void RenderGrid()
    {
        if (_grid == null) return;
        for (var x = 0; x < _gridSizeX; x++)
        {
            for (var y = 0; y < _gridSizeY; y++)
            {
                for (var z = 0; z < _gridSizeZ; z++)
                {
                    if (!_grid[x, y, z].Terrain) continue;
                    _grid[x, y, z].Mesh = PrefabUtility.InstantiatePrefab(Obj) as GameObject;
                    var mesh = _grid[x, y, z].Mesh;
                    if (mesh != null)
                        mesh.transform.position = _grid[x, y, z].WorldPosition;
                }
            }
        }
    }

    private void UpdateGridColors()
    {
        if (_grid == null) return;
        var AINode = NodeFromWorldPosition(AI.position + Vector3.down);
        var playerNode = NodeFromWorldPosition(Player.position + Vector3.down);


        foreach (var node in _grid)
        {
            if (!node.Terrain) continue;
            node.Mesh.GetComponent<Renderer>().material = DefaultMaterial;
            node.InRange = false;
        }

        foreach (var node in _grid)
        {
            if (playerNode != node || !node.Terrain) continue;
            var neighbours = GetNeighbours(node);
            node.Mesh.GetComponent<Renderer>().material = PositionMaterial;
            node.InRange = false;
            foreach (var neighbour in neighbours)
            {
                if (!neighbour.Terrain ||
                    !NodeFromWorldPosition(neighbour.WorldPosition + Vector3.up).Walkable) continue;
                neighbour.Mesh.GetComponent<Renderer>().material = FinnishMaterial;
                neighbour.InRange = true;
            }
        }

        foreach (var node in _grid)
        {
            if (AINode == node)
            {
                if (node.Terrain)
                {
                    node.Mesh.GetComponent<Renderer>().material = PositionMaterial;
                }
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        var percentX = (worldPosition.x) / GridWorldSize.x;
        var percentY = (worldPosition.z) / GridWorldSize.y;
        var percentZ = (worldPosition.y) / GridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        var x = Mathf.RoundToInt((_gridSizeX) * percentX);
        var y = Mathf.RoundToInt((_gridSizeY) * percentY);
        var z = Mathf.RoundToInt((_gridSizeZ) * percentZ);

        return _grid[x, y, z];
    }

    public List<Node> GetNeighbours(Node node)
    {
        var neighbours = new List<Node>();

        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }

                    var checkX = node.GridX + x;
                    var checkY = node.GridY + y;
                    var checkZ = node.GridZ + z;

                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY && checkZ >= 0 &&
                        checkZ < _gridSizeZ)
                    {
                        neighbours.Add(_grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbours;
    }

    public List<Node> Path;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.z, GridWorldSize.y));

        if (_grid == null) return;
        var AINode = NodeFromWorldPosition(AI.position);
        var playerNode = NodeFromWorldPosition(Player.position);
        foreach (var node in _grid)
        {
            Gizmos.color = (node.Walkable) ? Color.green : Color.red;

            if (Path != null)
            {
                if (Path.Contains(node))
                {
                    Gizmos.color = Color.black;
                }
            }

            if (playerNode == node || AINode == node)
            {
                Gizmos.color = Color.yellow;
            }
            Gizmos.DrawSphere(node.WorldPosition - new Vector3(0, 0, 0), 0.1f);
        }
    }
}