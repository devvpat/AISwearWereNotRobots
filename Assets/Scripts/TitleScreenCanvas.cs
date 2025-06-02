using UnityEngine;

public class TitleScreenCanvas : MonoBehaviour
{
    public void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Globals.GameSceneName);
    }
}
