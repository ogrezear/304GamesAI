using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {

        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {

        Application.Quit();
    }
}
