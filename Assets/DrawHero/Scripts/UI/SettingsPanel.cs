using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour
{
    public static void Open()
    {
        GameObject go = new GameObject("SettingsPanel");
        SettingsPanel panel = go.AddComponent<SettingsPanel>();
        panel.Build();
    }

    private RectTransform panel;
    private TextMeshProUGUI sfxLabel;
    private TextMeshProUGUI musicLabel;
    private TextMeshProUGUI hapticLabel;

    private void Build()
    {
        GameObject canvasGo = new GameObject("SettingsCanvas");
        canvasGo.transform.SetParent(transform);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32760;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        GameObject backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        backdrop.transform.SetParent(canvasGo.transform, false);
        backdrop.GetComponent<Image>().color = Color.black;
        RectTransform bdRt = backdrop.GetComponent<RectTransform>();
        bdRt.anchorMin = Vector2.zero;
        bdRt.anchorMax = Vector2.one;
        bdRt.offsetMin = Vector2.zero;
        bdRt.offsetMax = Vector2.zero;
        CanvasGroup bdGroup = backdrop.GetComponent<CanvasGroup>();
        bdGroup.alpha = 0f;
        bdGroup.DOFade(0.6f, 0.3f);
        Button bdBtn = backdrop.AddComponent<Button>();
        bdBtn.onClick.AddListener(Close);

        GameObject panelGo = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panelGo.transform.SetParent(canvasGo.transform, false);
        panelGo.GetComponent<Image>().color = new Color(0.14f, 0.14f, 0.24f, 1f);
        panel = panelGo.GetComponent<RectTransform>();
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(820, 1000);
        panel.anchoredPosition = Vector2.zero;

        CreateLabel(panel, "SETTINGS", 60, Color.white, new Vector2(0, -70), new Vector2(760, 120));

        sfxLabel = CreateToggleRow(panel, "SFX", new Vector2(0, 120), OnSfxToggle);
        musicLabel = CreateToggleRow(panel, "MUSIC", new Vector2(0, -30), OnMusicToggle);
        hapticLabel = CreateToggleRow(panel, "HAPTICS", new Vector2(0, -180), OnHapticToggle);

        RefreshLabels();

        CreateButton(panel, "RESET PROGRESS", new Color(0.7f, 0.3f, 0.3f), new Vector2(0, -330), OnReset);
        CreateButton(panel, "CLOSE", new Color(0.961f, 0.651f, 0.137f), new Vector2(0, -470), Close);

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

    private TextMeshProUGUI CreateToggleRow(Transform parent, string name, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(name + "Toggle", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.290f, 0.565f, 0.886f);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600, 120);
        rt.anchoredPosition = pos;

        go.GetComponent<Button>().onClick.AddListener(onClick);

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = name;
        tmp.fontSize = 44;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        return tmp;
    }

    private void CreateLabel(Transform parent, string text, float fontSize, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject("Header", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
    }

    private void CreateButton(Transform parent, string text, Color color, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(text + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600, 120);
        rt.anchoredPosition = pos;

        go.GetComponent<Button>().onClick.AddListener(onClick);

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 40;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
    }
}
