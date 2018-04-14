using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DFS : MonoBehaviour {

    private Grid _grid;
    public GameObject Seeker, Target;

    private bool _move;
    private bool _canStart = true;
    private Vector3 _cachedSeekerPos;
    private Vector3 _cachedTargetPos;

    void Start ()
    {
        _cachedSeekerPos = Seeker.transform.position;
        _cachedTargetPos = Target.transform.position;
    }
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        StartAI();
	    }

        //Uncomment this if you wish to update the path real time, while you are moving the sheep around;
        //if (!_move && _canStart)
        //{
        //    if (_cachedSeekerPos != Seeker.transform.position)
        //    {
        //        _cachedSeekerPos = Seeker.transform.position;
        //        FindShortestPathDFS(Seeker.transform.position, Target.transform.position);
        //    }
        //    if (_cachedTargetPos != Target.transform.position)
        //    {
        //        _cachedTargetPos = Target.transform.position;
        //        FindShortestPathDFS(Seeker.transform.position, Target.transform.position);
        //    }
        //}
        else
        {
            AnimatePath();
        }
    }

    public void StartAI()
    {
        FindShortestPathDFS(Seeker.transform.position, Target.transform.position);
        _move = true;
    }
    void AnimatePath()
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

    IEnumerator UpdatePosition(Vector3 currentPos, Node n, int index)
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

        while (t < 1.0f)
        {

            Seeker.transform.rotation = Quaternion.Lerp(Seeker.transform.rotation, Quaternion.LookRotation(-(Seeker.transform.position - correctedPathPos)), t);
            if (t == 0.0f)
            {
                Seeker.GetComponent<Animator>().SetTrigger("jump");
            }
            Seeker.transform.position = Vector3.Lerp(currentPos, correctedPathPos, t);
            t += Time.deltaTime / distance;
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
            SceneManager.LoadScene(8);
            yield return null;
        }

    }

    void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    //Depth first search of graph.
    //Populates IList<Vector3> path with a valid solution to the goalPosition.
    public void FindShortestPathDFS(Vector3 startPosition, Vector3 goalPosition)
    {
        var stack = new Stack<Node> ();
        var exploredNodes = new List<Node>();

        var startNode = _grid.NodeFromWorldPosition(startPosition);
        var targetNode = _grid.NodeFromWorldPosition(goalPosition);

        stack.Push(startNode);

        IDictionary<Node, Node> nodeParents = new Dictionary<Node, Node>();

        while (stack.Count != 0)
        {
            var currentNode = stack.Pop();
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            var nodes = GetWalkableNodes(currentNode);

            foreach (var node in nodes)
            {
                if (exploredNodes.Contains(node)) continue;
                //Mark the node as explored
                exploredNodes.Add(node);

                //Store a reference to the previous node
                node.Parent = currentNode;
                nodeParents.Add(node, currentNode);

                //Add this to the queue of nodes to examine
                stack.Push(node);
            }
        }
    }

    private IEnumerable<Node> GetWalkableNodes(Node currentNode)
    {
        var neighours = _grid.GetNeighbours(currentNode);
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

    private void RetracePath(Node startNode, Node endNode)
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
