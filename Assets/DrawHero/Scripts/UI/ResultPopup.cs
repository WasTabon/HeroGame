using UnityEngine;
using UnityEngine.UI;
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
    public Image[] stars;

    public Color winColor = new Color(0.4f, 0.85f, 0.35f);
    public Color loseColor = new Color(0.9f, 0.4f, 0.35f);
    public Color starEarnedColor = new Color(0.98f, 0.78f, 0.20f);
    public Color starEmptyColor = new Color(1f, 1f, 1f, 0.15f);

    public static ResultPopup Spawn()
    {
        GameObject prefab = Resources.Load<GameObject>("ResultPopup");
        if (prefab == null)
        {
            Debug.LogError("ResultPopup prefab not found in Resources folder!");
            return null;
        }

        GameObject instance = Instantiate(prefab);
        return instance.GetComponent<ResultPopup>();
    }

    public void ShowWin(int starCount)
    {
        titleText.text = "LEVEL COMPLETE";
        titleText.color = winColor;

        nextButton.gameObject.SetActive(true);
        SetStarsActive(true);

        Show();
        AnimateStars(starCount);
    }

    public void ShowLose()
    {
        titleText.text = "OUT OF INK";
        titleText.color = loseColor;

        nextButton.gameObject.SetActive(false);
        SetStarsActive(false);

        Show();
    }

    private void SetStarsActive(bool active)
    {
        if (stars == null) return;
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].gameObject.SetActive(active);
        }
    }

    private void AnimateStars(int starCount)
    {
        if (stars == null) return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;
            stars[i].color = starEmptyColor;
            stars[i].transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;

            int index = i;
            bool won = index < starCount;

            stars[index].transform.DOScale(1f, 0.4f)
                .SetEase(Ease.OutBack)
                .SetDelay(0.5f + index * 0.2f)
                .OnStart(() =>
                {
                    if (won)
                    {
                        stars[index].color = starEarnedColor;
                        if (SoundManager.Instance != null)
                            SoundManager.Instance.PlaySfx("pop", 1f + index * 0.15f);
                    }
                });
        }
    }

    private void Show()
    {
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
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");

        int next = GameSession.SelectedLevel + 1;
        if (next > GameSession.TotalLevels)
            next = GameSession.TotalLevels;

        GameSession.SelectedLevel = next;
        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnRetry()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");

        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnMenu()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");

        TransitionManager.Instance.LoadScene("LevelMap");
    }
}
