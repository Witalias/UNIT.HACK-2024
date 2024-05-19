using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _content;

    private bool _isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause();
        }
    }

    private void SetPause()
    {
        _isPaused = !_isPaused;
        _content.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0.0f : 1.0f;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
