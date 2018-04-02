using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pathfinding : MonoBehaviour
{
    public GameObject seeker, target;

    private Grid grid;

    private bool move = false, canStart = true;
    private Vector3 cachedSeekerPos;
    private Vector3 cachedTargetPos;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Start()
    {
        cachedSeekerPos = seeker.transform.position;
        cachedTargetPos = target.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(seeker.transform.position, target.transform.position);
            move = true;
        }

        if (!move && canStart)
        {
            if (cachedSeekerPos != seeker.transform.position)
            {
                cachedSeekerPos = seeker.transform.position;
                FindPath(seeker.transform.position, target.transform.position);
            }
            if (cachedTargetPos != target.transform.position)
            {
                cachedTargetPos = target.transform.position;
                FindPath(seeker.transform.position, target.transform.position);
            }
        }
        else
        {
             AnimatePath();
        }
    }

    void AnimatePath()
    {
        move = false;
        canStart = false;
        Vector3 currentPos = seeker.transform.position;
        seeker.GetComponent<Animator>().speed = 0.9f;
        if (grid.path != null)
        {
            StartCoroutine(UpdatePosition(currentPos, grid.path[0], 0));
        }
    }

    IEnumerator UpdatePosition(Vector3 currentPos, Node n, int index)
    {
        float t = 0.0f;
        Vector3 correctedPathPos = new Vector3(n.WorldPosition.x, n.WorldPosition.y - 0.487f, n.WorldPosition.z);
        var distance = GetDistance(grid.NodeFromWorldPosition(currentPos), n);
        Debug.Log(distance.ToString());

      

        if (distance == 2)
        {
            seeker.GetComponent<Animator>().speed = 0.8f;
        }
        else if (distance == 1)
        {
            seeker.GetComponent<Animator>().speed = 1.2f;
        }
        else if (distance == 3)
        {
            seeker.GetComponent<Animator>().speed = 0.6f;
        }

        while (t < 1.0f )
        {
            
            seeker.transform.rotation = Quaternion.Lerp(seeker.transform.rotation ,Quaternion.LookRotation(-(seeker.transform.position - correctedPathPos)),t);
            if (t==0.0f)
            {
                seeker.GetComponent<Animator>().SetTrigger("jump");
            }
            seeker.transform.position = Vector3.Lerp(currentPos, correctedPathPos, t);
            t += Time.deltaTime/distance;
            yield return null;

        }
        seeker.transform.position = correctedPathPos;
        currentPos = correctedPathPos;

        index++;
        if (index < grid.path.Count)
            StartCoroutine(UpdatePosition(currentPos, grid.path[index], index));
        else
        {
            canStart = true;
            Debug.Log("finish");
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(1);
            yield return null;
        }

    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (var neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovementCostToNeighbour = (currentNode.gCost + GetDistance(currentNode, neighbour));
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    float GetDistance(Node nodeA, Node nodeB)
    {
        float dstX = nodeA.gridX - nodeB.gridX;
        float dstY = nodeA.gridY - nodeB.gridY;
        float dstZ = nodeA.gridZ - nodeB.gridZ;

        return dstX * dstX + dstY * dstY + dstZ * dstZ;
    }
}
