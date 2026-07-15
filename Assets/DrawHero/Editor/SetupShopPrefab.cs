using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class SetupShopPrefab
{
    private const string RESOURCES_DIR = "Assets/DrawHero/Resources";
    private const string PREFAB_PATH = "Assets/DrawHero/Resources/ShopPanel.prefab";

    [MenuItem("Tools/DrawHero/Generate Shop Prefab")]
    public static void GeneratePrefab()
    {
        if (!System.IO.Directory.Exists(RESOURCES_DIR))
            System.IO.Directory.CreateDirectory(RESOURCES_DIR);

        GameObject root = new GameObject("ShopPanel");

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32761;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        root.AddComponent<GraphicRaycaster>();

        ShopPanel script = root.AddComponent<ShopPanel>();

        GameObject backdropGo = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(Button));
        backdropGo.transform.SetParent(root.transform, false);
        backdropGo.GetComponent<Image>().color = Color.black;
        RectTransform bdRt = backdropGo.GetComponent<RectTransform>();
        bdRt.anchorMin = Vector2.zero;
        bdRt.anchorMax = Vector2.one;
        bdRt.offsetMin = Vector2.zero;
        bdRt.offsetMax = Vector2.zero;

        GameObject panelGo = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panelGo.transform.SetParent(root.transform, false);
        panelGo.GetComponent<Image>().color = new Color(0.14f, 0.14f, 0.24f, 1f);
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(820, 1000);
        panelRt.anchoredPosition = Vector2.zero;

        RectTransform titleRt = CreateText(panelGo.transform, "Title", "POWER PACK", 60,
            new Color(0.98f, 0.78f, 0.20f));
        titleRt.anchorMin = new Vector2(0.5f, 1f);
        titleRt.anchorMax = new Vector2(0.5f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.sizeDelta = new Vector2(760, 120);
        titleRt.anchoredPosition = new Vector2(0, -60);

        GameObject iconGo = new GameObject("PackIcon", typeof(RectTransform), typeof(Image));
        iconGo.transform.SetParent(panelGo.transform, false);
        Image packIcon = iconGo.GetComponent<Image>();
        packIcon.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        packIcon.color = Color.white;
        packIcon.preserveAspect = true;
        RectTransform iconRt = iconGo.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0.5f, 1f);
        iconRt.anchorMax = new Vector2(0.5f, 1f);
        iconRt.pivot = new Vector2(0.5f, 1f);
        iconRt.sizeDelta = new Vector2(200, 200);
        iconRt.anchoredPosition = new Vector2(0, -190);

        RectTransform contentsRt = CreateText(panelGo.transform, "Contents",
            "1x Heavy Ink\n1x Extra Ink\n1x Bomb Ink", 40, Color.white);
        contentsRt.anchorMin = new Vector2(0.5f, 1f);
        contentsRt.anchorMax = new Vector2(0.5f, 1f);
        contentsRt.pivot = new Vector2(0.5f, 1f);
        contentsRt.sizeDelta = new Vector2(760, 220);
        contentsRt.anchoredPosition = new Vector2(0, -410);

        Button buyBtn = CreateButton(panelGo.transform, "BuyButton", "BUY",
            new Color(0.4f, 0.75f, 0.4f), new Vector2(0, -180), new Vector2(560, 150));

        Button closeBtn = CreateButton(panelGo.transform, "CloseButton", "CLOSE",
            new Color(0.961f, 0.651f, 0.137f), new Vector2(0, -360), new Vector2(560, 120));

        script.backdrop = backdropGo.GetComponent<CanvasGroup>();
        script.backdropButton = backdropGo.GetComponent<Button>();
        script.panel = panelRt;
        script.buyButton = buyBtn;
        script.closeButton = closeBtn;
        script.titleText = titleRt.GetComponent<TextMeshProUGUI>();
        script.contentsText = contentsRt.GetComponent<TextMeshProUGUI>();
        script.packIcon = packIcon;

        PrefabUtility.SaveAsPrefabAsset(root, PREFAB_PATH);
        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("DrawHero: ShopPanel prefab created at " + PREFAB_PATH + ". Edit sprites there.");
    }

    private static RectTransform CreateText(Transform parent, string name, string text, float fontSize, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return go.GetComponent<RectTransform>();
    }

    private static Button CreateButton(Transform parent, string name, string text, Color color, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(ButtonPunch));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        GameObject textGo = new GameObject("Label", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 46;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }
}
