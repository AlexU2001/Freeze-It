using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public void NextLevel()
    {
        SceneController.instance.GoToNextLevel();
    }

    public void QuitGame()
    {
        SceneController.instance.Quit();
    }
}
