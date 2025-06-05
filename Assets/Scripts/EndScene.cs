using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EndScene : MonoBehaviour
{
    [SerializeField] private string[] part1Lines = new string[1];
    [SerializeField] private string[] part2Lines = new string[1];
    [SerializeField] private string[] part3Lines = new string[1];
    [SerializeField] private string[] part4Lines = new string[1];
    [SerializeField] private string[] part5Lines = new string[1];
    [SerializeField] private Sprite[] partsImages = new Sprite[5];
    [SerializeField] private TMP_Text textArea;
    [SerializeField] private Image imageArea;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button endButton;
    [SerializeField] private Volume glitchVolume;
    [SerializeField] private AudioSource glitchSFX;

    private int[] partLineLengths = new int[] { 13, 16, 7, 18, 8 };

    private int curImg = 0;
    private int curLine = 0;

    private void OnEnable()
    {
        imageArea.sprite = partsImages[0];
        textArea.text = part1Lines[0];
        nextButton.gameObject.SetActive(true);
        endButton.gameObject.SetActive(false);
    }

    public void NextPart()
    {
        curLine++;

        if (curLine >= partLineLengths[curImg])
        {
            curImg++;
            if (curImg >= partsImages.Length)
            {
                ShowEndText();
                return;
            }
            else
            {
                imageArea.sprite = partsImages[curImg];

                // Enable glitch only during Part 3
                bool isPart3 = (curImg == 2);
                if (glitchVolume != null)
                    glitchVolume.enabled = isPart3;

                if (glitchSFX != null)
                {
                    if (isPart3 && !glitchSFX.isPlaying)
                        glitchSFX.Play();
                    else if (!isPart3 && glitchSFX.isPlaying)
                        glitchSFX.Stop();
                }
            }
            curLine = 0;
        }

        string line = curImg switch
        {
            0 => part1Lines[curLine],
            1 => part2Lines[curLine],
            2 => part3Lines[curLine],
            3 => part4Lines[curLine],
            4 => part5Lines[curLine],
            _ => ""
        };
        textArea.text = line;
    }

    private void ShowEndText()
    {
        nextButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(true);
    }

    public void EndGame()
    {
        SceneManager.LoadScene(Globals.TitleSceneName);
    }

}
