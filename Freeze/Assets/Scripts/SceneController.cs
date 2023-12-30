using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject controllerObj = new GameObject();
                controllerObj.name = "SceneController";
                _instance = controllerObj.AddComponent<SceneController>();
                DontDestroyOnLoad(controllerObj);
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public void GoToNextLevel()
    {
        Debug.Log("Going to next...");
        int max = SceneManager.sceneCountInBuildSettings;
        int nextLevel = (SceneManager.GetActiveScene().buildIndex + 1) % max;
        Debug.Log("Next is level " + nextLevel);
        if(nextLevel == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        SceneManager.LoadScene(nextLevel);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
