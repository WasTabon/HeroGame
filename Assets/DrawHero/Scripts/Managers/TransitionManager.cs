using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    private Canvas canvas;
    private Image fadeImage;
    private CanvasGroup fadeGroup;

    private const float FADE_DURATION = 0.35f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureFadeCanvas();
        fadeGroup.alpha = 1f;
        fadeGroup.blocksRaycasts = true;
        fadeGroup.DOFade(0f, FADE_DURATION).SetEase(Ease.OutQuad)
            .OnComplete(() => fadeGroup.blocksRaycasts = false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void EnsureFadeCanvas()
    {
        if (fadeGroup != null && canvas != null) return;

        GameObject canvasGo = new GameObject("TransitionCanvas");
        canvasGo.transform.SetParent(transform);
        canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject imageGo = new GameObject("FadeImage");
        imageGo.transform.SetParent(canvasGo.transform, false);
        fadeImage = imageGo.AddComponent<Image>();
        fadeImage.color = new Color(0.1f, 0.1f, 0.18f, 1f);

        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        fadeGroup = imageGo.AddComponent<CanvasGroup>();
    }

    public void LoadScene(string sceneName)
    {
        EnsureFadeCanvas();

        fadeGroup.blocksRaycasts = true;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("transition");

        fadeGroup.DOFade(1f, FADE_DURATION).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(sceneName);
            });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        EnsureFadeCanvas();
        fadeGroup.blocksRaycasts = true;
        fadeGroup.alpha = 1f;
        fadeGroup.DOFade(0f, FADE_DURATION).SetEase(Ease.OutQuad)
            .OnComplete(() => fadeGroup.blocksRaycasts = false);
    }
}
