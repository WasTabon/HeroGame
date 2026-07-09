using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameHUD : MonoBehaviour
{
    public Image inkFill;
    public TextMeshProUGUI inkText;
    public Button backButton;
    public Button clearButton;
    public Button muteButton;
    public TextMeshProUGUI muteLabel;

    private GameManager gameManager;
    private Tween fillTween;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        backButton.onClick.AddListener(OnBackClicked);
        clearButton.onClick.AddListener(OnClearClicked);
        muteButton.onClick.AddListener(OnMuteClicked);

        UpdateMuteLabel();

        if (InkManager.Instance != null)
        {
            InkManager.Instance.OnInkChanged += HandleInkChanged;
            HandleInkChanged(InkManager.Instance.UsedInk, InkManager.Instance.MaxInk);
        }
    }

    private void OnDestroy()
    {
        if (InkManager.Instance != null)
            InkManager.Instance.OnInkChanged -= HandleInkChanged;
    }

    private void HandleInkChanged(float used, float max)
    {
        float remaining = Mathf.Clamp01((max - used) / max);

        fillTween?.Kill();
        fillTween = DOTween.To(() => inkFill.fillAmount, x => inkFill.fillAmount = x, remaining, 0.3f)
            .SetEase(Ease.OutQuad);

        inkText.text = Mathf.RoundToInt(max - used) + " / " + Mathf.RoundToInt(max);

        inkFill.color = remaining > 0.3f
            ? new Color(0.290f, 0.565f, 0.886f)
            : new Color(0.886f, 0.29f, 0.29f);
    }

    private void OnBackClicked()
    {
        TransitionManager.Instance.LoadScene("LevelMap");
    }

    private void OnClearClicked()
    {
        if (gameManager != null)
            gameManager.RestartLevel();
    }

    private void OnMuteClicked()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ToggleSfxMute();
        UpdateMuteLabel();
    }

    private void UpdateMuteLabel()
    {
        if (SoundManager.Instance == null) return;
        muteLabel.text = SoundManager.Instance.SfxMuted ? "OFF" : "ON";
    }
}
