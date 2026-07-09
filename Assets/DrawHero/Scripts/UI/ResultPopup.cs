using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class ResultPopup : MonoBehaviour
{
    public CanvasGroup backdrop;
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public Button nextButton;
    public Button retryButton;
    public Button menuButton;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowWin()
    {
        titleText.text = "LEVEL COMPLETE";
        titleText.color = new Color(0.4f, 0.85f, 0.35f);
        nextButton.gameObject.SetActive(true);
        Show();
    }

    public void ShowLose()
    {
        titleText.text = "OUT OF INK";
        titleText.color = new Color(0.9f, 0.4f, 0.35f);
        nextButton.gameObject.SetActive(false);
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);

        nextButton.onClick.RemoveAllListeners();
        retryButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnNext);
        retryButton.onClick.AddListener(OnRetry);
        menuButton.onClick.AddListener(OnMenu);

        backdrop.alpha = 0f;
        backdrop.DOFade(0.6f, 0.3f);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.1f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop");
    }

    private void OnNext()
    {
        int next = GameSession.SelectedLevel + 1;
        if (next > GameSession.TotalLevels)
            next = GameSession.TotalLevels;
        GameSession.SelectedLevel = next;
        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnRetry()
    {
        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnMenu()
    {
        TransitionManager.Instance.LoadScene("LevelMap");
    }
}
