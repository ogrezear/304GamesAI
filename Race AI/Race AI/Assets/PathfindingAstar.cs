using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class PathfindingAstar : MonoBehaviour
{
    public GameObject Seeker, Target;

    private Grid _grid;

    private bool _move;
    private bool _canStart = true;
    private Vector3 _cachedSeekerPos;
    private Vector3 _cachedTargetPos;

    void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    void Start()
    {
        _cachedSeekerPos = Seeker.transform.position;
        _cachedTargetPos = Target.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(Seeker.transform.position, Target.transform.position);
            _move = true;
        }
        
        // Uncomment this if you wish to update the path real time, while you are moving the sheep around;
        //if (!move && canStart)
        //{
        //    if (cachedSeekerPos != seeker.transform.position)
        //    {
        //        cachedSeekerPos = seeker.transform.position;
        //        FindPath(seeker.transform.position, target.transform.position);
        //    }
        //    if (cachedTargetPos != target.transform.position)
        //    {
        //        cachedTargetPos = target.transform.position;
        //        FindPath(seeker.transform.position, target.transform.position);
        //    }
        //}
        else
        {
            AnimatePath();
        }
    }

    private void AnimatePath()
    {
        _move = false;
        _canStart = false;
        var currentPos = Seeker.transform.position;
        Seeker.GetComponent<Animator>().speed = 0.9f;
        if (_grid.Path != null)
        {
            StartCoroutine(UpdatePosition(currentPos, _grid.Path[0], 0));
        }
    }

    private IEnumerator UpdatePosition(Vector3 currentPos, Node n, int index)
    {
        var t = 0.0f;
        var correctedPathPos = new Vector3(n.WorldPosition.x, n.WorldPosition.y - 0.487f, n.WorldPosition.z);
        var distance = GetDistance(_grid.NodeFromWorldPosition(currentPos), n);
        //Debug.Log(distance.ToString());

      

        if (distance == 2)
        {
            Seeker.GetComponent<Animator>().speed = 0.8f;
        }
        else if (distance == 1)
        {
            Seeker.GetComponent<Animator>().speed = 1.2f;
        }
        else if (distance == 3)
        {
            Seeker.GetComponent<Animator>().speed = 0.6f;
        }

        while (t < 1.0f )
        {
            
            Seeker.transform.rotation = Quaternion.Lerp(Seeker.transform.rotation ,Quaternion.LookRotation(-(Seeker.transform.position - correctedPathPos)),t);
            if (t==0.0f)
            {
                Seeker.GetComponent<Animator>().SetTrigger("jump");
            }
            Seeker.transform.position = Vector3.Lerp(currentPos, correctedPathPos, t);
            t += Time.deltaTime/distance;
            yield return null;

        }
        Seeker.transform.position = correctedPathPos;
        currentPos = correctedPathPos;

        index++;
        if (index < _grid.Path.Count)
            StartCoroutine(UpdatePosition(currentPos, _grid.Path[index], index));
        else
        {
            _canStart = true;
            //Debug.Log("finish");
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(1);
            yield return null;
        }

    }

    private void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        var sw = new Stopwatch();
        sw.Start();
        var startNode = _grid.NodeFromWorldPosition(startPos);
        var targetNode = _grid.NodeFromWorldPosition(targetPos);

        var openSet = new Heap<Node>(_grid.MaxSize);
        var closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.RemoveFirst(); // oprimised (0ms)

            // Old and unoptimised (2ms)
            //var currentNode = openSet[0];
            //for (var i = 1; i < openSet.Count; i++)
            //{
            //    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
            //    {
            //        currentNode = openSet[i];
            //    }
            //}
            //openSet.Remove(currentNode);

            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                sw.Stop();
                Debug.Log("Path found in: " + sw.ElapsedMilliseconds + "ms");
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (var neighbour in _grid.GetNeighbours(currentNode))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                var newMovementCostToNeighbour = (currentNode.GCost + GetDistance(currentNode, neighbour));
                if (!(newMovementCostToNeighbour < neighbour.GCost) && openSet.Contains(neighbour)) continue;
                neighbour.GCost = newMovementCostToNeighbour;
                neighbour.HCost = GetDistance(neighbour, targetNode);
                neighbour.Parent = currentNode;

                if (!openSet.Contains(neighbour))
                {
                    openSet.Add(neighbour);
                }
            }
        }
    }

    public void RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Node>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();

        _grid.Path = path;
    }

    private static float GetDistance(Node nodeA, Node nodeB)
    {
        float dstX = nodeA.GridX - nodeB.GridX;
        float dstY = nodeA.GridY - nodeB.GridY;
        float dstZ = nodeA.GridZ - nodeB.GridZ;

        return dstX * dstX + dstY * dstY + dstZ * dstZ;
    }
}
