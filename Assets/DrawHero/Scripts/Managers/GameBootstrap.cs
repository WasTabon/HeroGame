using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        SceneManager.LoadScene("MainMenu");
    }
}
