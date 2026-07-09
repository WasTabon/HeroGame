using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public Button backButton;
    public TMPro.TextMeshProUGUI levelLabel;

    private void Start()
    {
        levelLabel.text = "Level " + GameSession.SelectedLevel;

        backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
        TransitionManager.Instance.LoadScene("LevelMap");
    }
}
