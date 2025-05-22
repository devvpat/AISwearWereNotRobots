using UnityEngine;

public class TextWindowPopup : MonoBehaviour
{
    [SerializeField] private GameObject[] textWindows = new GameObject[5];

    void Start()
    {
        if (textWindows.Length != 5)
        {
            Debug.LogError(gameObject.name + " TextWindow array must have exactly 5 elements.");
            return;
        }
        for (int i = 0; i < textWindows.Length; i++)
        {
            if (textWindows[i] == null)
            {
                Debug.LogError($"{gameObject.name} TextWindow array index {i} is not assigned.");
                return;
            }
            textWindows[i].SetActive(false);
        }
    }

    public void OnClick()
    {
        Debug.Log(gameObject.name + " clicked");
        textWindows[GameManager.Instance.CurrentDay].transform.SetAsLastSibling();
        textWindows[GameManager.Instance.CurrentDay].SetActive(true);
    }
}
