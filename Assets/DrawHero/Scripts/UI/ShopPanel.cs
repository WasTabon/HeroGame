using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using TMPro;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    public static void Open()
    {
        GameObject go = new GameObject("ShopPanel");
        ShopPanel panel = go.AddComponent<ShopPanel>();
        panel.Build();
    }

    private RectTransform panel;

    private void Build()
    {
        GameObject canvasGo = new GameObject("ShopCanvas");
        canvasGo.transform.SetParent(transform);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32761;
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

        GameObject backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(Button));
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
        backdrop.GetComponent<Button>().onClick.AddListener(Close);

        GameObject panelGo = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panelGo.transform.SetParent(canvasGo.transform, false);
        panelGo.GetComponent<Image>().color = new Color(0.14f, 0.14f, 0.24f, 1f);
        panel = panelGo.GetComponent<RectTransform>();
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(820, 900);
        panel.anchoredPosition = Vector2.zero;

        CreateLabel(panel, "POWER PACK", 60, new Color(0.98f, 0.78f, 0.20f), new Vector2(0, -70), new Vector2(760, 120));
        CreateLabel(panel, "1x Heavy Ink\n1x Extra Ink\n1x Bomb Ink", 42, Color.white, new Vector2(0, -240), new Vector2(760, 300));

        BuildBuyButton(panel);

        CreateCloseButton(panel, new Vector2(0, -360));

        panel.localScale = Vector3.zero;
        panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.05f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop");
    }

    private void BuildBuyButton(Transform parent)
    {
        GameObject go = new GameObject("BuyButton", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.4f, 0.75f, 0.4f);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(560, 150);
        rt.anchoredPosition = new Vector2(0, -160);

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "BUY";
        tmp.fontSize = 48;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        go.GetComponent<Button>().onClick.AddListener(OnBuyClicked);

        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.OnPowerPackPurchased -= HandlePurchased;
            IAPManager.Instance.OnPowerPackPurchased += HandlePurchased;
        }
    }

    private void OnBuyClicked()
    {
        if (IAPManager.Instance != null)
            IAPManager.Instance.BuyPowerPack();
    }

    private void HandlePurchased()
    {
        Close();
    }

    private void OnDestroy()
    {
        if (IAPManager.Instance != null)
            IAPManager.Instance.OnPowerPackPurchased -= HandlePurchased;
    }

    private void Close()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("click");

        panel.DOScale(0f, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }

    private void CreateCloseButton(Transform parent, Vector2 pos)
    {
        GameObject go = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.961f, 0.651f, 0.137f);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(560, 120);
        rt.anchoredPosition = pos;

        go.GetComponent<Button>().onClick.AddListener(Close);

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "CLOSE";
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

    private void CreateLabel(Transform parent, string text, float fontSize, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject("Label", typeof(RectTransform));
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
}