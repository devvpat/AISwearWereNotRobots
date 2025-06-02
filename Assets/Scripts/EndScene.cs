using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    [SerializeField] private GameObject[] partsArray = new GameObject[3];
    [SerializeField] private Button nextButton;

    private int curPartIndex = 0;

    private void OnEnable()
    {
        foreach (GameObject part in partsArray)
        {
            part.SetActive(false);
        }
        if (partsArray.Length > 0)
        {
            partsArray[0].SetActive(true);
        }
    }

    public void NextPart()
    {
        if (curPartIndex < partsArray.Length)
        {
            partsArray[curPartIndex].SetActive(false);
            curPartIndex++;
            if (curPartIndex < partsArray.Length)
            {
                partsArray[curPartIndex].SetActive(true);
                if (curPartIndex == partsArray.Length - 1)
                {
                    nextButton.onClick.RemoveAllListeners();
                    nextButton.onClick.AddListener(EndGame);
                }
            }
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene(Globals.TitleSceneName);
    }
}
