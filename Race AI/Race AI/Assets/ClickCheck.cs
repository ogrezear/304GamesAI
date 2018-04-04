using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ClickCheck : MonoBehaviour
{
    private Grid grid;
    public GameObject player;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (grid.NodeFromWorldPosition(hit.transform.position).InRange)
                {
                    Debug.Log(hit.transform.name);
                    var currentPos = player.transform.position;
                    var targetPos = hit.transform.position + Vector3.up - new Vector3(0, 0.4f, 0);
                    float t = 0.0f;
                    while (t < 1.0f)
                    {

                        //player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.LookRotation(-(player.transform.position - hit.transform.position + Vector3.up + new Vector3(0, 0.55f,0) )), t);
                        if (t == 0.0f)
                        {
                           // player.GetComponent<Animator>().SetTrigger("jump");
                        }
                        player.transform.position = Vector3.Lerp(currentPos, targetPos, t);
                        t += Time.deltaTime;
                    }
                }
            }
        }
    }
}
