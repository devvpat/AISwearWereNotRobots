using System.Collections;
using UnityEngine;

public class FailScreen : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private GameObject failWindow;
    [SerializeField] private int numExtraPopups = 29; // total popups will be +1 including the original failWindow

    private int xMin = -683;
    private int xMax = 683;
    private int yMin = -179;
    private int yMax = 179;

    void OnEnable()
    {
        StartCoroutine(FailEffects());
    }

    private IEnumerator FailEffects()
    {
        audioSource.PlayOneShot(failSound);
        yield return new WaitForSeconds(1f);
        // popups
        failWindow.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        // create duplicates of failwindow and set their positions randomly in the 1920x1080 screen space
        for (int i = 0; i < numExtraPopups; i++)
        {
            GameObject popup = Instantiate(failWindow);
            popup.transform.SetParent(transform, false);
            // set rect transform position randomly within the screen bounds
            popup.transform.localPosition = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
            popup.SetActive(true);
            yield return new WaitForSeconds(0.1f); // slight delay between popups
        }
        yield return new WaitForSeconds(0.5f);
        // return to title screen
        UnityEngine.SceneManagement.SceneManager.LoadScene(Globals.TitleSceneName);
    }
}
