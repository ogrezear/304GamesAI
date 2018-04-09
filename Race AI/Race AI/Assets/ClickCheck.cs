using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ClickCheck : MonoBehaviour
{
    private Grid _grid;
    public GameObject Player;

    void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit)) return;
        if (!_grid.NodeFromWorldPosition(hit.transform.position).InRange) return;
        //Debug.Log(hit.transform.name);
        var currentPos = Player.transform.position;
        var targetPos = hit.transform.position + Vector3.up - new Vector3(0, 0.4f, 0);
        var t = 0.0f;
        while (t < 1.0f)
        {

            //player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.LookRotation(-(player.transform.position - hit.transform.position + Vector3.up + new Vector3(0, 0.55f,0) )), t);
            if (t == 0.0f)
            {
                // player.GetComponent<Animator>().SetTrigger("jump");
            }
            Player.transform.position = Vector3.Lerp(currentPos, targetPos, t);
            t += Time.deltaTime;
        }
    }
}
