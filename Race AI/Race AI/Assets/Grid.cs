using System.Collections;
using System.Collections.Generic;
using System.Xml.Xsl;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask BlockedMask;
    public Vector3 GridWorldSize;
    public float NodeRadius;
    private Node[,,] _grid;

    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY, _gridSizeZ;
    void Start()
    {
        _nodeDiameter = NodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);
        _gridSizeZ = Mathf.RoundToInt(GridWorldSize.z / _nodeDiameter);
        CreateGrid();
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

                    if (Physics.CheckSphere(worldPoint, NodeRadius, BlockedMask))
                    {
                        walkable = false;
                    }
                    else if (!(Physics.CheckSphere(worldPoint + Vector3.down, NodeRadius, BlockedMask)))
                    {
                        walkable = false;
                    }

                    _grid[x, y, z] = new Node(walkable, worldPoint);
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.z, GridWorldSize.y));

        if (_grid != null)
        {
            foreach (var node in _grid)
            {
                Gizmos.color = (node.Walkable) ? Color.green : Color.red;
                Gizmos.DrawSphere(node.WorldPosition, 0.1f);
            }
        }
    }
}
