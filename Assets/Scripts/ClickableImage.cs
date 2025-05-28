using UnityEngine;
using UnityEngine.UI;

public class ClickableImage : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Child clicked");
        transform.parent.GetComponent<Button>().onClick.Invoke();
    }
}
