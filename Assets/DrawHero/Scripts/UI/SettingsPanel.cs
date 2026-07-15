using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour
{
    public CanvasGroup backdrop;
    public RectTransform panel;
    public Button backdropButton;
    public Button sfxButton;
    public Button musicButton;
    public Button hapticButton;
    public Button shopButton;
    public Button resetButton;
    public Button closeButton;
    public TextMeshProUGUI sfxLabel;
    public TextMeshProUGUI musicLabel;
    public TextMeshProUGUI hapticLabel;

    public static void Open()
    {
        GameObject prefab = Resources.Load<GameObject>("SettingsPanel");
        if (prefab == null)
        {
            Debug.LogError("SettingsPanel prefab not found in Resources folder!");
            return;
        }

        Instantiate(prefab);
    }

    private void Start()
    {
        sfxButton.onClick.AddListener(OnSfxToggle);
        musicButton.onClick.AddListener(OnMusicToggle);
        hapticButton.onClick.AddListener(OnHapticToggle);
        shopButton.onClick.AddListener(OnShop);
        resetButton.onClick.AddListener(OnReset);
        closeButton.onClick.AddListener(Close);

        if (backdropButton != null)
            backdropButton.onClick.AddListener(Close);

        RefreshLabels();

        backdrop.alpha = 0f;
        backdrop.DOFade(0.6f, 0.3f);

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.05f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop");
    }

    private void RefreshLabels()
    {
        if (SoundManager.Instance != null)
        {
            sfxLabel.text = "SFX: " + (SoundManager.Instance.SfxMuted ? "OFF" : "ON");
            musicLabel.text = "MUSIC: " + (SoundManager.Instance.MusicMuted ? "OFF" : "ON");
        }

        if (HapticManager.Instance != null)
            hapticLabel.text = "HAPTICS: " + (HapticManager.Instance.HapticEnabled ? "ON" : "OFF");
    }

    private void OnSfxToggle()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ToggleSfxMute();
        RefreshLabels();
    }

    private void OnMusicToggle()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ToggleMusicMute();
        RefreshLabels();
    }

    private void OnHapticToggle()
    {
        if (HapticManager.Instance != null)
            HapticManager.Instance.ToggleHaptic();
        RefreshLabels();
    }

    private void OnShop()
    {
        ShopPanel.Open();
    }

    private void OnReset()
    {
        ProgressManager.ResetAllProgress();
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("back");
    }

    private void Close()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");

        panel.DOScale(0f, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
