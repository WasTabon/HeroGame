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

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void EnsureStars()
    {
        if (stars != null && stars.Length == 3 && stars[0] != null && stars[1] != null && stars[2] != null)
            return;

        Transform panel = transform.Find("Panel");
        if (panel == null) return;

        Transform existing = panel.Find("StarRow");
        if (existing != null)
            Destroy(existing.gameObject);

        GameObject starRow = new GameObject("StarRow", typeof(RectTransform));
        starRow.transform.SetParent(panel, false);
        RectTransform rowRt = starRow.GetComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0.5f, 1f);
        rowRt.anchorMax = new Vector2(0.5f, 1f);
        rowRt.pivot = new Vector2(0.5f, 1f);
        rowRt.sizeDelta = new Vector2(600, 180);
        rowRt.anchoredPosition = new Vector2(0, -200);

        stars = new Image[3];
        float spacing = 190f;
        for (int i = 0; i < 3; i++)
        {
            GameObject holder = new GameObject("StarHolder" + i, typeof(RectTransform));
            holder.transform.SetParent(starRow.transform, false);
            RectTransform hRt = holder.GetComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0.5f, 0.5f);
            hRt.anchorMax = new Vector2(0.5f, 0.5f);
            hRt.pivot = new Vector2(0.5f, 0.5f);
            hRt.sizeDelta = new Vector2(160, 160);
            hRt.anchoredPosition = new Vector2((i - 1) * spacing, i == 1 ? 20 : 0);

            GameObject starGo = new GameObject("Star", typeof(RectTransform), typeof(Image));
            starGo.transform.SetParent(holder.transform, false);
            Image img = starGo.GetComponent<Image>();
            img.sprite = PrimitiveSprites.Circle();
            img.color = new Color(1f, 1f, 1f, 0.15f);
            RectTransform sRt = starGo.GetComponent<RectTransform>();
            sRt.anchorMin = Vector2.zero;
            sRt.anchorMax = Vector2.one;
            sRt.offsetMin = Vector2.zero;
            sRt.offsetMax = Vector2.zero;
            stars[i] = img;
        }
    }

    public void ShowWin(int starCount)
    {
        EnsureStars();
        titleText.text = "LEVEL COMPLETE";
        titleText.color = new Color(0.4f, 0.85f, 0.35f);
        nextButton.gameObject.SetActive(true);
        SetStarsVisible(true);
        Show();
        AnimateStars(starCount);
    }

    public void ShowLose()
    {
        titleText.text = "OUT OF INK";
        titleText.color = new Color(0.9f, 0.4f, 0.35f);
        nextButton.gameObject.SetActive(false);
        SetStarsVisible(false);
        Show();
    }

    private void SetStarsVisible(bool visible)
    {
        if (stars == null) return;
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;
            Transform parent = stars[i].transform.parent;
            if (parent != null)
                parent.gameObject.SetActive(visible);
            else
                stars[i].gameObject.SetActive(visible);
        }
    }

    private void AnimateStars(int starCount)
    {
        if (stars == null || stars.Length == 0) return;

        Color earned = new Color(0.98f, 0.78f, 0.20f);
        Color empty = new Color(1f, 1f, 1f, 0.15f);

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;
            stars[i].color = empty;
            stars[i].transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;
            int index = i;
            bool won = index < starCount;
            float delay = 0.5f + index * 0.2f;

            stars[index].transform.DOScale(1f, 0.4f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay)
                .OnStart(() =>
                {
                    if (won)
                    {
                        stars[index].color = earned;
                        if (SoundManager.Instance != null)
                            SoundManager.Instance.PlaySfx("pop", 1f + index * 0.15f);
                    }
                });
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Canvas overrideCanvas = GetComponent<Canvas>();
        if (overrideCanvas == null)
        {
            overrideCanvas = gameObject.AddComponent<Canvas>();
            gameObject.AddComponent<GraphicRaycaster>();
        }
        overrideCanvas.overrideSorting = true;
        overrideCanvas.sortingOrder = 500;

        nextButton.onClick.RemoveAllListeners();
        retryButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnNext);
        retryButton.onClick.AddListener(OnRetry);
        menuButton.onClick.AddListener(OnMenu);

        backdrop.DOKill();
        panel.DOKill();

        backdrop.alpha = 0.6f;
        backdrop.alpha = 0f;
        backdrop.DOFade(0.6f, 0.3f);

        panel.localScale = Vector3.one;
        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.1f)
            .OnKill(() => panel.localScale = Vector3.one)
            .OnComplete(() => panel.localScale = Vector3.one);

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