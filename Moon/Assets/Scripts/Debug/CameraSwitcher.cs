using Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] _cameras;

    private int _currentCameraIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _cameras[_currentCameraIndex].SetActive(false);

            if (++_currentCameraIndex >= _cameras.Length)
                _currentCameraIndex = 0;

            _cameras[_currentCameraIndex].SetActive(true);
        }
    }
}
