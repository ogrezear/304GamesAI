using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BFS : MonoBehaviour {

    private Grid grid;
    public GameObject seeker, target;

    private bool move = false, canStart = true;
    private Vector3 cachedSeekerPos;
    private Vector3 cachedTargetPos;

    // Use this for initialization
    void Start ()
    {
        cachedSeekerPos = seeker.transform.position;
        cachedTargetPos = target.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.B))
	    {
	        FindShortestPathBFS(seeker.transform.position, target.transform.position);
	        move = true;
	    }

        // Uncomment this if you wish to update the path real time, while you are moving the sheep around;
        //if (!move && canStart)
        //{
        //    if (cachedSeekerPos != seeker.transform.position)
        //    {
        //        cachedSeekerPos = seeker.transform.position;
        //        FindShortestPathBFS(seeker.transform.position, target.transform.position);
        //    }
        //    if (cachedTargetPos != target.transform.position)
        //    {
        //        cachedTargetPos = target.transform.position;
        //        FindShortestPathBFS(seeker.transform.position, target.transform.position);
        //    }
        //}
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

        while (t < 1.0f)
        {

            seeker.transform.rotation = Quaternion.Lerp(seeker.transform.rotation, Quaternion.LookRotation(-(seeker.transform.position - correctedPathPos)), t);
            if (t == 0.0f)
            {
                seeker.GetComponent<Animator>().SetTrigger("jump");
            }
            seeker.transform.position = Vector3.Lerp(currentPos, correctedPathPos, t);
            t += Time.deltaTime / distance;
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

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    //Breadth first search of graph.
    //Populates IList<Vector3> path with a valid solution to the goalPosition.
    public void FindShortestPathBFS(Vector3 startPosition, Vector3 goalPosition)
    {
        Queue<Node> queue = new Queue<Node>();
        List<Node> exploredNodes = new List<Node>();

        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(goalPosition);

        queue.Enqueue(startNode);
        IDictionary<Node, Node> nodeParents = new Dictionary<Node, Node>();

        while (queue.Count != 0)
        {
            Node currentNode = queue.Dequeue();
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            IList<Node> nodes = GetWalkableNodes(currentNode);

            foreach (Node node in nodes)
            {
                if (!exploredNodes.Contains(node))
                {
                    //Mark the node as explored
                    exploredNodes.Add(node);

                    //Store a reference to the previous node
                    node.parent = currentNode;
                    nodeParents.Add(node, currentNode);

                    //Add this to the queue of nodes to examine
                    queue.Enqueue(node);
                }
            }
        }
    }

    private IList<Node> GetWalkableNodes(Node currentNode)
    {
        var neighours = grid.GetNeighbours(currentNode);
        var walkableNeighours = new List<Node>();
        foreach (var neighour in neighours)
        {
            if (neighour.Walkable)
            {
                walkableNeighours.Add(neighour);
            }  
        }

        return walkableNeighours;
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
