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
        SceneManager.LoadScene(1);
    }

    public void Astar()
    {
        SceneManager.LoadScene(1);
    }

    public void BFS()
    {
        SceneManager.LoadScene(4);
    }

    public void DFS()
    {

        SceneManager.LoadScene(7);
    }

    public void QuitGame()
    {

        Application.Quit();
    }
}
