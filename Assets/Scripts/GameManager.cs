using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        SetupCursor();
    }

    private void SetupCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
