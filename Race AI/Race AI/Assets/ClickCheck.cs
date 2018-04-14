using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickCheck : MonoBehaviour
{
    private Grid _grid;
    private PathfindingAstar _aStar;
    private BFS _bfs;
    private DFS _dfs;
    private bool aiStarted = false;
    public GameObject Player;
    private bool _click = true;
    public int game;

    void Awake()
    {
        _grid = GetComponent<Grid>();
        _aStar = GetComponent<PathfindingAstar>();
        _bfs = GetComponent<BFS>();
        _dfs = GetComponent<DFS>();
    }

    void Start()
    {
        Player.GetComponent<Animator>().speed = 1.2f;
    }

    void Update()
    {
       

        if (!Input.GetMouseButtonDown(0) || !_click) return;
        RaycastHit hit;
        if (!aiStarted)
        {
            if (game==1)
            {
                _aStar.StartAI();
            }
            else if (game==2)
            {
                _bfs.StartAI();
            }
            else if (game == 3)
            {
                _dfs.StartAI();
            }
            aiStarted = true;
        }
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit)) return;
        if (!_grid.NodeFromWorldPosition(hit.transform.position).InRange) return;
        //Debug.Log(hit.transform.name);
        var currentPos = Player.transform.position;
        var targetPos = hit.transform.position + Vector3.up - new Vector3(0, 0.4f, 0);

        var distance = Vector3.Distance(currentPos, targetPos);
        if (Math.Abs(distance - 1.41) < 0.15)
        {
            Player.GetComponent<Animator>().speed = 1f;
        }
        else if (Math.Abs(distance - 1) < 0.15)
        {
            Player.GetComponent<Animator>().speed = 1.2f;
        }
        else if (Math.Abs(distance - 1.73) < 0.15)
        {
            Player.GetComponent<Animator>().speed = 0.8f;
        }
        Player.GetComponent<Animator>().SetTrigger("jump");
        StartCoroutine(UpdatePosition(currentPos, targetPos, distance));

        //var t = 0.0f;
        //while (t < 1.0f)
        //{

        //    //player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.LookRotation(-(player.transform.position - hit.transform.position + Vector3.up + new Vector3(0, 0.55f,0) )), t);
        //    if (t == 0.0f)
        //    {
        //        // player.GetComponent<Animator>().SetTrigger("jump");
        //    }
        //    Player.transform.position = Vector3.Lerp(currentPos, targetPos, t);
        //    t += Time.deltaTime;
        //}
    }

    IEnumerator UpdatePosition(Vector3 currentPos, Vector3 targetPos, float distance)
    {
        _click = false;
        var t = 0.0f;
        

        while (t < 1.0f)
        {

            Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, Quaternion.LookRotation(-(Player.transform.position - targetPos)), t);
            Player.transform.position = Vector3.Lerp(currentPos, targetPos, t);
            t += Time.deltaTime/distance;
            yield return null;

        }
        Player.transform.position = targetPos;
        _click = true;
    }

}
