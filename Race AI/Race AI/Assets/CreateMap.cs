using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMap : MonoBehaviour
{

    public GameObject Tile;
    public int x, z;

	// Use this for initialization
	void Start () {
	    for (int a = 0; a < x; a++)
	    {
	        for (int b = 0; b < z; b++)
	        {
	            //GameObject cube = Object.Instantiate(Tile, new Vector3(a, 0, b), Quaternion.identity);//GameObject.CreatePrimitive(PrimitiveType.Cube);
	            //cube.transform.position = new Vector3(a, 0, b);
	            //cube.transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);

	        }
	    }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
