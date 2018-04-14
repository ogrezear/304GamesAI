using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTarget : MonoBehaviour {

    public LayerMask TrgetMask;
    public int enemy;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	    if (Physics.CheckSphere(transform.position, 0.3f, TrgetMask))
	    {
	        Debug.Log("Finnish");
	        if (enemy == 1)
	        {
                SceneManager.LoadScene(3);
            }
            else if (enemy == 2)
	        {
	            SceneManager.LoadScene(6);
            }
            else if (enemy == 3)
	        {
	            SceneManager.LoadScene(9);
            }

        }

    }
}
